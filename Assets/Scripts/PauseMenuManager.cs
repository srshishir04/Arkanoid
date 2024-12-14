using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    private const string SavedLevelKey = "SavedLevel";
    private const string PlayerPositionKey = "PlayerPosition";

    public GameObject pauseMenu; // Assign your pause menu UI in the inspector

    void Update()
    {
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


    // Pause the game
    public void PauseGame()
    {
        Time.timeScale = 0; // Freeze the game
        pauseMenu.SetActive(true); // Show the pause menu
    }

    // Resume the game
    public void ResumeGame()
    {
        Time.timeScale = 1; // Unfreeze the game
        pauseMenu.SetActive(false); // Hide the pause menu
    }

    // Restart the current level
    public void RestartGame()
    {
        Time.timeScale = 1; // Ensure time resumes
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene); // Reload the current scene
    }

    // Return to main menu
    public void MainMenu()
    {
        Time.timeScale = 1; // Ensure time resumes
        string currentScene = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString(SavedLevelKey, currentScene);

        // Save player's position (optional)
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Vector3 playerPosition = player.transform.position;
            PlayerPrefs.SetString(PlayerPositionKey, $"{playerPosition.x},{playerPosition.y},{playerPosition.z}");
        }

        PlayerPrefs.Save();
        SceneManager.LoadScene("MainMenu"); // Replace "MainMenu" with your main menu scene name
    }

    // Quit the game
    public void QuitGame()
    {
        PlayerPrefs.DeleteKey(SavedLevelKey);
        PlayerPrefs.DeleteKey(PlayerPositionKey);
        Debug.Log("Exiting game...");

        SceneManager.LoadScene("MainMenu");
    }
}

