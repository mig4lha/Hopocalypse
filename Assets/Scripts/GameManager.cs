using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // A list of all level configurations you want to support (assign your LevelData assets in the Inspector)
    public List<LevelData> levels;
    public int currentLevelIndex = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Automatically load the first level (index 0)
        LoadLevel(0);
    }

    // Returns the current level's data
    public LevelData GetCurrentLevelData()
    {
        return levels[currentLevelIndex];
    }

    public int GetCurrentLevelIndex()
    {
        return currentLevelIndex;
    }

    // Load a level by index (assumes your scenes are named according to LevelData.levelName)
    public void LoadLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levels.Count)
        {
            currentLevelIndex = levelIndex;
            SceneManager.LoadScene(levels[currentLevelIndex].levelName);
        }
        else
        {
            Debug.LogWarning("Invalid level index.");
        }
    }
}
