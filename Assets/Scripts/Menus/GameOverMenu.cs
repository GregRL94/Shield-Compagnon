using UnityEngine;

public class GameOverMenu : MonoBehaviour
{
    GameManager _gameManager;
    public static GameOverMenu Instance { get; private set; }

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
        _gameManager = GameManager.Instance;
    }

    public void OnLastCheckPointClicked()
    {
        _gameManager.LastCheckpoint();
    }

    public void OnRestartClicked()
    {
        _gameManager.RestartGame();
    }

    public void OnQuitClicked()
    {
        _gameManager.QuitGame();
    }
}
