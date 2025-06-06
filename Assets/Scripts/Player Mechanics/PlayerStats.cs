using System;
using System.IO;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField, Tooltip("Vida do player")]
    public float health = 100f;
    [SerializeField, Tooltip("Vida Max do player")]
    public float maxHealth = 100f;
    [SerializeField, Tooltip("Valor de velocidade ao andar")]
    public float moveSpeed = 6f;
    [SerializeField, Tooltip("Multiplicador de velocidade de sprint")]
    public float sprintSpeedMultiplier = 1.5f;
    [SerializeField, Tooltip("Valor de força do salto")]
    public float jumpForce = 2.5f;
    [SerializeField, Tooltip("Valor de força do salto")]
    public float gravity = -80f;

    [Header("Jump Buffer Settings")]
    [SerializeField, Tooltip("Tempo (em segundos) para o jump buffer (coyote time)")]
    public float coyoteTime = 0.1f;

    [Header("Bunny Hop Settings")]
    [SerializeField, Tooltip("Tempo máximo entre saltos para considerar como consecutivos")]
    public float consecutiveJumpWindow = 0.4f;
    [SerializeField, Tooltip("Multiplicador máximo de velocidade por bunny hopping")]
    public float maxBhopMultiplier = 2f;
    [SerializeField, Tooltip("Aumento de multiplicador por salto consecutivo")]
    public float bhopMultiplierIncrement = 0.1f;
    [SerializeField, Tooltip("Tempo necessário no chão para iniciar um salto (evita spam de salto)")]
    public float jumpCooldown = 0.1f;
    [SerializeField, Tooltip("Controla o quão bem o jogador mantém a velocidade em curvas durante o bhop")]
    [Range(0f, 1f)]
    public float airControl = 0.7f;

    [Header("AxeGun Settings")]
    [SerializeField, Tooltip("Número balas por tiro")]
    public int pelletCount = 20;
    [SerializeField, Tooltip("Ângulo máximo do spread em X")]
    public float spreadAngleX = 50f;
    [SerializeField, Tooltip("Ângulo máximo do spread em Y")]
    public float spreadAngleY = 25f;
    [SerializeField, Tooltip("Range máximo de cada bala do tiro")]
    public float maxRange = 10f;
    [SerializeField, Tooltip("Dano de cada bala do tiro")]
    public float pelletDamage = 40f;

    [Header("Fire Rate Settings")]
    [SerializeField, Tooltip("Tempo de delay entre cada tiro")]
    public float fireRate = 0.3f;

    [Header("Ammo and Reload Settings")]
    [SerializeField, Tooltip("Max Ammo da AxeGun")]
    public int maxAmmo = 12;
    [SerializeField, Tooltip("AxeGun Reload Time")]
    public float reloadTime = 2f;

    private UIController UIController;

    private void Awake()
    {
        UIController = FindAnyObjectByType<UIController>();
        if (UIController == null)
        {
            Debug.LogError("UIController not found in the scene.");
        }
    }

    internal void AdjustReloadTime(float magnitude, EffectStrengthType effectStrengthType)
    {
        if(effectStrengthType == EffectStrengthType.Additive)
            reloadTime += magnitude;
        else if (effectStrengthType == EffectStrengthType.Multiplicative)
            reloadTime *= magnitude;

        if (reloadTime < 0.1f)
            reloadTime = 0.1f;

        UIController.UpdateCurrentReloadTime(reloadTime);

        Debug.Log("Reload time adjusted: " + reloadTime);
    }

    internal void AdjustClipSize(float magnitude, EffectStrengthType effectStrengthType)
    {
        float maxAmmoTemp = maxAmmo;

        if (effectStrengthType == EffectStrengthType.Additive)
            maxAmmoTemp += magnitude;
        else if (effectStrengthType == EffectStrengthType.Multiplicative)
            maxAmmoTemp *= magnitude;

        if (maxAmmoTemp <= 0)
            maxAmmoTemp = 1;

        maxAmmo = (int)maxAmmoTemp;

    }

    internal void AdjustHopSpeed(float magnitude, EffectStrengthType effectStrengthType)
    {
        if (effectStrengthType == EffectStrengthType.Additive)
            maxBhopMultiplier += magnitude;
        else if (effectStrengthType == EffectStrengthType.Multiplicative)
            maxBhopMultiplier *= magnitude;

        if (maxBhopMultiplier < 0.1f)
            maxBhopMultiplier = 0.1f;
    }

    internal void AdjustPelletCount(float magnitude, EffectStrengthType effectStrengthType)
    {
        float pelletCountTemp = pelletCount;

        if (effectStrengthType == EffectStrengthType.Additive)
            pelletCountTemp += magnitude;
        else if (effectStrengthType == EffectStrengthType.Multiplicative)
            pelletCountTemp *= magnitude;

        if (pelletCountTemp <= 0)
            pelletCountTemp = 1;

        pelletCount = (int)pelletCountTemp;
    }

    internal void AdjustShotgunDamage(float magnitude, EffectStrengthType effectStrengthType)
    {
        if (effectStrengthType == EffectStrengthType.Additive)
            pelletDamage += magnitude;
        else if (effectStrengthType == EffectStrengthType.Multiplicative)
            pelletDamage *= magnitude;

        if (pelletDamage < 0.1f)
            pelletDamage = 0.1f;
    }

    internal void AdjustShotgunRange(float magnitude, EffectStrengthType effectStrengthType)
    {
        if (effectStrengthType == EffectStrengthType.Additive)
            maxRange += magnitude;
        else if (effectStrengthType == EffectStrengthType.Multiplicative)
            maxRange *= magnitude;

        if (maxRange < 0.1f)
            maxRange = 0.1f;
    }

    internal void AdjustSpread(float magnitude, EffectStrengthType effectStrengthType)
    {
        if (effectStrengthType == EffectStrengthType.Additive)
        {
            spreadAngleX += magnitude;
            spreadAngleY += magnitude;
        }
        else if (effectStrengthType == EffectStrengthType.Multiplicative)
        {
            spreadAngleX *= magnitude;
            spreadAngleY *= magnitude;
        }

        if (spreadAngleX < 0.1f) 
            spreadAngleX = 0.1f;
        if (spreadAngleY < 0.1f)
            spreadAngleY = 0.1f;
    }

    public void PrintEffectedStats()
    {
        string levelInfo = "Unknown Level";
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().IsValid())
        {
            levelInfo = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        }

        string statsReport =
            $"Level: {levelInfo}\n" +
            "=== Player Stats Affected by Effects ===\n" +
            $"Health: {health}/{maxHealth}\n" +
            $"Reload Time: {reloadTime}\n" +
            $"Max Ammo: {maxAmmo}\n" +
            $"Max Bhop Multiplier: {maxBhopMultiplier}\n" +
            $"Pellet Count: {pelletCount}\n" +
            $"Pellet Damage: {pelletDamage}\n" +
            $"Spread Angle X: {spreadAngleX}\n" +
            $"Spread Angle Y: {spreadAngleY}\n" +
            $"Max Range: {maxRange}\n" +
            "========================================\n\n";

        string filePath = Path.Combine(Application.persistentDataPath, "effects.txt");
        Debug.Log("Writing effects.txt to: " + filePath);
        try
        {
            File.AppendAllText(filePath, statsReport + "\n");
            Debug.Log($"Player stats appended to {filePath}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to write player stats to file: {ex.Message}");
        }
    }
}
