using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectController : MonoBehaviour
{
    // List of active effects.
    private List<StatusEffectData> activeEffects = new List<StatusEffectData>();

    // Assume our player has a PlayerStats component that handles the actual stat modifications.
    private PlayerStats playerStats;

    private void Awake()
    {
        playerStats = FindAnyObjectByType<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogError("StatusEffectController requires a PlayerStats GameObject!");
        }
    }

    /// <summary>
    /// Applies an effect to the player.
    /// </summary>
    public void ApplyStatusEffect(StatusEffectData effectData)
    {
        // Add the effect to our active list.
        activeEffects.Add(effectData);

        // Immediately modify stats according to the effect.
        ApplyEffect(effectData);

        // If the effect is temporary, schedule it for removal.
        if (effectData.duration > 0f)
        {
            StartCoroutine(RemoveEffectAfter(effectData, effectData.duration));
        }
    }

    /// <summary>
    /// Removes an active effect from the player.
    /// </summary>
    public void RemoveStatusEffect(StatusEffectData effectData)
    {
        if (activeEffects.Contains(effectData))
        {
            // Reverse the effect's impact on player stats.
            ReverseEffect(effectData);
            activeEffects.Remove(effectData);
        }
    }

    // Coroutine: wait for a delay and then remove the effect.
    private IEnumerator RemoveEffectAfter(StatusEffectData effectData, float delay)
    {
        yield return new WaitForSeconds(delay);
        RemoveStatusEffect(effectData);
    }

    // Applies the effect based on its type.
    private void ApplyEffect(StatusEffectData effectData)
    {
        switch (effectData.effectType)
        {
            // PowerUps
            case EffectType.HopFury:
                playerStats.AdjustHopSpeed(effectData.magnitude);
                break;
            case EffectType.ReloadBoost:
                // Assuming a reduced reload time means subtracting time.
                playerStats.AdjustReloadTime(-effectData.magnitude);
                break;
            case EffectType.ShotgunOvercharge:
                playerStats.AdjustShotgunDamage(effectData.magnitude);
                break;
            case EffectType.PelletIncrease:
                playerStats.AdjustPelletCount((int)effectData.magnitude);
                break;
            case EffectType.ExtraClip:
                playerStats.AdjustClipSize((int)effectData.magnitude);
                break;
            case EffectType.SpreadPlus:
                playerStats.AdjustSpread(effectData.magnitude);
                break;
            case EffectType.Shield:
                playerStats.AddShield(effectData.magnitude);
                break;
            case EffectType.HopWindowUp:
                playerStats.AdjustHopWindow(effectData.magnitude);
                break;
            case EffectType.HealthRegen:
                playerStats.StartHealthRegen(effectData.magnitude);
                break;
            case EffectType.RicochetPellets:
                playerStats.SetRicochetPellets(true);
                break;

            // Debuffs
            case EffectType.MovementDown:
                playerStats.AdjustMovementSpeed(-effectData.magnitude);
                break;
            case EffectType.HeavyReload:
                playerStats.AdjustReloadTime(effectData.magnitude);
                break;
            case EffectType.LessDamage:
                playerStats.AdjustShotgunDamage(-effectData.magnitude);
                break;
            case EffectType.ShotgunJam:
                // Here magnitude represents the chance (e.g., 0.2 for 20% chance to jam)
                playerStats.SetShotgunJamChance(effectData.magnitude);
                break;
            case EffectType.PelletDecrease:
                playerStats.AdjustPelletCount(-(int)effectData.magnitude);
                break;
            case EffectType.SpreadMinus:
                playerStats.AdjustSpread(-effectData.magnitude);
                break;
            case EffectType.RangeDown:
                playerStats.AdjustShotgunRange(-effectData.magnitude);
                break;
            case EffectType.HopWindowDown:
                playerStats.AdjustHopWindow(-effectData.magnitude);
                break;
            case EffectType.BloodLoss:
                playerStats.StartHealthDrain(effectData.magnitude);
                break;
            case EffectType.Clipless:
                playerStats.AdjustClipSize(-(int)effectData.magnitude);
                break;
        }
    }

    // Reverses the effect applied.
    private void ReverseEffect(StatusEffectData effectData)
    {
        switch (effectData.effectType)
        {
            // PowerUps - reverse by subtracting what was added (or vice versa).
            case EffectType.HopFury:
                playerStats.AdjustHopSpeed(-effectData.magnitude);
                break;
            case EffectType.ReloadBoost:
                playerStats.AdjustReloadTime(effectData.magnitude);
                break;
            case EffectType.ShotgunOvercharge:
                playerStats.AdjustShotgunDamage(-effectData.magnitude);
                break;
            case EffectType.PelletIncrease:
                playerStats.AdjustPelletCount(-(int)effectData.magnitude);
                break;
            case EffectType.ExtraClip:
                playerStats.AdjustClipSize(-(int)effectData.magnitude);
                break;
            case EffectType.SpreadPlus:
                playerStats.AdjustSpread(-effectData.magnitude);
                break;
            case EffectType.Shield:
                playerStats.RemoveShield(effectData.magnitude);
                break;
            case EffectType.HopWindowUp:
                playerStats.AdjustHopWindow(-effectData.magnitude);
                break;
            case EffectType.HealthRegen:
                playerStats.StopHealthRegen();
                break;
            case EffectType.RicochetPellets:
                playerStats.SetRicochetPellets(false);
                break;

            // Debuffs - reverse changes in the opposite direction.
            case EffectType.MovementDown:
                playerStats.AdjustMovementSpeed(effectData.magnitude);
                break;
            case EffectType.HeavyReload:
                playerStats.AdjustReloadTime(-effectData.magnitude);
                break;
            case EffectType.LessDamage:
                playerStats.AdjustShotgunDamage(effectData.magnitude);
                break;
            case EffectType.ShotgunJam:
                playerStats.ResetShotgunJamChance();
                break;
            case EffectType.PelletDecrease:
                playerStats.AdjustPelletCount((int)effectData.magnitude);
                break;
            case EffectType.SpreadMinus:
                playerStats.AdjustSpread(effectData.magnitude);
                break;
            case EffectType.RangeDown:
                playerStats.AdjustShotgunRange(effectData.magnitude);
                break;
            case EffectType.HopWindowDown:
                playerStats.AdjustHopWindow(effectData.magnitude);
                break;
            case EffectType.BloodLoss:
                playerStats.StopHealthDrain();
                break;
            case EffectType.Clipless:
                playerStats.AdjustClipSize((int)effectData.magnitude);
                break;
        }
    }
}
