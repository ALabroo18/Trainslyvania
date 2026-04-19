using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    [Header("Wave Settings")]
    public int currentWave = 0;
    public float timeBetweenWaves = 2f;

    [Header("Enemy Scaling")]
    public GameObject enemyPrefab;
    public int baseEnemyDamage = 10;
    public float enemyDamageScaling = 1.2f;
    public float enemyCountBase = 5f;
    public float enemyCountGrowth = 0.15f;
    public float enemyHPBase = 100f;
    public float enemyHPGrowthRate = 1.12f;

    [Header("Spawn Area")]
    public Transform spawnCenter;
    public Vector2 boxSize = new Vector2(30f, 30f);
    public float spawnHeight = 0f;

    [Header("Currency")]
    public int baseWaveReward = 100;
    public float rewardScaling = 0.3f;

    public event Action<int> OnWaveStarted;
    public event Action<int> OnWaveCompleted;
    public event Action OnRunEnded;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private bool waveInProgress = false;
    private bool runEnded = false;

    public int CurrentWave => currentWave;
    public bool WaveInProgress => waveInProgress;
    public event Action<int> OnEnemyCountChanged;

    public int LastWaveReward { get; private set; }
    private int flatRewardBonus = 0;

    public int FlatRewardBonus => flatRewardBonus;

    private int expectedEnemyCount = 0;
    private int killedEnemyCount = 0;
    private int remainingEnemyCount = 0;
    private bool allSpawned = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        StartCoroutine(StartNextWave());
    }

    public void AddFlatRewardBonus(int amount)
    {
        flatRewardBonus += amount;
    }

    public int GetExpectedEnemyCount(int wave)
    {
        return Mathf.RoundToInt(enemyCountBase * (1f + enemyCountGrowth * wave));
    }

    public int GetExpectedEnemyHP(int wave)
    {
        return Mathf.RoundToInt(enemyHPBase * Mathf.Pow(enemyHPGrowthRate, wave));
    }

    public int GetExpectedEnemyDamage(int wave)
    {
        return Mathf.RoundToInt(baseEnemyDamage * Mathf.Pow(enemyDamageScaling, wave));
    }

    IEnumerator StartNextWave()
    {
        if (waveInProgress)
        {
            Debug.LogWarning("StartNextWave called while wave already in progress!");
            yield break;
        }

        yield return new WaitForSeconds(timeBetweenWaves);

        currentWave++;
        waveInProgress = true;
        killedEnemyCount = 0;
        allSpawned = false;
        activeEnemies.Clear();

        expectedEnemyCount = GetExpectedEnemyCount(currentWave);
        remainingEnemyCount = expectedEnemyCount;
        int enemyHP = GetExpectedEnemyHP(currentWave);
        int enemyDamage = GetExpectedEnemyDamage(currentWave);

        OnWaveStarted?.Invoke(currentWave);
        OnEnemyCountChanged?.Invoke(remainingEnemyCount);

        Debug.Log("Wave " + currentWave + " starting - Enemies: " + expectedEnemyCount + " HP: " + enemyHP);

        for (int i = 0; i < expectedEnemyCount; i++)
        {
            SpawnEnemy(enemyHP, enemyDamage);
            yield return new WaitForSeconds(0.5f);
        }

        allSpawned = true;
        CheckWaveComplete();
    }

    void SpawnEnemy(int hp, int damage)
    {
        Vector3 spawnPos = GetRandomPointOnBoxEdge();
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        InfiniteVampireHealth health = enemy.GetComponent<InfiniteVampireHealth>();
        if (health != null)
        {
            health.SetStats(hp);
            health.OnDied += () => OnEnemyDied(enemy);
        }

        activeEnemies.Add(enemy);
    }

    void OnEnemyDied(GameObject enemy)
    {
        activeEnemies.Remove(enemy);
        killedEnemyCount++;

        int remaining = Mathf.Max(0, expectedEnemyCount - killedEnemyCount);

        Debug.Log("Enemy died, remaining: " + remainingEnemyCount + " killed: " + killedEnemyCount);
        OnEnemyCountChanged?.Invoke(remaining);

        CheckWaveComplete();
    }

    void CheckWaveComplete()
    {
        if (allSpawned && killedEnemyCount >= expectedEnemyCount && waveInProgress)
        {
            waveInProgress = false;
            GiveWaveReward();
            OnWaveCompleted?.Invoke(currentWave);
            Debug.Log("Wave " + currentWave + " complete!");
        }
    }

    void GiveWaveReward()
    {
        float scaleFactor = 1f + (currentWave * 0.15f) + (Mathf.Sqrt(currentWave) * 0.2f);
        float multiplier = InfiniteUpgradeManager.Instance != null
            ? InfiniteUpgradeManager.Instance.TotalMoneyMultiplier
            : 1f;

        LastWaveReward = Mathf.RoundToInt((baseWaveReward * scaleFactor + flatRewardBonus) * multiplier);
        InfiniteRunCurrency.Instance.AddCurrency(LastWaveReward);
        Debug.Log("Wave " + currentWave + " reward: " + LastWaveReward);
    }

    public void StartNextWaveAfterShop()
    {
        if (!runEnded && !waveInProgress)
            StartCoroutine(StartNextWave());
    }

    public void EndRun()
    {
        runEnded = true;
        waveInProgress = false;
        OnRunEnded?.Invoke();
    }

    Vector3 GetRandomPointOnBoxEdge()
    {
        float halfX = boxSize.x / 4f;
        float halfZ = boxSize.y / 4f;
        int side = UnityEngine.Random.Range(0, 4);
        float x = 0f, z = 0f;

        switch (side)
        {
            case 0: x = UnityEngine.Random.Range(-halfX, halfX); z = halfZ; break;
            case 1: x = UnityEngine.Random.Range(-halfX, halfX); z = -halfZ; break;
            case 2: x = -halfX; z = UnityEngine.Random.Range(-halfZ, halfZ); break;
            case 3: x = halfX; z = UnityEngine.Random.Range(-halfZ, halfZ); break;
        }

        return new Vector3(spawnCenter.position.x + x, spawnHeight, spawnCenter.position.z + z);
    }
}