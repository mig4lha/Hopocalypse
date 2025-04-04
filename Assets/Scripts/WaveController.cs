using System.Collections;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    private LevelData levelData;

    // Variables to track the wave state
    private int enemiesSpawned = 0;
    private int enemiesDefeated = 0;

    void Start()
    {
        // Retrieve the current level data from the GameManager
        levelData = GameManager.instance.GetCurrentLevelData();
        StartCoroutine(RunWaveSystem());
    }

    IEnumerator RunWaveSystem()
    {
        float elapsedTime = 0f;

        // Spawn regular enemies until we've either spawned the desired amount or hit the boss time limit
        while (enemiesSpawned < levelData.enemyCount && elapsedTime < levelData.timeUntilBoss)
        {
            SpawnEnemy();
            enemiesSpawned++;
            yield return new WaitForSeconds(1f);  // Adjust spawn delay as needed
            elapsedTime += 1f;
        }

        // After regular spawns, check for the boss prefab and spawn the boss
        if (levelData.bossPrefab != null)
        {
            SpawnBoss();
        }

        // Optionally, add logic here to wait until all enemies are defeated before ending the level
        yield return null;
    }

    //void SpawnEnemy()
    //{
    //    // Implement your enemy spawn logic (e.g., choose a random spawn point, instantiate the enemy prefab, etc.)
    //    Debug.Log("Spawned enemy #" + enemiesSpawned);
    //    Instantiate(levelData.enemyPrefab, new Vector3(0,1,2), Quaternion.identity);
    //}

    void SpawnEnemy()
    {
        // Determine the current level number, for example, from your GameManager
        int currentLevel = GameManager.instance.GetCurrentLevelIndex()+1;

        GameObject groundObject = null;

        // Construct the parent GameObject's name
        string parentName = "Level" + currentLevel;

        // Find the parent GameObject
        GameObject parentObject = GameObject.Find(parentName);

        if (parentObject != null)
        {
            // Find the "Ground" child within the parent
            Transform groundTransform = parentObject.transform.Find("Ground");

            if (groundTransform != null)
            {
                groundObject = groundTransform.gameObject;
                // Proceed with using groundObject as needed
            }
            else
            {
                Debug.LogError("Ground GameObject not found under " + parentName);
            }
        }
        else
        {
            Debug.LogError("Parent GameObject " + parentName + " not found.");
        }

        if(groundObject == null)
        {
            Debug.LogError("Ground object is null. Cannot spawn enemy.");
            return;
        }

        // Reference to the GameObject serving as the spawn area
        GameObject spawnArea = groundObject;

        // Get the Collider component from the spawn area
        Collider spawnAreaCollider = spawnArea.GetComponent<Collider>();

        if (spawnAreaCollider == null)
        {
            Debug.LogError("Spawn area does not have a Collider component.");
            return;
        }

        // Calculate random position within the Collider's bounds
        Vector3 randomPosition = GetRandomPositionInCollider(spawnAreaCollider);

        // Instantiate the enemy at the calculated position
        Instantiate(levelData.enemyPrefab, randomPosition, Quaternion.identity);

        Debug.Log("Spawned enemy at position: " + randomPosition);
    }

    Vector3 GetRandomPositionInCollider(Collider collider)
    {
        Vector3 boundsMin = collider.bounds.min;
        Vector3 boundsMax = collider.bounds.max;

        float randomX = Random.Range(boundsMin.x, boundsMax.x);
        float randomY = Random.Range(1, 1);
        float randomZ = Random.Range(boundsMin.z, boundsMax.z);

        return new Vector3(randomX, randomY, randomZ);
    }

    void SpawnBoss()
    {
        // Implement your boss spawn logic. For example, instantiate the boss prefab at a designated location:
        Debug.Log("Boss spawned!");
        Instantiate(levelData.bossPrefab, new Vector3(0, 1, 2), Quaternion.identity);
    }

    // This method should be called from enemy scripts when an enemy is defeated
    public void OnEnemyDefeated()
    {
        enemiesDefeated++;
        // You can add logic here to check if the wave or level is complete
    }
}
