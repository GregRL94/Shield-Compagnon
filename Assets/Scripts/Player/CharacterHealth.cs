using System.Collections;
using UnityEngine;

public class CharacterHealth : MonoBehaviour, IHittable
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

    void TakeDamage(float damage)
    {
        if (_isInvulnerable) { return; }
        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, _maxHealth);
        Debug.Log("Player took " + damage + " damage. Current health: " + _currentHealth);

        // Invulnerability frame
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

    public void ResetHealth()
    {
        _currentHealth = _maxHealth;
        _isDead = false;
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
        GameManager.Instance.GameOver();
    }

    public void TakeHit(float value)
    {
        TakeDamage(value);
    }
}
