using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private GameObject _pauseCover;
    private static bool _gameIsPaused = false;
    private GameObject _resumeButton;
    private GameObject _menuButton;
    private GameObject _exitButton;
    private static bool _isGameOver;
    public static void GameOver()
    {
        _isGameOver = true;
    }
    private void Start()
    {
        _gameIsPaused = false;
        _isGameOver = false;
        _pauseCover = GameObject.Find("PauseCover");
        _resumeButton = GameObject.Find("ResumeButton");
        _menuButton = GameObject.Find("MenuButton");
        _exitButton = GameObject.Find("ExitButton");
        Resume();
    }
    private void Update()
    {
        if (_isGameOver && !_gameIsPaused)
        {
            Pause();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_gameIsPaused && !_isGameOver)
            {
                Resume();
            }
            else
            {
                if (_gameIsPaused)
                {
                    return;
                }
                Pause();
            }
        }
    }
    public void Resume()
    {
        _gameIsPaused = false;
        //_pauseCover.SetActive(false);
        _menuButton.SetActive(false);
        _exitButton.SetActive(false);
        _resumeButton.SetActive(false);
        Time.timeScale = 1f;
    }
    public void Pause()
    {
        _gameIsPaused = true;
        //_pauseCover.SetActive(true);
        _menuButton.SetActive(true);
        _exitButton.SetActive(true);
        if(!_isGameOver)
        {
            _resumeButton.SetActive(true);
        }
        Time.timeScale = 0f;
    }
    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
