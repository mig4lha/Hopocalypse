using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelData", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    public string levelName;         
    public int enemyCount;          
    public float timeUntilBoss;
    public GameObject enemyPrefab; 
    public GameObject bossPrefab;   
}
