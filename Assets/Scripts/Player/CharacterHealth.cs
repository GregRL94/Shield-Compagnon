using System.Collections;
using UnityEngine;

public class CharacterHealth : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _iFrameDuration = 0.25f;
    [SerializeField] private float _gameOverDelay = 1f;

    private float _currentHealth;
    private bool _isInvulnerable;
    private bool _isDead;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(int damage)
    {
        // Si frame d'invulnérabilité active, ne pas prendre de dégâts
        if (_isInvulnerable) { return; }
        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, _maxHealth);

        // Frame d'invulnérabilité
        SetInvulnerable(_iFrameDuration);

        if (_isDead ) { return; }
        if (_currentHealth <= 0f)
        {
            StartCoroutine(Die());
        }
    }

    public void SetInvulnerable(float duration)
    {
        StartCoroutine(Invulnerability(duration));
    }

    IEnumerator Invulnerability(float invulnerabilityDuration)
    {
        _isInvulnerable = true;
        Debug.Log("Invulnerability activated for " + invulnerabilityDuration + " seconds.");
        yield return new WaitForSeconds(invulnerabilityDuration);
        _isInvulnerable = false;
        Debug.Log("Invulnerability ended.");
    }

    IEnumerator Die()
    {
        _isDead = true;
        PlayerInputHandler.Instance.enabled = false;
        Debug.Log("Deactivated inputs");
        yield return new WaitForSeconds(_gameOverDelay);
        // Handle game over logic here
        Debug.Log("Show game Over screen");
    }
}
