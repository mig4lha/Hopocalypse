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

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject uiControllerPrefab;
    [SerializeField] private GameObject waveControllerPrefab;
    [SerializeField] private GameObject debugLineManagerPrefab;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            if (pauseBlurVolume != null)
                DontDestroyOnLoad(pauseBlurVolume.gameObject);
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

    // Load a level by index
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

    internal void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void SavePlayerState(PlayerStats player, AxeGunController gun, StatusEffectController effects)
    {
        playerData.health = player.health;
        playerData.currentAmmo = gun.currentAmmo;
        playerData.activeEffects = effects.GetActiveEffects();
        Debug.Log("Player state saved: " +
                  "Health: " + playerData.health + ", " +
                  "Ammo: " + playerData.currentAmmo + ", " +
                  "Effects: " + playerData.activeEffects.ToString());
    }

    public void LoadPlayerState(PlayerStats player, AxeGunController gun, StatusEffectController effects)
    {
        player.health = playerData.health;
        gun.currentAmmo = playerData.currentAmmo;
        effects.ApplyEffectsByList(playerData.activeEffects);
        Debug.Log("Player state loaded: " +
                  "Health: " + player.health + ", " +
                  "Ammo: " + gun.currentAmmo + ", " +
                  "Effects: " + effects.GetActiveEffects().ToString());
    }

}
