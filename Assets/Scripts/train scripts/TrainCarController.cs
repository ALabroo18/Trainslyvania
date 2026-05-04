using UnityEngine;

public class TrainCarController : MonoBehaviour
{
    [Header("Turret Prefabs")]
    public GameObject mediumTurretPrefab;
    public GameObject catapultTurretPrefab;

    [Header("Turret Spawn Points")]
    public Transform leftTurretSpawnPoint;
    public Transform rightTurretSpawnPoint;

    private GameObject spawnedLeftTurret;
    private GameObject spawnedRightTurret;

    public trainHealth carHealth;
    public int trainIndex;

    void Start()
    {
        if (carHealth != null)
            carHealth.OnBreached += HandleBreach;
    }

    void OnDestroy()
    {
        if (carHealth != null)
            carHealth.OnBreached -= HandleBreach;
    }

    void HandleBreach()
    {
        CarData car = TrainManager.Instance.GetCar(trainIndex);
        if (car != null && car.carType == CarType.Passenger)
        {
            TrainManager.Instance.RemoveMultiplier(trainIndex);
        }
    }

    public void SetTurrets(bool hasLeft, bool hasRight, TurretType leftType = TurretType.Medium, TurretType rightType = TurretType.Medium)
    {
        //left turret
        if (hasLeft && spawnedLeftTurret == null)
        {
            GameObject prefab = leftType == TurretType.Catapult ? catapultTurretPrefab : mediumTurretPrefab;
            if (prefab != null && leftTurretSpawnPoint != null)
            {
                spawnedLeftTurret = Instantiate(prefab, leftTurretSpawnPoint.position, leftTurretSpawnPoint.rotation);
                mediumTurret t = spawnedLeftTurret.GetComponent<mediumTurret>();
                if (t != null) t.owningCar = carHealth;
            }
        }
        else if (!hasLeft && spawnedLeftTurret != null)
        {
            Destroy(spawnedLeftTurret);
            spawnedLeftTurret = null;
        }

        //right turret
        if (hasRight && spawnedRightTurret == null)
        {
            GameObject prefab = rightType == TurretType.Catapult ? catapultTurretPrefab : mediumTurretPrefab;
            if (prefab != null && rightTurretSpawnPoint != null)
            {
                spawnedRightTurret = Instantiate(prefab, rightTurretSpawnPoint.position, rightTurretSpawnPoint.rotation);
                mediumTurret t = spawnedRightTurret.GetComponent<mediumTurret>();
                if (t != null) t.owningCar = carHealth;
            }
        }
        else if (!hasRight && spawnedRightTurret != null)
        {
            Destroy(spawnedRightTurret);
            spawnedRightTurret = null;
        }
    }

    public void RefreshTurrets()
    {
        CarData car = TrainManager.Instance.GetCar(trainIndex);
        if (car == null) return;
        SetTurrets(car.leftTurret, car.rightTurret, car.leftTurretType, car.rightTurretType);
    }
}