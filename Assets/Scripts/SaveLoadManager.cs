using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

// Serializable class to store game data
[System.Serializable]



public class GameData
{
    public int currentLevel;
}

public class SaveLoadManager : MonoBehaviour
{
    #region Singleton
    private static SaveLoadManager _instance;

    public static SaveLoadManager Instance => _instance;

    public static event Action OnLevelLoaded;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    #endregion

    private static string savePath => Application.persistentDataPath + "/save.dat";

    /// <summary>
    /// Saves the current game progress.
    /// </summary>
    public void SaveGame(int currentLevel)
    {
        GameData data = new GameData { currentLevel = currentLevel };

        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(savePath, FileMode.Create))
            {
                formatter.Serialize(stream, data);
            }
            Debug.Log($"Game saved successfully. Current Level: {currentLevel}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to save game: {ex.Message}");
        }
    }

    /// <summary>
    /// Loads the saved game progress.
    /// </summary>
    public int LoadGame()
    {
        if (File.Exists(savePath))
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream stream = new FileStream(savePath, FileMode.Open))
                {
                    GameData data = (GameData)formatter.Deserialize(stream);
                    Debug.Log($"Game loaded successfully. Loaded Level: {data.currentLevel}");
                    return data.currentLevel;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to load save file: {ex.Message}");
            }
        }
        else
        {
            Debug.LogWarning("No save file found. Starting at Level 0.");
        }

        return 0; // Default to level 0 if no save exists
    }

    /// <summary>
    /// Checks if a save file exists.
    /// </summary>
    public bool HasSave()
    {
        return File.Exists(savePath);
    }

    /// <summary>
    /// Deletes the save file to reset game progress.
    /// </summary>
    public void ClearSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Save file deleted. Progress reset.");
        }
        else
        {
            Debug.LogWarning("No save file found to delete.");
        }
    }

    internal void SaveGame()
    {
        throw new NotImplementedException();
    }

    public static implicit operator SaveLoadManager(LevelManager v)
    {
        throw new NotImplementedException();
    }
}
