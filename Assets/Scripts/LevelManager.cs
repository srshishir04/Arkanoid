using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    // Singleton Instance of LevelManager
    public static LevelManager Instance { get; private set; }

    private SaveLoadManager saveLoadManager; // Reference to SaveLoadManager

    private void Awake()
    {
        // Singleton pattern for LevelManager
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Access SaveLoadManager Singleton safely
        saveLoadManager = SaveLoadManager.Instance;
        if (saveLoadManager == null)
        {
            Debug.LogError("SaveLoadManager instance not found! Ensure it exists in the scene.");
        }
    }

    public void StartGame()
    {
        if (saveLoadManager != null)
        {
            int savedLevel = saveLoadManager.LoadGame();
            SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
            BrickManager.Instance?.LoadLevel(savedLevel);
        }
        else
        {
            Debug.LogError("SaveLoadManager is not initialized.");
        }
    }

    public void NextLevel()
    {
        if (BrickManager.Instance != null && saveLoadManager != null)
        {
            BrickManager.Instance.LoadNextLevel();
            saveLoadManager.SaveGame(BrickManager.Instance.CurrentLevel);
        }
        else
        {
            Debug.LogError("BrickManager or SaveLoadManager is not initialized.");
        }
    }

    public void MainMenu()
    {
        if (saveLoadManager != null)
        {
            saveLoadManager.SaveGame(BrickManager.Instance?.CurrentLevel ?? 0);
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
        else
        {
            Debug.LogError("SaveLoadManager is not initialized.");
        }
    }
}
