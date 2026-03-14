using UnityEngine;

public class RouteWinCondition : MonoBehaviour
{
    public float routeTime = 60f;
    public int reward = 100;
    public GameObject winScreenUI;
    public GameObject loseScreenUI;

    public vampireSpawn spawner;
    public trainHealth[] trains;

    private float timer;
    private bool routeCompleted;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = routeTime;
        if (winScreenUI != null)
            winScreenUI.SetActive(false);
        if (loseScreenUI != null)
            loseScreenUI.SetActive(false);

        foreach (trainHealth train in trains)
        {
            if (train != null)
                train.OnBreached += HandleBreach;
        }    
    }

    void OnDestroy()
    {
        foreach (trainHealth train in trains)
        {
            if (train != null)
                train.OnBreached -= HandleBreach;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (routeCompleted)
            return;

        timer -= Time.deltaTime;

        bool timeUp = timer <= 0;
        bool allSpawned = spawner != null && spawner.HasReachedMaxSpawns();

        if (timeUp && allSpawned)
        {
            CompleteRoute();
        }
    }

    void CompleteRoute()
    {
        routeCompleted = true;

        CurrencyManager.Instance.AddCurrency(reward);

        Debug.Log("Route completed! Earned" + reward);

        if (winScreenUI != null)
            winScreenUI.SetActive(true);

        Time.timeScale = 0f;
    }

    void HandleBreach()
    {
        if (routeCompleted) return;

        foreach (trainHealth train in trains)
        {
            if (train != null && !train.isBreached)
                return;
        }    

        routeCompleted = true;

        if (loseScreenUI != null)
            loseScreenUI.SetActive(true);
        Time.timeScale = 0f;
    }
}
