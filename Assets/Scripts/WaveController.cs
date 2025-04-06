using System.Collections;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    private LevelData levelData;

    private int enemiesSpawned = 0;
    private int enemiesDefeated = 0;

    void Start()
    {
        levelData = GameManager.instance.GetCurrentLevelData(); 
        StartCoroutine(RunWaveSystem());
    }

    IEnumerator RunWaveSystem()
    {
        float elapsedTime = 0f;
        while (enemiesSpawned < levelData.enemyCount && elapsedTime < levelData.timeUntilBoss){
            SpawnEnemy();
            enemiesSpawned++;
            yield return new WaitForSeconds(1f);
            elapsedTime += 1f;
        }
        if (levelData.bossPrefab != null){
            SpawnBoss();
        }
        yield return null;
    }

    void SpawnEnemy()
    {
        int currentLevel = GameManager.instance.GetCurrentLevelIndex()+1;

        // Debug apenas para level1
        GameObject groundObject = GameObject.Find("floor0_ground");

        if (groundObject == null){
            Debug.LogError("Ground object is null. Cannot spawn enemy.");
            return;
        }

        GameObject spawnArea = groundObject;
        Collider spawnAreaCollider = spawnArea.GetComponent<Collider>();

        if (spawnAreaCollider == null)
        {
            Debug.LogError("Spawn area does not have a Collider component.");
            return;
        }

        Vector3 randomPosition = GetRandomPositionInCollider(spawnAreaCollider);
        Instantiate(levelData.enemyPrefab, randomPosition, Quaternion.identity);

        Debug.Log("Spawned enemy at position: " + randomPosition);
    }

    Vector3 GetRandomPositionInCollider(Collider collider){
        Vector3 boundsMin = collider.bounds.min;
        Vector3 boundsMax = collider.bounds.max;
        float randomX = Random.Range(boundsMin.x, boundsMax.x);
        float randomY = Random.Range(boundsMin.y+2, boundsMax.y+2);
        float randomZ = Random.Range(boundsMin.z, boundsMax.z);
        return new Vector3(randomX, randomY, randomZ);
    }

    void SpawnBoss()
    {
        Debug.Log("Boss spawned!");
        Instantiate(levelData.bossPrefab, new Vector3(0, 1, 2), Quaternion.identity);
    }

    public void OnEnemyDefeated(){
        enemiesDefeated++;
    }
}
