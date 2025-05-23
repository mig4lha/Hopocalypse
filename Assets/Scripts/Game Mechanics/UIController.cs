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
        // Atualizar tempo passado no ecr� no fomarto "Time: MM:SS"
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
        // Atualizar enemies spawned no ecr�
        if (currentEnemiesSpawned != null)
            currentEnemiesSpawned.text = $"Enemies Spawned: {enemiesSpawned}";
    }

    public void UpdateEnemiesDefeated(int enemiesDefeated)
    {
        // Atualizar enemies defeated no ecr�
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
        // Atualizar timer de reload no ecr�
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

    public void AddStatusEffect(StatusEffectData effectData)
    {
        string effectName = effectData.name;
        Sprite iconSprite = effectData.icon;

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
            newIconImage.color = DEBUG_GetColorForStatusEffect(effectData);
        }
    }

    private Color DEBUG_GetColorForStatusEffect(StatusEffectData effectData)
    {
        switch (effectData.effectType)
        {
            // PowerUps
            case EffectType.HopFury:
                return Color.green;
            case EffectType.ReloadBoost:
                return Color.yellow;
            case EffectType.ShotgunOvercharge:
                return Color.red;
            case EffectType.PelletIncrease:
                return Color.blue;
            case EffectType.ExtraClip:
                return Color.cyan;
            case EffectType.SpreadPlus:
                return Color.magenta;
            case EffectType.Shield:
                // return rgb(92, 76, 54)
                return new Color(0.36f, 0.3f, 0.21f); // Dark Brown
            case EffectType.HopWindowUp:
                return Color.grey;
            case EffectType.HealthRegen:
                return Color.black;
            case EffectType.RicochetPellets:
                return new Color(0.01f, 1, 0.83f); // Light Cyan

            // Debuffs
            case EffectType.MovementDown:
                // return color purple
                return new Color(0.5f, 0, 0.5f); // Purple
            case EffectType.HeavyReload:
                // return color orange
                return new Color(1, 0.5f, 0); // Orange
            case EffectType.LessDamage:
                // return color pink
                return new Color(1, 0.75f, 0.8f); // Pink
            case EffectType.ShotgunJam:
                //return color dark pink
                return new Color(1, 0.2f, 0.6f); // Dark Pink
            case EffectType.PelletDecrease:
                // return dark green
                return new Color(0, 0.5f, 0); // Dark Green
            case EffectType.SpreadMinus:
                // return light green
                return new Color(0.5f, 1, 0.5f); // Light Green
            case EffectType.RangeDown:
                // return dark red
                return new Color(0.5f, 0, 0); // Dark Red
            case EffectType.HopWindowDown:
                // return dark blue
                return new Color(0, 0, 0.5f); // Dark Blue
            case EffectType.BloodLoss:
                // return rgb(115, 42, 86)
                return new Color(0.45f, 0.16f, 0.34f); // Dark Red
            case EffectType.Clipless:
                // return rgb(82, 120, 50)
                return new Color(0.32f, 0.47f, 0.2f); // Dark Green
        }

        return Color.white;
    }

    public void RemoveStatusEffect(StatusEffectData effectData)
    {
        string effectName = effectData.name;

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
