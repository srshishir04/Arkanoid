using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControl : MonoBehaviour
{
    public GameObject settingsPanel;
    private SaveLoadManager saveLoadManager;

    void Start()
    {
        saveLoadManager = FindObjectOfType<SaveLoadManager>();

        if (saveLoadManager == null)
        {
            Debug.LogError("SaveLoadManager not found in the scene!");
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");

        // Safely subscribe to scene loaded event
        SceneManager.sceneLoaded += OnGameSceneLoaded;
    }

    private void OnGameSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene")
        {
            if (BrickManager.Instance != null)
            {
                int savedLevel = (saveLoadManager != null && saveLoadManager.HasSave())
                    ? saveLoadManager.LoadGame()
                    : 0; // Load saved level or start fresh

                BrickManager.Instance.LoadLevel(savedLevel);
            }
            else
            {
                Debug.LogError("BrickManager not found in the GameScene!");
            }

            // Unsubscribe after handling
            SceneManager.sceneLoaded -= OnGameSceneLoaded;
        }
    }

    public void MainMenu()
    {
        if (saveLoadManager != null && BrickManager.Instance != null)
        {
            saveLoadManager.SaveGame(BrickManager.Instance.CurrentLevel); // Save progress
        }

        Time.timeScale = 1; // Reset time scale
        SceneManager.LoadScene("MainMenu");
    }

    public void ShowOptions()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(!settingsPanel.activeSelf);
        }
        else
        {
            Debug.LogError("Settings panel is not assigned!");
        }
    }

    public void QuitGame()
    {
        if (saveLoadManager != null)
        {
            saveLoadManager.ClearSave();
        }

        Debug.Log("Exiting game...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
