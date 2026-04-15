using UnityEngine;

public class TrainLevelSpawner : MonoBehaviour
{
    [Header("Car Prefabs")]
    public GameObject passengerCarPrefab;
    public GameObject defensiveCarPrefab;

    [Header("Rotation")]
    public Vector3 carRotation = new Vector3(-90f, 180f, 0f);

    [Header("Train Head")]
    public Transform trainHead;

    [HideInInspector] public trainHealth[] spawnedTrainHealths;

    void Start()
    {
        SpawnTrain();
    }

    void SpawnTrain()
    {
        if (TrainManager.Instance == null)
        {
            return;
        }

        int totalCars = 0;
        for (int i = 0; i < TrainManager.MAX_CARS; i++)
        {
            CarData car = TrainManager.Instance.GetCar(i);
            if (car != null && car.carType != CarType.None) totalCars++;
        }

        spawnedTrainHealths = new trainHealth[totalCars];
        GameObject[] spawnedCars = new GameObject[totalCars];

        int spawnedCount = 0;

        for (int i = 0; i < TrainManager.MAX_CARS; i++)
        {
            CarData car = TrainManager.Instance.GetCar(i);
            if (car == null || car.carType == CarType.None) continue;

            bool isPassenger = car.carType == CarType.Passenger;
            GameObject prefab = isPassenger ? passengerCarPrefab : defensiveCarPrefab;

            if (prefab == null)
            {
                continue;
            }

            GameObject spawnedCar = Instantiate(prefab, Vector3.zero, Quaternion.Euler(carRotation));
            spawnedCars[spawnedCount] = spawnedCar;

            Transform snapFront = spawnedCar.transform.Find("SnapFront");

            Transform snapBack = spawnedCar.transform.Find("SnapFront");

            if (snapBack == null)
            {
            }
            else
            {
                Transform snapTarget = null;

                if (spawnedCount == 0)
                {
                    snapTarget = trainHead.Find("SnapBack");
                    if (snapTarget == null)
                        snapTarget = trainHead;
                }
                else
                {
                    snapTarget = spawnedCars[spawnedCount - 1].transform.Find("SnapBack");
                }

                if (snapTarget != null)
                {
                    Vector3 offset = spawnedCar.transform.position - snapBack.position;
                    spawnedCar.transform.position = snapTarget.position + offset;
                }
            }

            trainHealth health = spawnedCar.GetComponentInChildren<trainHealth>();
            if (health != null)
                spawnedTrainHealths[spawnedCount] = health;

            TrainCarController controller = spawnedCar.GetComponent<TrainCarController>();
            if (controller == null)
                controller = spawnedCar.GetComponentInChildren<TrainCarController>();
            if (controller != null)
            {
                controller.trainIndex = i;
                controller.carHealth = health;
                controller.SetTurrets(car.leftTurret, car.rightTurret);
            }

            spawnedCount++;
        }

        RouteWinCondition winCondition = FindObjectOfType<RouteWinCondition>();
        if (winCondition != null)
            winCondition.SetTrains(spawnedTrainHealths);

        TrainUIManager uiManager = FindObjectOfType<TrainUIManager>();
        if (uiManager != null)
            uiManager.SetTrains(spawnedTrainHealths);
    }
}