using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager _instance;
    public static GameManager Instance => _instance;
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return; 
        }

        _instance = this;
        DontDestroyOnLoad(gameObject); 
    }


    #endregion

    public GameObject gameOverScreen;

    public GameObject victoryScreen;

    public int AvaialableLives = 3;
    public int Lives { get; set; }

    public bool IsGameStarted { get; set; }


    public static event Action<int> OnLiveLost;

    private void Start()
    {
        this.Lives = this.AvaialableLives;

        int width = 1920;
        int height = 1080;
        bool isFullscreen = true; // Change to false if you want windowed mode
        Screen.SetResolution(width, height, isFullscreen);

        Ball.onBallDeath += OnBallDeath;
        Brick.OnBrickDestruction += OnBrickDestruction;
    }

    private void OnBrickDestruction(Brick obj)
    {
        if (BrickManager.Instance != null && BrickManager.Instance.RemainingBricks.Count <= 0)
        {
            StartCoroutine(TransitionToNextLevel());
        }
    }


    private IEnumerator TransitionToNextLevel()
    {
        GameManager.Instance.IsGameStarted = false;

        // Reset balls and wait briefly to avoid ball hitting new bricks
        BallsManager.Instance.ResetBalls();
        yield return new WaitForSeconds(0.5f);

        // Load the next level
        BrickManager.Instance.LoadNextLevel();

        
    }


    //public void RestartGame()
    //{
    //    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    //}

    private void OnBallDeath(Ball obj)
    {
        if (BallsManager.Instance != null && BallsManager.Instance.Balls.Count <= 0)
        {
            this.Lives--;
        }

        if (this.Lives < 1)
        {
            if (gameOverScreen != null)
                gameOverScreen.SetActive(true);
        }
        else
        {
            OnLiveLost?.Invoke(this.Lives);
            BallsManager.Instance?.ResetBalls();
            IsGameStarted = false;
            BrickManager.Instance?.LoadLevel(BrickManager.Instance.CurrentLevel);
        }
    }


    internal void ShowVictoryScreen()
    {
        victoryScreen.SetActive(true);
    }

    private void OnDisable()
    {
        Ball.onBallDeath -= OnBallDeath;
        Brick.OnBrickDestruction -= OnBrickDestruction; 
    }


    public void RestartGame()
    {
        Debug.Log("Restarting game...");

        BrickManager.Instance?.ResetLevel(); // Reset the bricks.
        BallsManager.Instance?.ResetBalls(); // Reset the balls.
        Lives = AvaialableLives;             // Reset lives.
        IsGameStarted = false;               // Mark the game as not started.

        if (gameOverScreen != null)
            gameOverScreen.SetActive(false); // Hide the game over screen.

        if (victoryScreen != null)
            victoryScreen.SetActive(false);  // Hide the victory screen.

        Debug.Log("Game restarted successfully. Lives reset to " + Lives);
    }



}
