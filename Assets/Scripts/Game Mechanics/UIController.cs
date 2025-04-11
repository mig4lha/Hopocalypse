using System;
using TMPro;
using UnityEngine;

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
}
