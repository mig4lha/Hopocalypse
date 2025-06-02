using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public List<LevelData> levels;
    public int currentLevelIndex = 0;
    [SerializeField] private Volume pauseBlurVolume;
    public PlayerData playerData = new PlayerData();

    private PlayerStats playerStats;
    private AxeGunController axeGunController;
    private StatusEffectController statusEffectController;
    private UIController uiController;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            if (pauseBlurVolume != null)
                DontDestroyOnLoad(pauseBlurVolume.gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Automatically load the first level (index 0)
        LoadMainMenu();
        //LoadLevel(0);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.StartsWith("Level"))
        {
            playerStats = FindAnyObjectByType<PlayerStats>();
            if (playerStats == null)
            {
                Debug.LogError("PlayerStats not found in the scene.");
            }

            axeGunController = FindAnyObjectByType<AxeGunController>();
            if (axeGunController == null)
            {
                Debug.LogError("AxeGunController not found in the scene.");
            }

            statusEffectController = FindAnyObjectByType<StatusEffectController>();
            if (statusEffectController == null)
            {
                Debug.LogError("StatusEffectController not found in the scene.");
            }

            uiController = FindAnyObjectByType<UIController>();
            if (uiController == null)
            {
                Debug.LogError("UIController not found in the scene.");
            }

            if (scene.name != "Level1")
            {
                LoadPlayerState();
            }
        }
    }

    private void LoadMainMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Load the main menu scene
        SceneManager.LoadScene("MainMenu");
    }

    // Returns the current level's data
    public LevelData GetCurrentLevelData(){
        return levels[currentLevelIndex];
    }

    public int GetCurrentLevelIndex()
    {
        return currentLevelIndex;
    }

    // Increment the level index
    public void IncrementLevelIndex()
    {
        currentLevelIndex++;
        Debug.Log("Current Level Index: " + currentLevelIndex);
    }

    // Load a level by index
    public void LoadLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levels.Count)
        {
            currentLevelIndex = levelIndex;
            Debug.Log(GetCurrentLevelIndex() + " - " + levels[currentLevelIndex].levelName);
            SceneManager.LoadScene(levels[currentLevelIndex].levelName);
        }
        else
        {
            Debug.LogWarning("Invalid level index.");
        }
    }

    internal void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void SavePlayerState()
    {
        playerData.health = playerStats.health;
        playerData.activeEffects = statusEffectController.GetActiveEffects();
        Debug.Log("Player state saved: " +
                  "Health: " + playerData.health + ", " +
                  "Effects: " + playerData.activeEffects.ToString());
    }

    public void LoadPlayerState()
    {
        playerStats.health = playerData.health;
        statusEffectController.ApplyEffectsByList(playerData.activeEffects);

        if (uiController != null)
        {
            uiController.UpdateHealthBar(playerStats.health, playerStats.maxHealth);
        }

        Debug.Log("Player state loaded: " +
                  "Health: " + playerStats.health + ", " +
                  "Effects: " + statusEffectController.GetActiveEffects().ToString());
    }

}
