using System.Collections;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    private LevelData levelData;

    // Wave tracking variables
    private int currentWaveNumber = 0;
    private int enemiesSpawnedInCurrentWave = 0;
    private int enemiesDefeatedInCurrentWave = 0;
    private int totalEnemiesDefeated = 0;
    private int totalEnemiesSpawned = 0;
    private float timePassed = 0;
    private bool timerStopped = false;
    private int currentLevel = 0;

    [SerializeField] private float timeBeforeWavesStart = 3f;
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private float spawnDelay = 1f;

    private UIController UIController;
    private SlotMachineController slotMachine;

    private bool isBossDefeated = false;

    private bool checkpointTriggered = false;


    void Start()
    {
        isBossDefeated = false;

        UIController = FindAnyObjectByType<UIController>();
        if (UIController == null)
        {
            Debug.LogError("UIController not found in the scene.");
        }

        slotMachine = FindAnyObjectByType<SlotMachineController>();
        if (slotMachine == null)
        {
            Debug.LogError("SlotMachineController not found in the scene.");
        }

        // Retrieve the level data from the GameManager singleton
        levelData = GameManager.instance.GetCurrentLevelData();
        currentLevel = GameManager.instance.GetCurrentLevelIndex()+1;

        StartCoroutine(RunWaveSystem());
    }

    IEnumerator RunWaveSystem()
    {
        yield return new WaitForSeconds(timeBeforeWavesStart);

        for (int wave = 1; wave <= levelData.waveCount; wave++)
        {
            currentWaveNumber = wave;
            int enemyCountForThisWave = levelData.baseWaveEnemyCount + (wave - 1) * levelData.enemyPerWaveIncrementMult;
            enemiesSpawnedInCurrentWave = 0;
            enemiesDefeatedInCurrentWave = 0;

            Debug.Log($"Starting Wave {wave} with {enemyCountForThisWave} enemies.");

            while (enemiesSpawnedInCurrentWave < enemyCountForThisWave)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(spawnDelay);
            }

            yield return new WaitUntil(() => enemiesDefeatedInCurrentWave >= enemyCountForThisWave);
            Debug.Log($"Wave {wave} completed!");

            if (wave < levelData.waveCount)
            {
                yield return new WaitForSeconds(timeBetweenWaves);
            }
        }

        Debug.Log("All waves done. Waiting on checkpoint...");

        if (levelData.bossPrefab != null)
        {
            // Level 1 specific mechanic - open the 2nd floor for boss fight
            if (currentLevel == 1)
            {
                Level1SpecificMechanic();
                yield return new WaitUntil(() => checkpointTriggered);
                Debug.Log("Checkpoint reached! Spawning boss.");
                SpawnBoss();
            } else if (currentLevel == 2)
            {
                SpawnBoss();
            }

            Debug.Log("Waiting for boss defeat...");

            yield return new WaitUntil(() => isBossDefeated);
        }

        Debug.Log("Wave system complete for this level!");
        yield return null;
    }

    private void Level1SpecificMechanic()
    {
        Transform nextStageTransform = GameObject.Find("NextStage2").transform;
        Collider nextStageCollider = GameObject.Find("NextStageWallCollider").GetComponent<Collider>();
        Collider floor1TriggerCollider = GameObject.Find("Floor1TriggerCollider").GetComponent<Collider>();
        nextStageTransform.rotation = Quaternion.Euler(0, 0, -90);
        nextStageCollider.enabled = false;
        Debug.Log("Opened the 2nd floor!");
    }

    void SpawnEnemy()
    {
        GameObject groundObject = GameObject.Find("floor0_ground");
        if (groundObject == null)
        {
            Debug.LogError("Ground object not found. Cannot spawn enemy.");
            return;
        }

        Collider spawnAreaCollider = groundObject.GetComponent<Collider>();
        if (spawnAreaCollider == null)
        {
            Debug.LogError("Spawn area does not have a Collider component.");
            return;
        }

        Vector3 spawnPosition = GetRandomPositionInCollider(spawnAreaCollider);

        GameObject enemy = Instantiate(levelData.enemyPrefab, spawnPosition, Quaternion.identity);

        enemiesSpawnedInCurrentWave++;
        totalEnemiesSpawned++;

        UIController.UpdateEnemiesSpawned(totalEnemiesSpawned);
        //Debug.Log("Spawned enemy at position: " + spawnPosition);
    }

    void SpawnBoss()
    {
        GameObject groundObject = null;
        if (currentLevel == 1)
        {
            groundObject = GameObject.Find("floor1_ground");
        }
        else if (currentLevel == 2)
        {
            groundObject = GameObject.Find("floor0_ground");
        }

        if (groundObject == null)
        {
            Debug.LogError("Ground object not found. Cannot spawn boss.");
            return;
        }

        Collider spawnAreaCollider = groundObject.GetComponent<Collider>();
        if (spawnAreaCollider == null)
        {
            Debug.LogError("Spawn area does not have a Collider component.");
            return;
        }

        Vector3 spawnPosition = new Vector3(0,0,0);
        if (currentLevel == 1)
        {
            spawnPosition = new Vector3(0, 3, 0);
        } else if (currentLevel == 2)
        {
            spawnPosition = new Vector3(0, 10, 0);
        }

        GameObject boss = Instantiate(levelData.bossPrefab, spawnPosition, Quaternion.identity);

        enemiesSpawnedInCurrentWave++;
        totalEnemiesSpawned++;

        UIController.UpdateEnemiesSpawned(totalEnemiesSpawned);
        Debug.Log("Spawned Boss at position: " + spawnPosition);

        MusicManager.instance?.PlayBossMusic();
    }

    Vector3 GetRandomPositionInCollider(Collider collider)
    {
        float margin = 4f;

        Vector3 boundsMin = collider.bounds.min;
        Vector3 boundsMax = collider.bounds.max;
        float randomX = Random.Range(boundsMin.x + margin, boundsMax.x - margin);
        float randomZ = Random.Range(boundsMin.z + margin, boundsMax.z - margin);

        float randomY = boundsMax.y + 2f;

        return new Vector3(randomX, randomY, randomZ);
    }

    private void Update()
    {
        if (timerStopped == false)
        {
            timePassed += Time.deltaTime;
            UIController.UpdateTimePassed(timePassed);
        }
    }

    public void OnEnemyDefeated()
    {
        enemiesDefeatedInCurrentWave++;
        totalEnemiesDefeated++;
        UIController.UpdateEnemiesDefeated(totalEnemiesDefeated);
    }

    public void OnBossDefeated()
    {
        MusicManager.instance?.FadeOutMusic();

        Debug.Log("Boss defeated!");
        isBossDefeated = true;

        timerStopped = true;

        slotMachine.Drop();
        Debug.Log("Dropping slot machine");

        //StartCoroutine(LoadEndScene());
    }

    public void OnGameOver()
    {
        timerStopped = true;
        StartCoroutine(LoadGameOverScene());
    }

    public IEnumerator LoadEndScene()
    {
        yield return new WaitForSeconds(5f);
        GameManager.instance.LoadScene("EndScene");
    }

    public IEnumerator LoadGameOverScene()
    {
        yield return new WaitForSeconds(3f);
        GameManager.instance.LoadScene("GameOver");
    }

    public IEnumerator LoadNextLevel()
    {
        GameManager.instance.IncrementLevelIndex();
        int newCurrentLevelIndex = GameManager.instance.GetCurrentLevelIndex();

        GameManager.instance.SavePlayerState();

        if (newCurrentLevelIndex >= GameManager.instance.levels.Count)
        {
            Debug.Log("No more levels to load.");
            StartCoroutine(LoadEndScene());
            yield break;
        } else
        {
            yield return new WaitForSeconds(5f);
            GameManager.instance.LoadLevel(newCurrentLevelIndex);
        }
    }

    public void OnCheckpointReached()
    {
        checkpointTriggered = true;
    }

}
