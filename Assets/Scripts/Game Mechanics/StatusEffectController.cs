using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectController : MonoBehaviour
{
    [SerializeField]
    public StatusEffectData ReloadBoost;

    private UIController UIController;

    // List of all possible status effects.
    private List<StatusEffectData> allEffects = new List<StatusEffectData>();

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

        UIController = FindAnyObjectByType<UIController>();
        if (UIController == null)
        {
            Debug.LogError("StatusEffectController requires a UIController GameObject!");
        }

        PopulateAllStatusEffectsList();
    }

    private void PopulateAllStatusEffectsList()
    {
        // Populate the list with all possible status effects.
        foreach (StatusEffectData effect in Resources.LoadAll<StatusEffectData>("Status Effects Data"))
        {
            allEffects.Add(effect);
            //Debug.Log("Loaded effect: " + effect.effectName);
        }
    }

    public void ApplyStatusEffect(StatusEffectData effectData)
    {
        // Add the effect to our active list.
        activeEffects.Add(effectData);

        // Immediately modify stats according to the effect.
        ApplyEffect(effectData);

        UIController.AddStatusEffect(effectData);

        // If the effect is temporary, schedule it for removal.
        if (effectData.duration > 0f)
        {
            StartCoroutine(RemoveEffectAfter(effectData, effectData.duration));
        }
    }

    public void RemoveStatusEffect(StatusEffectData effectData)
    {
        if (activeEffects.Contains(effectData))
        {
            // Reverse the effect's impact on player stats.
            ReverseEffect(effectData);
            activeEffects.Remove(effectData);
            UIController.RemoveStatusEffect(effectData);
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
        Debug.Log("Applying effect: " + effectData.effectName + " with magnitude: " + effectData.magnitude + " and strength type: " + effectData.effectStrengthType);

        switch (effectData.effectType)
        {
            // PowerUps
            case EffectType.HopFury:
                playerStats.AdjustHopSpeed(effectData.magnitude, effectData.effectStrengthType);
                break;
            case EffectType.ReloadBoost:
                playerStats.AdjustReloadTime(effectData.magnitude, effectData.effectStrengthType);
                break;
            case EffectType.ShotgunOvercharge:
                playerStats.AdjustShotgunDamage(effectData.magnitude, effectData.effectStrengthType);
                break;
            case EffectType.PelletIncrease:
                playerStats.AdjustPelletCount((int)effectData.magnitude, effectData.effectStrengthType);
                break;
            case EffectType.ExtraClip:
                playerStats.AdjustClipSize((int)effectData.magnitude, effectData.effectStrengthType);
                break;
            case EffectType.SpreadPlus:
                playerStats.AdjustSpread(effectData.magnitude, effectData.effectStrengthType);
                break;

            // Debuffs
            case EffectType.HeavyReload:
                playerStats.AdjustReloadTime(effectData.magnitude, effectData.effectStrengthType);
                break;
            case EffectType.LessDamage:
                playerStats.AdjustShotgunDamage(effectData.magnitude, effectData.effectStrengthType);
                break;
            case EffectType.PelletDecrease:
                playerStats.AdjustPelletCount((int)effectData.magnitude, effectData.effectStrengthType);
                break;
            case EffectType.SpreadMinus:
                playerStats.AdjustSpread(effectData.magnitude, effectData.effectStrengthType);
                break;
            case EffectType.RangeDown:
                playerStats.AdjustShotgunRange(effectData.magnitude, effectData.effectStrengthType);
                break;
            case EffectType.Clipless:
                playerStats.AdjustClipSize(effectData.magnitude, effectData.effectStrengthType);
                break;
        }
    }

    // Reverses the effect applied.
    private void ReverseEffect(StatusEffectData effectData)
    {
        float magnitude = effectData.magnitude;

        if (effectData.effectStrengthType == EffectStrengthType.Multiplicative)
        {
            magnitude = 1 / effectData.magnitude;
        }

        Debug.Log($"Reversing effect: {effectData.effectName} with magnitude: {magnitude} and strength type: {effectData.effectStrengthType}");

        switch (effectData.effectType)
        {
            // PowerUps - reverse by subtracting what was added (or vice versa).
            case EffectType.HopFury:
                playerStats.AdjustHopSpeed(magnitude, effectData.effectStrengthType);
                break;
            case EffectType.ReloadBoost:
                playerStats.AdjustReloadTime(magnitude, effectData.effectStrengthType);
                break;
            case EffectType.ShotgunOvercharge:
                playerStats.AdjustShotgunDamage(magnitude, effectData.effectStrengthType);
                break;
            case EffectType.PelletIncrease:
                playerStats.AdjustPelletCount(magnitude, effectData.effectStrengthType);
                break;
            case EffectType.ExtraClip:
                playerStats.AdjustClipSize(magnitude, effectData.effectStrengthType);
                break;
            case EffectType.SpreadPlus:
                playerStats.AdjustSpread(magnitude, effectData.effectStrengthType);
                break;

            // Debuffs - reverse changes in the opposite direction.
            case EffectType.HeavyReload:
                playerStats.AdjustReloadTime(effectData.magnitude, effectData.effectStrengthType);
                break;
            case EffectType.LessDamage:
                playerStats.AdjustShotgunDamage(effectData.magnitude, effectData.effectStrengthType);
                break;
            case EffectType.PelletDecrease:
                playerStats.AdjustPelletCount(effectData.magnitude, effectData.effectStrengthType);
                break;
            case EffectType.SpreadMinus:
                playerStats.AdjustSpread(effectData.magnitude, effectData.effectStrengthType);
                break;
            case EffectType.RangeDown:
                playerStats.AdjustShotgunRange(effectData.magnitude, effectData.effectStrengthType);
                break;
            case EffectType.Clipless:
                playerStats.AdjustClipSize(effectData.magnitude, effectData.effectStrengthType);
                break;
        }
    }
    
    public void ApplyRandomStatusEffect()
    {
        // Filter out effects that are already active
        List<StatusEffectData> availableEffects = allEffects.FindAll(effect => !activeEffects.Contains(effect));

        if (availableEffects.Count == 0)
        {
            Debug.LogWarning("No available status effects to apply.");
            return;
        }

        // Get a random effect from the available ones
        StatusEffectData randomEffect = availableEffects[UnityEngine.Random.Range(0, availableEffects.Count)];

        // Apply the random effect
        ApplyStatusEffect(randomEffect);
    }

    public List<StatusEffectData> GetActiveEffects()
    {
        return new List<StatusEffectData>(activeEffects);
    }

    public void ApplyEffectsByList(List<StatusEffectData> effects)
    {
        foreach (var effect in effects)
        {
            ApplyStatusEffect(effect);
        }
    }
}
