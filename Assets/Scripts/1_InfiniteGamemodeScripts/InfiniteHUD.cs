using UnityEngine;
using TMPro;

public class InfiniteHUD : MonoBehaviour
{
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI enemyCountText;
    public TextMeshProUGUI totalTrainHealthtext;

    void Start()
    {
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.OnWaveStarted += OnWaveStarted;
            WaveManager.Instance.OnEnemyCountChanged += OnEnemyCountChanged;
        }
        else
            Debug.LogWarning("WaveManager.Instance is null in InfiniteHUD.Start!");

        if (waveText != null)
            waveText.text = "Wave: 0";
        if (enemyCountText != null)
            enemyCountText.text = "Enemies: 0";

        trainHealth[] allHealth = FindObjectsByType<trainHealth>(FindObjectsSortMode.None);
        foreach (trainHealth h in allHealth)
            h.OnHealthChanged += (current, max) => UpdateTrainHP();

        UpdateTrainHP();
    }

    void UpdateTrainHP()
    {
        trainHealth[] allHealth = FindObjectsByType<trainHealth>(FindObjectsSortMode.None);
        int currentHP = 0;
        int totalHP = 0;
        foreach (trainHealth h in allHealth)
        {
            currentHP += h.currentHealth;
            totalHP += h.maxHealth;
        }
        if (totalTrainHealthtext != null)
            totalTrainHealthtext.text = "Train HP: " + currentHP + "/" + totalHP;
    }

        void OnDestroy()
    {
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.OnWaveStarted -= OnWaveStarted;
            WaveManager.Instance.OnEnemyCountChanged -= OnEnemyCountChanged;
        }
    }

    void OnWaveStarted(int wave)
    {
        if (waveText != null)
            waveText.text = "Wave: " + wave;

        UpdateTrainHP();
    }

    void OnEnemyCountChanged(int count)
    {
        if (enemyCountText != null)
            enemyCountText.text = "Enemies: " + count;
    }
}