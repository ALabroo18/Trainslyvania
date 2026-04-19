using UnityEngine;

public class InfiniteCarSpawner : MonoBehaviour
{
    public static InfiniteCarSpawner Instance;

    [Header("Prefabs")]
    public GameObject passengerCarPrefab;
    public GameObject defensiveCarPrefab;

    [Header("Fixed Spawn Points")]
    public Transform passengerCarSpawnPoint;
    public Transform defensiveCarSpawnPoint;

    [Header("Stack Offset")]
    public Vector3 stackOffset = new Vector3(0f, 0.1f, 0f);

    private int passengerCarCount = 0;
    private int defensiveCarCount = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    public void SpawnPassengerCar()
    {
        if (passengerCarPrefab == null || passengerCarSpawnPoint == null) return;

        Vector3 pos = passengerCarSpawnPoint.position + stackOffset * passengerCarCount;
        GameObject car = Instantiate(passengerCarPrefab, pos, passengerCarSpawnPoint.rotation);

        //wire up health to TrainUIManager and RouteWinCondition
        trainHealth health = car.GetComponentInChildren<trainHealth>();
        if (health != null)
        {
            InfiniteWinLose winLose = FindFirstObjectByType<InfiniteWinLose>();
            if (winLose != null) winLose.RegisterTrain(health);
        }

        passengerCarCount++;
        Debug.Log("Passenger car spawned, total: " + passengerCarCount);
    }

    public void SpawnDefensiveCar()
    {
        if (defensiveCarPrefab == null || defensiveCarSpawnPoint == null) return;

        Vector3 pos = defensiveCarSpawnPoint.position + stackOffset * defensiveCarCount;
        GameObject car = Instantiate(defensiveCarPrefab, pos, defensiveCarSpawnPoint.rotation);

        TrainCarController controller = car.GetComponent<TrainCarController>();
        if (controller == null)
            controller = car.GetComponentInChildren<TrainCarController>();

        //give it 2 turret slots automatically
        if (controller != null)
            controller.SetTurrets(false, false); //start with no turrets placed

        trainHealth health = car.GetComponentInChildren<trainHealth>();
        if (health != null)
        {
            InfiniteWinLose winLose = FindFirstObjectByType<InfiniteWinLose>();
            if (winLose != null) winLose.RegisterTrain(health);
        }

        defensiveCarCount++;
        Debug.Log("Defensive car spawned, total: " + defensiveCarCount);
    }
}