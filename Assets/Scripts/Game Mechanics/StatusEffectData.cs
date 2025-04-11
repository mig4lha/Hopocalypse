using UnityEngine;

public enum EffectStrengthType
{
    Additive,            // Add to the base value
    Multiplicative,      // Multiply the base value by this amount
    None                 // No effect
}

public enum EffectType
{
    // PowerUps
    HopFury,             // Boost hop speed multiplier
    ReloadBoost,         // Reduced reload time
    ShotgunOvercharge,   // Increase shotgun damage
    PelletIncrease,      // Extra pellets per shot
    ExtraClip,           // Increase number of shotgun shots before reload
    SpreadPlus,          // Increase spread by a set amount
    Shield,              // Temporary energy shield (absorbs damage)
    HopWindowUp,         // Extend maximum hop window (fewer jumps needed)
    HealthRegen,         // Regenerate health continuously over time
    RicochetPellets,     // Pellets bounce off surfaces

    // Debuffs
    MovementDown,        // Reduce player movement speed
    HeavyReload,         // Increase reload time
    LessDamage,          // Lower shotgun pellet damage
    ShotgunJam,          // Chance for the shotgun to jam (not fire)
    PelletDecrease,      // Fewer pellets per shot
    SpreadMinus,         // Decrease spread by a set amount
    RangeDown,           // Reduce range of the shotgun shots
    HopWindowDown,       // Shorten maximum hop window (more frequent jumps)
    BloodLoss,           // Player loses health over time
    Clipless             // Fewer bullets before reloading
}

[CreateAssetMenu(fileName = "NewStatusEffect", menuName = "Status Effects/Effect Data")]
public class StatusEffectData : ScriptableObject
{

    [Header("Basic Info")]
    [Tooltip("A friendly name for the effect (for example, 'Hop Fury' or 'Movement Down').")]
    public string effectName;

    [Tooltip("The type of effect. Determines what mechanic is affected.")]
    public EffectType effectType;

    [Tooltip("Duration in seconds. If 0 or negative, the effect is permanent or requires manual removal.")]
    public float duration;

    [Header("Effect Strength")]
    [Tooltip("The magnitude of the effect. For a boost, use a positive value (e.g. +1.5 to increase hop speed). For a debuff that reduces a value, you might use a multiplier below 1 (e.g. 0.8 to reduce reload speed by 20%) or a negative addition.")]
    public float magnitude;

    [Header("Effect Strenght Type")]
    [Tooltip("The type of magnitude of the effect. Is it additive or multipicative?")]
    public EffectStrengthType effectStrengthType;

    [Header("Optional UI Info")]
    [Tooltip("A description of what the effect does (shown in UI or tooltips).")]
    [TextArea]
    public string description;

    [Tooltip("An icon representing this effect.")]
    public Sprite icon;
}
