using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    public float health;
    public List<StatusEffectData> activeEffects = new List<StatusEffectData>();
}
