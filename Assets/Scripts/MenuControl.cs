using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControl : MonoBehaviour
{
    private const string SavedLevelKey = "SavedLevel"; // Key to store the saved level

    public GameObject settingsPanel;

    // Called when Start is pressed in the Main Menu
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");

        if (PlayerPrefs.HasKey(SavedLevelKey))
        {
            // Load saved level
            string savedLevel = PlayerPrefs.GetString(SavedLevelKey);
            SceneManager.LoadScene(savedLevel);
        }
        else
        {
            // Start from the first level
            SceneManager.LoadScene("Level1"); // Replace with your first level's scene name
        }
    }

    // Called when Main Menu is pressed in the Pause Menu
    public void MainMenu()
    {
        // Save the current level
        string currentScene = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString(SavedLevelKey, currentScene);
        PlayerPrefs.Save();

        // Load Main Menu scene
        SceneManager.LoadScene("MainMenu"); // Replace with your main menu's scene name
    }

    public void ShowOptions()
    {
        if (settingsPanel.activeSelf == false)
        {
            settingsPanel.SetActive(true);
        }
        else
        {
            settingsPanel.SetActive(false);
        }
    }

    // Called when Quit Game is pressed
    public void QuitGame()
    {
        // Clear saved progress
        PlayerPrefs.DeleteKey(SavedLevelKey);

        Debug.Log("Exiting game...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}