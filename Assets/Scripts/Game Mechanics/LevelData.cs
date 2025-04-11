using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelData", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    public string levelName;        
    public int waveCount;
    public int baseWaveEnemyCount;
    public int enemyPerWaveIncrementMult;
    public float enemyHealthMult;
    public float enemyDamageMult;
    public float enemySpeedMult;
    public GameObject enemyPrefab; 
    public GameObject bossPrefab;   
}
