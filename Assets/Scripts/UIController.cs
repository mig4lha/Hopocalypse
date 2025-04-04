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
    [SerializeField, Tooltip("TextMesh de Saltos")]
    private TMP_Text currentConsecutiveJumpsText;

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

    public void UpdateMovementUI(float velocity, float currentBhopMultiplier, int consecutiveJumps)
    {
        // Update no UI
        currentSpeedText.text = $"Speed: {velocity:F2} m/s";
        currentHopMultiplierText.text = $"Hop Mult: {currentBhopMultiplier:F2}x";
        currentConsecutiveJumpsText.text = $"Jumps: {consecutiveJumps}";
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
