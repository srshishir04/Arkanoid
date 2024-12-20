using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenu; // Reference to the Pause Menu UI
    private SaveLoadManager saveLoadManager;

    void Start()
    {
        // Attempt to find SaveLoadManager in the scene
        saveLoadManager = FindObjectOfType<SaveLoadManager>();

        if (saveLoadManager == null)
        {
            Debug.LogError("SaveLoadManager not found in the scene!");
        }

        // Ensure the Pause Menu is hidden on start
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
    }

    void Update()
    {
        // Toggle pause with the Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 0)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0; // Pause the game
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(true);
        }
        else
        {
            Debug.LogError("Pause Menu UI is not assigned in the Inspector.");
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1; // Resume the game
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1; // Reset time scale
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the current scene
    }

    public void MainMenu()
    {
        Time.timeScale = 1; // Reset time scale

        // Save the game progress if SaveLoadManager exists
        if (saveLoadManager != null)
        {
            if (BrickManager.Instance != null)
            {
                saveLoadManager.SaveGame(BrickManager.Instance.CurrentLevel);
            }
            else
            {
                Debug.LogWarning("BrickManager instance not found. Skipping save.");
            }
        }
        else
        {
            Debug.LogWarning("SaveLoadManager is not initialized. Skipping save.");
        }

        // Load the MainMenu scene
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        if (saveLoadManager != null)
        {
            saveLoadManager.ClearSave();
        }

        SceneManager.LoadScene("MainMenu");
    }
}









//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class PauseMenuManager : MonoBehaviour
//{
//    public GameObject pauseMenu;
//    private SaveLoadManager saveLoadManager;

//    void Start()
//    {
//        saveLoadManager = FindObjectOfType<SaveLoadManager>();
//    }

//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.Escape))
//        {
//            if (Time.timeScale == 0)
//            {
//                ResumeGame();
//            }
//            else
//            {
//                PauseGame();
//            }
//        }
//    }

//    public void PauseGame()
//    {
//        Time.timeScale = 0;
//        pauseMenu.SetActive(true);
//    }

//    public void ResumeGame()
//    {
//        Time.timeScale = 1;
//        pauseMenu.SetActive(false);
//    }

//    public void RestartGame()
//    {
//        Time.timeScale = 1;
//        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
//    }

//    public void MainMenu()
//    {
//        Time.timeScale = 1;

//        if (saveLoadManager != null)
//        {
//            saveLoadManager.SaveGame();
//        }

//        SceneManager.LoadScene("MainMenu");
//    }

//public void QuitGame()
//    {
//        if (saveLoadManager != null)
//        {
//            saveLoadManager.ClearSave();
//        }

//        SceneManager.LoadScene("MainMenu");
//    }
//}
