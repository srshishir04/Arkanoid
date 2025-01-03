using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class BrickManager : MonoBehaviour
{
    #region Singleton
    private static BrickManager _instance;

    public static BrickManager Instance => _instance;

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

    private int maxRows = 17;
    private int maxCols = 12;
    private GameObject bricksContainer;
    private float initialBrickSpawnPositionX = -1.96f;
    private float initialBrickSpawnPositionY = 3.325f;
    private float shiftAmount = 0.365f;

    public Brick brickPrefab;

    public Sprite[] Sprites;

    public Color[] BrickColors;

    public List<Brick> RemainingBricks { get; set; }

    public List<int[,]> LevelsData { get; set; }

    public int InitialBricksCount { get; set; }

    public int CurrentLevel;

    private void Start()
    {
        this.bricksContainer = new GameObject("BricksContainer");
        this.RemainingBricks = new List<Brick>();
        this.LevelsData = this.LoadLevelsData();
        this.GenerateBricks();

    }

    public void LoadNextLevel()
    {
        this.CurrentLevel++;

        if (this.CurrentLevel >= this.LevelsData.Count)
        {
            GameManager.Instance.ShowVictoryScreen();
        }
        else
        {
            this.LoadLevel(this.CurrentLevel);
        }
    }

    public void LoadLevel(int level)
    {
        this.CurrentLevel = level;
        this.ClearRemainingBricks();
        this.GenerateBricks();
    }

    private void ClearRemainingBricks()
    {
        Debug.Log($"Clearing {RemainingBricks.Count} remaining bricks...");
        for (int i = this.RemainingBricks.Count - 1; i >= 0; i--)
        {
            Brick brick = this.RemainingBricks[i];
            if (brick != null)
            {
                Debug.Log($"Destroying brick: {brick.gameObject.name}");
                Destroy(brick.gameObject);
            }
            else
            {
                Debug.LogWarning("Found null brick in RemainingBricks list.");
            }
        }
        this.RemainingBricks.Clear();
        Debug.Log("Remaining bricks cleared successfully.");
    }


    private void OnDestroy()
    {
        // Clean up subscriptions to avoid memory leaks.
        OnLevelLoaded = null;
    }




    //private void ClearRemainingBricks()
    //{
    //    foreach (Brick brick in this.RemainingBricks.ToList())
    //    {
    //        if (brick != null) // Check if the brick still exists
    //        {
    //            Destroy(brick.gameObject);
    //        }
    //        this.RemainingBricks.Remove(brick); // Safely remove the brick from the list
    //    }
    //}


    //private void GenerateBricks()
    //{
    //    int[,] currentLevelData = this.LevelsData[this.CurrentLevel];
    //    float currentSpawnX = initialBrickSpawnPositionX;
    //    float currentSpawnY = initialBrickSpawnPositionY;
    //    float zShift = 0;

    //    for (int row = 0; row < this.maxRows; row++)
    //    {
    //        for (int col = 0; col < this.maxCols; col++)
    //        {
    //            int brickType = currentLevelData[row, col];

    //            if (brickType > 0)
    //            {
    //                Brick newBrick = Instantiate(brickPrefab, new Vector3(currentSpawnX, currentSpawnY, 0.0f - zShift), Quaternion.identity) as Brick;
    //                newBrick.Init(bricksContainer.transform, this.Sprites[brickType - 1], this.BrickColors[brickType], brickType);

    //                this.RemainingBricks.Add(newBrick);
    //                zShift += 0.0001f;
    //            }

    //            currentSpawnX += shiftAmount;
    //            if (col + 1 == this.maxCols)
    //            {
    //                currentSpawnX = initialBrickSpawnPositionX;
    //            }
    //        }

    //        currentSpawnY -= shiftAmount;
    //    }
    //    this.InitialBricksCount = this.RemainingBricks.Count;
    //    OnLevelLoaded?.Invoke();

    //}

    private void GenerateBricks()
    {
        this.RemainingBricks.Clear(); // Clear any stale references.

        int[,] currentLevelData = this.LevelsData[this.CurrentLevel];
        float currentSpawnX = initialBrickSpawnPositionX;
        float currentSpawnY = initialBrickSpawnPositionY;
        float zShift = 0;

        for (int row = 0; row < this.maxRows; row++)
        {
            for (int col = 0; col < this.maxCols; col++)
            {
                int brickType = currentLevelData[row, col];

                if (brickType > 0)
                {
                    Brick newBrick = Instantiate(brickPrefab, new Vector3(currentSpawnX, currentSpawnY, 0.0f - zShift), Quaternion.identity);
                    newBrick.Init(bricksContainer.transform, this.Sprites[brickType - 1], this.BrickColors[brickType], brickType);

                    this.RemainingBricks.Add(newBrick);
                    zShift += 0.0001f;
                }

                currentSpawnX += shiftAmount;
                if (col + 1 == this.maxCols)
                {
                    currentSpawnX = initialBrickSpawnPositionX;
                }
            }

            currentSpawnY -= shiftAmount;
        }

        this.InitialBricksCount = this.RemainingBricks.Count;
        Debug.Log($"Generated {RemainingBricks.Count} bricks for level {this.CurrentLevel}.");
        OnLevelLoaded?.Invoke();
    }


    private List<int[,]> LoadLevelsData()
    {
        TextAsset text = Resources.Load("levels") as TextAsset;

        string[] rows = text.text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

        List<int[,]> levelsData = new List<int[,]>();
        int[,] currentLevel = new int[maxRows, maxCols];
        int currentRow = 0;

        for (int row = 0; row < rows.Length; row++)
        {
            string line = rows[row];

            if (line.IndexOf("--") == -1)
            {
                string[] bricks = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int col = 0; col < bricks.Length; col++)
                {
                    currentLevel[currentRow, col] = int.Parse(bricks[col]);


                }

                currentRow++;

            }
            else
            {
                //end of the current level
                currentRow = 0;
                levelsData.Add(currentLevel);
                currentLevel = new int[maxRows, maxCols];

            }

        }

        return levelsData;
    }

    public void ResetLevel()
    {
        Debug.Log("Resetting level...");
        ClearRemainingBricks();
        GenerateBricks();
        Debug.Log("Level reset complete. Total bricks: " + RemainingBricks.Count);
    }



    //public void ResetBricks()
    //{
    //    ClearRemainingBricks();
    //    GenerateBricks();
    //}
}