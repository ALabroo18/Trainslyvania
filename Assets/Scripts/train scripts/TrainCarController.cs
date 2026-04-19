using UnityEngine;

public class TrainCarController : MonoBehaviour
{
    [Header("Pre-placed turrets")]
    public GameObject leftTurret;
    public GameObject rightTurret;

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

    public void SetTurrets(bool hasLeft, bool hasRight)
    {
        if (leftTurret != null)
        {
            leftTurret.SetActive(hasLeft);
        }

        if (rightTurret != null)
        {
            rightTurret.SetActive(hasRight);
        }
    }

    public void RefreshTurrets()
    {
        CarData car = TrainManager.Instance.GetCar(trainIndex);
        if (car == null) return;
        SetTurrets(car.leftTurret, car.rightTurret);
    }
}