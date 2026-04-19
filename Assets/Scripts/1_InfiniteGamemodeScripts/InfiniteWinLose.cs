using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class InfiniteWinLose : MonoBehaviour
{
    public GameObject loseScreenUI;
    public TextMeshProUGUI finalWaveText;
    public TextMeshProUGUI finalCurrencyText;

    private List<trainHealth> trains = new List<trainHealth>();

    public void RegisterTrain(trainHealth train)
    {
        if (train == null || trains.Contains(train)) return;
        trains.Add(train);
        train.OnBreached += CheckAllBreached;
        Debug.Log("Train registered, total: " + trains.Count);
    }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (loseScreenUI != null) loseScreenUI.SetActive(false);
        trainHealth[] prePlaced = FindObjectsByType<trainHealth>(FindObjectsSortMode.None);
        foreach (trainHealth t in prePlaced)
            RegisterTrain(t);
    }

    void OnDestroy()
    {
        foreach (trainHealth train in trains)
            if (train != null)
                train.OnBreached -= CheckAllBreached;
    }

    void CheckAllBreached()
    {
        foreach (trainHealth train in trains)
            if (train != null && !train.isBreached) return;

        EndRun();
    }

    void EndRun()
    {
        WaveManager.Instance.EndRun();

        if (finalWaveText != null)
            finalWaveText.text = "You survived " + WaveManager.Instance.CurrentWave + " waves!";
        if (finalCurrencyText != null)
            finalCurrencyText.text = "Earned: " + InfiniteRunCurrency.Instance.Currency;

        if (loseScreenUI != null) loseScreenUI.SetActive(true);
        Time.timeScale = 0f;
    }
}