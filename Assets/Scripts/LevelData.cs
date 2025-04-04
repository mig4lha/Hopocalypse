using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelData", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    public string levelName;         // This can match your scene name or be used for display
    public int enemyCount;           // Total number of enemies to spawn before boss
    public float timeUntilBoss;      // Time until the boss should appear (if using a time-based condition)
    public GameObject enemyPrefab;   // Enemy prefab to spawn for this level
    public GameObject bossPrefab;    // Boss enemy prefab to spawn for this level
    // Add any additional wave settings you need here (e.g., spawn rates, enemy types, etc.)
}
