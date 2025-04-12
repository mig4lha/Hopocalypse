using System;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    // Debug UI variables
    [Header("Debug UI Elements")]
    [SerializeField, Tooltip("TextMesh da Velocidade")]
    private TMP_Text currentSpeedText;
    [SerializeField, Tooltip("TextMesh da Hop Mult")]
    private TMP_Text currentHopMultiplierText;
    [SerializeField, Tooltip("TextMesh de Reload Time")]
    private TMP_Text currentReloadTime;
    [SerializeField, Tooltip("TextMesh de Saltos")]
    private TMP_Text currentConsecutiveJumpsText;
    [SerializeField, Tooltip("TextMesh de Enemies Defeated")]
    private TMP_Text currentEnemiesDefeated;
    [SerializeField, Tooltip("TextMesh de Enemies Spawned")]
    private TMP_Text currentEnemiesSpawned;
    [SerializeField, Tooltip("TextMesh de Time Passed")]
    private TMP_Text currentTimePassed;

    [Header("UI Elements")]
    [SerializeField, Tooltip("TextMesh da Ammo Count")]
    private TMP_Text ammoTextMesh;
    [SerializeField, Tooltip("TextMesh do Reload Countdown")]
    private TMP_Text reloadCountdownText;
    [SerializeField, Tooltip("Crosshair GameObject")]
    private GameObject crosshair;
    [SerializeField, Tooltip("Health Bar GameObject")]
    private GameObject healthBar;
    [SerializeField] 
    private Transform statusEffectsContainer;
    [SerializeField] 
    private GameObject statusEffectIconPrefab;

    [Header("Player Data")]
    [SerializeField] private Player player;
    [Header("AxeGun Data")]
    [SerializeField] private AxeGunController axeGunController;

    public void UpdateTimePassed(float timePassed)
    {
        // Atualizar tempo passado no ecrã no fomarto "Time: MM:SS"
        TimeSpan time = TimeSpan.FromSeconds(timePassed);
        string formattedTime = string.Format("{0:D2}:{1:D2}", (int)time.TotalMinutes, time.Seconds);
        if (currentTimePassed != null)
            currentTimePassed.text = $"Time Passed:\n{formattedTime}";
    }

    public void UpdateCurrentReloadTime(float reloadTime)
    {
        if (currentReloadTime != null)
            currentReloadTime.text = $"Current Reload Time: {reloadTime}s";
    }

    public void UpdateEnemiesSpawned(int enemiesSpawned)
    {
        // Atualizar enemies spawned no ecrã
        if (currentEnemiesSpawned != null)
            currentEnemiesSpawned.text = $"Enemies Spawned: {enemiesSpawned}";
    }

    public void UpdateEnemiesDefeated(int enemiesDefeated)
    {
        // Atualizar enemies defeated no ecrã
        if (currentEnemiesDefeated != null)
            currentEnemiesDefeated.text = $"Enemies Defeated: {enemiesDefeated}";
    }

    public void UpdateMovementUI(float velocity, float currentBhopMultiplier, int consecutiveJumps)
    {
        consecutiveJumps--; // Decrementar para que o primeiro salto seja 0
        if (consecutiveJumps == -1) consecutiveJumps = 0;

        // Update no UI
        currentSpeedText.text = $"Speed: {velocity:F2} m/s";
        currentHopMultiplierText.text = $"Hop Mult: {currentBhopMultiplier:F2}x";
        currentConsecutiveJumpsText.text = $"Consecutive Jumps: {consecutiveJumps}";
    }

    public void UpdateAmmoUI(float currentAmmo)
    {
        if (ammoTextMesh != null)
            ammoTextMesh.text = currentAmmo.ToString();
    }

    public void UpdateReloadTimer(float reloadTimer)
    {
        // Atualizar timer de reload no ecrã
        reloadCountdownText.text = reloadTimer.ToString("F2");
    }

    public void HideReloadElement()
    {
        // Esconder reload text
        if (reloadCountdownText != null)
            reloadCountdownText.gameObject.SetActive(false);
    }

    public void HideCrosshairShowReload()
    {
        // Tirar crosshair durante relaod e meter countdown de reload
        if (crosshair != null)
            crosshair.SetActive(false);
        if (reloadCountdownText != null)
            reloadCountdownText.gameObject.SetActive(true);
    }

    public void ResetAmmoAndReloadUI()
    {
        // Reset do UI de reload e ammo
        reloadCountdownText.text = "";
        if (reloadCountdownText != null)
            reloadCountdownText.gameObject.SetActive(false);

        if (crosshair != null)
            crosshair.SetActive(true);
    }

    internal void UpdateHealthBar(float health, float maxHealth)
    {
        // Atualizar a barra de vida
        if (healthBar != null)
        {
            // get the reference to the health bar image object that is a child of the healthBar object and is named "Health"
            Image healthBarImage = healthBar.transform.Find("Health").GetComponent<Image>();

            healthBarImage.fillAmount = health / maxHealth;

            Debug.Log("Health bar updated: " + health);
        }
    }

    public void AddStatusEffect(Sprite iconSprite, string effectName)
    {
        // Instantiate the prefab as a child of the container
        GameObject newIcon = Instantiate(statusEffectIconPrefab, statusEffectsContainer);

        // Pass false for the second parameter to preserve local scale and anchoring
        newIcon.transform.SetParent(statusEffectsContainer, false);
        newIcon.name = effectName;


        // Set the sprite on the Image component
        Image newIconImage = newIcon.GetComponent<Image>();
        if (newIconImage != null)
        {
            newIconImage.sprite = iconSprite;
        }
    }

    public void RemoveStatusEffect(string effectName)
    {
        // find the icon gameobject by its name
        Transform icon = statusEffectsContainer.Find(effectName);
        GameObject iconGameObject = icon.gameObject;

        if(iconGameObject != null)
        {
            // Destroy the icon gameobject
            Destroy(iconGameObject);
        }
        else
        {
            Debug.LogWarning("Status effect icon not found: " + effectName);
        }
    }
}
