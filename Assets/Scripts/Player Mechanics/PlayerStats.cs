using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField, Tooltip("Vida do player")]
    public float health = 100f;
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
    public int pelletCount = 30;
    [SerializeField, Tooltip("Ângulo máximo do spread em X")]
    public float spreadAngleX = 50f;
    [SerializeField, Tooltip("Ângulo máximo do spread em Y")]
    public float spreadAngleY = 25f;
    [SerializeField, Tooltip("Range máximo de cada bala do tiro")]
    public float maxRange = 15f;

    [Header("Fire Rate Settings")]
    [SerializeField, Tooltip("Tempo de delay entre cada tiro")]
    public float fireRate = 0.3f; // Customizable time between shots

    [Header("Ammo and Reload Settings")]
    [SerializeField, Tooltip("Max Ammo da AxeGun")]
    public int maxAmmo = 12;
    [SerializeField, Tooltip("AxeGun Reload Time")]
    public float reloadTime = 2f;

    internal void AdjustReloadTime(float magnitude, EffectStrengthType effectStrengthType)
    {
        if(effectStrengthType == EffectStrengthType.Additive)
            reloadTime += magnitude;
        else if (effectStrengthType == EffectStrengthType.Multiplicative)
            reloadTime *= magnitude;

        if (reloadTime < 0.1f)
            reloadTime = 0.1f;
    }

    internal void AddShield(float magnitude)
    {
        throw new NotImplementedException();
    }

    internal void AdjustClipSize(int magnitude)
    {
        throw new NotImplementedException();
    }

    internal void AdjustHopSpeed(float magnitude)
    {
        throw new NotImplementedException();
    }

    internal void AdjustHopWindow(float magnitude)
    {
        throw new NotImplementedException();
    }

    internal void AdjustMovementSpeed(float v)
    {
        throw new NotImplementedException();
    }

    internal void AdjustPelletCount(int magnitude)
    {
        throw new NotImplementedException();
    }

    internal void AdjustShotgunDamage(float magnitude)
    {
        throw new NotImplementedException();
    }

    internal void AdjustShotgunRange(float v)
    {
        throw new NotImplementedException();
    }

    internal void AdjustSpread(float magnitude)
    {
        throw new NotImplementedException();
    }

    internal void RemoveShield(float magnitude)
    {
        throw new NotImplementedException();
    }

    internal void ResetShotgunJamChance()
    {
        throw new NotImplementedException();
    }

    internal void SetRicochetPellets(bool v)
    {
        throw new NotImplementedException();
    }

    internal void SetShotgunJamChance(float magnitude)
    {
        throw new NotImplementedException();
    }

    internal void StartHealthDrain(float magnitude)
    {
        throw new NotImplementedException();
    }

    internal void StartHealthRegen(float magnitude)
    {
        throw new NotImplementedException();
    }

    internal void StopHealthDrain()
    {
        throw new NotImplementedException();
    }

    internal void StopHealthRegen()
    {
        throw new NotImplementedException();
    }
}
