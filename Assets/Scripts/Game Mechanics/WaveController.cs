using System.Collections;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    private LevelData levelData;

    private int enemiesSpawned = 0;
    private int enemiesDefeated = 0;
    private float timePassed = 0;
    private bool timerStopped = false;

    private UIController UIController;

    void Start()
    {
        UIController = FindAnyObjectByType<UIController>();
        if (UIController == null)
        {
            Debug.LogError("UIController not found in the scene.");
        }

        levelData = GameManager.instance.GetCurrentLevelData();
        StartCoroutine(RunWaveSystem());
    }

    IEnumerator RunWaveSystem()
    {
        while (enemiesDefeated < levelData.enemyCount && timePassed < levelData.timeUntilBoss)
        {
            SpawnEnemy();
            enemiesSpawned++;
            yield return new WaitForSeconds(1f);
        }
        if (levelData.bossPrefab != null)
        {
            enemiesSpawned++;
            SpawnBoss();
        }
        yield return null;
    }

    void SpawnEnemy()
    {
        int currentLevel = GameManager.instance.GetCurrentLevelIndex() + 1;

        // Debug apenas para level1
        GameObject groundObject = GameObject.Find("floor0_ground");

        if (groundObject == null)
        {
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

        UIController.UpdateEnemiesSpawned(enemiesSpawned);

        //Debug.Log("Spawned enemy at position: " + randomPosition);
    }

    Vector3 GetRandomPositionInCollider(Collider collider)
    {
        Vector3 boundsMin = collider.bounds.min;
        Vector3 boundsMax = collider.bounds.max;
        float randomX = Random.Range(boundsMin.x, boundsMax.x);
        float randomY = boundsMax.y + 2; // Spawn above the ground
        float randomZ = Random.Range(boundsMin.z, boundsMax.z);
        return new Vector3(randomX, randomY, randomZ);
    }

    void SpawnBoss()
    {
        int currentLevel = GameManager.instance.GetCurrentLevelIndex() + 1;

        // Debug apenas para level1
        GameObject groundObject = GameObject.Find("floor0_ground");

        if (groundObject == null)
        {
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
        randomPosition.y += 0.9f;
        Instantiate(levelData.bossPrefab, randomPosition, Quaternion.identity);

        UIController.UpdateEnemiesSpawned(enemiesSpawned);

        //Debug.Log("Spawned Boss at position: " + randomPosition);
    }

    private void Update()
    {
        if (timerStopped == false)
        {
            //update time passed
            timePassed += Time.deltaTime;
            UIController.UpdateTimePassed(timePassed);
        }
    }

    public void OnEnemyDefeated()
    {
        enemiesDefeated++;
        UIController.UpdateEnemiesDefeated(enemiesDefeated);

        // check if enemiesdefeated == enemiesSpawned and stop timer
        if (enemiesDefeated >= enemiesSpawned)
        {
            timerStopped = true;
            Debug.Log("All enemies defeated. Stopping timer.");

            // after 5 seconds, load scene "PrototipoEndScene"
            StartCoroutine(LoadEndScene());
        }
    }

    IEnumerator LoadEndScene()
    {
        yield return new WaitForSeconds(5f);
        GameManager.instance.LoadScene("PrototipoEndScene");
    }
}
