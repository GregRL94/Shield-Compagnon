using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _playerCamAnchor;
    [SerializeField] private GameObject _gameOverMenu;
    [SerializeField] private Transform[] _playerRespawnsPoints;
    [SerializeField] private GameObject[] _enemiesToRespawn;
    [SerializeField] private Transform[] _enemiesRespawnPoints;
    [SerializeField] private Transform[] _enemyPatrolPoints;
    [SerializeField] private CinemachineCamera _virtualCamera;

    public static GameManager Instance { get; private set; }
    
    int _currentRespawnIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    public void RespawnPlayer()
    {
        Debug.Log("Replacing Player");        
        _player.transform.position = _playerRespawnsPoints[_currentRespawnIndex].position;
        _player.transform.rotation = _playerRespawnsPoints[_currentRespawnIndex].rotation;
        _player.GetComponent<CharacterHealth>().ResetHealth();
        _player.GetComponent<CharacterActions>().ResetShield();
        _player.GetComponent<CharacterMovement>().ResetDashCooldown();
        _player.SetActive(true);
        PlayerInputHandler.Instance.enabled = true;
    }

    public void RespawnEnemies()
    {
        if (_currentRespawnIndex == 0) { return; }

        foreach (EnemyBaseAI enemy in GameObject.FindObjectsByType<EnemyBaseAI>(FindObjectsSortMode.None))
        {
            Destroy(enemy.gameObject);
        }

        for (int i = 0; i < _enemiesToRespawn.Length; i++)
        {
            EnemyBaseAI enemy = Instantiate(_enemiesToRespawn[i], _enemiesRespawnPoints[i].position, _enemiesRespawnPoints[i].rotation).GetComponent<EnemyBaseAI>();
            if (enemy != null && _enemyPatrolPoints.Length > 0)
            {
                enemy.Waypoints = _enemyPatrolPoints;
            }
        }
    }

    public void IncreaseRespawnPointIndex()
    {
        _currentRespawnIndex++;
        if (_currentRespawnIndex >= _playerRespawnsPoints.Length)
        {
            _currentRespawnIndex = _playerRespawnsPoints.Length - 1;
        }
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
        Time.timeScale = 0f;
        _player.SetActive(false);
        _gameOverMenu.SetActive(true);
    }

    public void LastCheckpoint()
    {
        RespawnPlayer();
        RespawnEnemies();
        _gameOverMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void RestartGame()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //RespawnPlayer();
        //_gameOverMenu.SetActive(false);
        //Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
