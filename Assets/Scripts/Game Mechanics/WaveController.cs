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
    private int currentLevel = 0; // Current level index (1-based for display purposes)

    // Configurable delays (could also be stored in LevelData)
    [SerializeField] private float timeBeforeWavesStart = 3f;
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private float spawnDelay = 1f; // Delay between spawning individual enemies

    // Reference to UIController for updating any UI elements
    private UIController UIController;

    // Flag to determine if the boss is defeated (set this from your boss enemy when it dies)
    private bool isBossDefeated = false;

    private bool checkpointTriggered = false;


    void Start()
    {
        // Locate and assign UIController (if not assigned via Inspector)
        UIController = FindAnyObjectByType<UIController>();
        if (UIController == null)
        {
            Debug.LogError("UIController not found in the scene.");
        }

        // Retrieve the level data from the GameManager singleton
        levelData = GameManager.instance.GetCurrentLevelData();
        currentLevel = GameManager.instance.GetCurrentLevelIndex()+1;

        // Begin the wave system
        StartCoroutine(RunWaveSystem());
    }

    IEnumerator RunWaveSystem()
    {
        // Wait before starting the waves (for instance, to allow players to get ready)
        yield return new WaitForSeconds(timeBeforeWavesStart);

        // For each wave as defined in LevelData
        for (int wave = 1; wave <= levelData.waveCount; wave++)
        {
            currentWaveNumber = wave;

            // Calculate the enemy count for this wave.
            // For example: baseWaveEnemyCount + (wave - 1) * enemyPerWaveIncrementMult
            int enemyCountForThisWave = levelData.baseWaveEnemyCount + (wave - 1) * levelData.enemyPerWaveIncrementMult;

            // Reset counters for the new wave
            enemiesSpawnedInCurrentWave = 0;
            enemiesDefeatedInCurrentWave = 0;

            Debug.Log($"Starting Wave {wave} with {enemyCountForThisWave} enemies.");

            // Spawn enemies one by one until you reach the required count for this wave
            while (enemiesSpawnedInCurrentWave < enemyCountForThisWave)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(spawnDelay);
            }

            // Wait until all the enemies spawned in this wave have been defeated
            yield return new WaitUntil(() => enemiesDefeatedInCurrentWave >= enemyCountForThisWave);
            Debug.Log($"Wave {wave} completed!");

            // If there are additional waves, wait for a bit before starting the next one
            if (wave < levelData.waveCount)
            {
                yield return new WaitForSeconds(timeBetweenWaves);
            }
        }

        Debug.Log("All waves done. Waiting on checkpoint...");

        // All waves are complete�now spawn the boss if it's assigned
        if (levelData.bossPrefab != null)
        {
            // Level 1 specific mechanic - open the 2nd floor for boss fight
            if (currentLevel == 1)
            {
                // Call a specific mechanic for this wave
                Level1SpecificMechanic();
            }

            yield return new WaitUntil(() => checkpointTriggered);
            Debug.Log("Checkpoint reached! Spawning boss.");
            SpawnBoss();


            // Wait until the boss is defeated.
            yield return new WaitUntil(() => isBossDefeated);
        }

        // Wave system for this level has ended.
        Debug.Log("All waves and boss completed for this level!");
        EndWaveSystem();
        yield return null;
    }

    private void Level1SpecificMechanic()
    {
        //get gameobject called "NextStage2"
        Transform nextStageTransform = GameObject.Find("NextStage2").transform;
        //get a collider called "NextStageWallCollider"
        Collider nextStageCollider = GameObject.Find("NextStageWallCollider").GetComponent<Collider>();

        //get a collider called "Floor1TriggerCollider"
        Collider floor1TriggerCollider = GameObject.Find("Floor1TriggerCollider").GetComponent<Collider>();

        //rotate the nextStageTransform to -90 degrees on the z axis
        nextStageTransform.rotation = Quaternion.Euler(0, 0, -90);

        //set the nextStageCollider off
        nextStageCollider.enabled = false;

        Debug.Log("Opened the 2nd floor!");
    }

    void SpawnEnemy()
    {
        // Example: Find the ground object (assumed to be the same across levels) by name.
        // Here it searches for a GameObject called "floor0_ground".
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

        // Instantiate the enemy. Optionally, you could apply the enemy stat multipliers here,
        // e.g., by passing them to an enemy component on the spawned prefab.
        GameObject enemy = Instantiate(levelData.enemyPrefab, spawnPosition, Quaternion.identity);

        //Debug.Log("Spawned enemy at position: " + randomPosition);
        // Example of applying multipliers if your enemy script supports it.
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            //enemyScript.ApplyMultipliers(levelData.enemyHealthMult, levelData.enemyDamageMult, levelData.enemySpeedMult);
        }

        enemiesSpawnedInCurrentWave++;
        totalEnemiesSpawned++;

        UIController.UpdateEnemiesSpawned(totalEnemiesSpawned);
        //Debug.Log("Spawned enemy at position: " + spawnPosition);
    }

    void SpawnBoss()
    {
        // Find the same ground object to pick a spawn area
        GameObject groundObject = GameObject.Find("floor1_ground");
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

        Vector3 spawnPosition = GetRandomPositionInCollider(spawnAreaCollider);
        // Adjust boss spawn position if necessary
        spawnPosition.y += 0.9f;

        GameObject boss = Instantiate(levelData.bossPrefab, spawnPosition, Quaternion.identity);

        enemiesSpawnedInCurrentWave++;
        totalEnemiesSpawned++;

        UIController.UpdateEnemiesSpawned(totalEnemiesSpawned);
        Debug.Log("Spawned Boss at position: " + spawnPosition);
    }

    Vector3 GetRandomPositionInCollider(Collider collider)
    {
        float margin = 2f;  // Adjust as needed to ensure enemies spawn safely away from edges.

        // Ensure we don't sample too close to the edges by offsetting the bounds.
        Vector3 boundsMin = collider.bounds.min;
        Vector3 boundsMax = collider.bounds.max;

        // Clamp the random range within the shrunk bounds.
        float randomX = Random.Range(boundsMin.x + margin, boundsMax.x - margin);
        float randomZ = Random.Range(boundsMin.z + margin, boundsMax.z - margin);

        // Spawn slightly above the ground (you might adjust the Y offset as needed).
        float randomY = boundsMax.y + 2f;

        return new Vector3(randomX, randomY, randomZ);
    }


    void EndWaveSystem()
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

        UIController.UpdateEnemiesSpawned(totalEnemiesSpawned);

        //Debug.Log("Spawned Boss at position: " + randomPosition);
        // Place any logic here that should run when the wave system ends,
        // for example, progressing to the next level or displaying a victory screen.
        Debug.Log("Wave system complete for this level!");
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
        enemiesDefeatedInCurrentWave++;
        totalEnemiesDefeated++;
        UIController.UpdateEnemiesDefeated(totalEnemiesDefeated);
    }

    public void OnBossDefeated()
    {
        isBossDefeated = true;

        // Stop the timer
        timerStopped = true;
        StartCoroutine(LoadEndScene());
    }

    public void OnGameOver()
    {
        // Stop the timer
        timerStopped = true;
        StartCoroutine(LoadEndScene());
    }

    IEnumerator LoadEndScene()
    {
        yield return new WaitForSeconds(3f);
        GameManager.instance.LoadScene("PrototipoEndScene");
    }

    public void OnCheckpointReached()
    {
        checkpointTriggered = true;
    }

}
