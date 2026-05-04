using UnityEngine;
using System;

[System.Serializable]
public class CarData
{
    public CarType carType;
    public bool leftTurret;
    public bool rightTurret;
    public TurretType leftTurretType;
    public TurretType rightTurretType;
}

public enum CarType
{
    None,
    Passenger,
    Defensive
}

public enum TurretType
{
    Medium,
    Catapult
}

public class TrainManager : MonoBehaviour
{
    public static TrainManager Instance;

    public const int MAX_CARS = 7;


    [SerializeField] private int freeTurretCharges = 2;
    public int FreeTurretCharges => freeTurretCharges;
    [SerializeField] private int passengerCarInventory = 0;
    [SerializeField] private int defensiveCarInventory = 0;

    [SerializeField] private CarData[] cars = new CarData[MAX_CARS];

    private bool[] carBreached = new bool[MAX_CARS];

    public event Action OnTrainChanged;

    [ContextMenu("Clear All PlayerPrefs")]
    public void ClearAllPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("All PlayerPrefs cleared");
    }

    public int TotalTurretSlots
    {
        get
        {
            int slots = 0;
            foreach (CarData car in cars)
                if (car != null && car.carType == CarType.Defensive)
                    slots += 2;
            return slots;
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnValidate()
    {
        passengerCarInventory = Mathf.Max(0, passengerCarInventory);
        defensiveCarInventory = Mathf.Max(0, defensiveCarInventory);
    }

    public int PlacedTurrets
    {
        get
        {
            int count = 0;
            foreach (CarData car in cars)
            {
                if (car != null)
                {
                    if (car.leftTurret) count++;
                    if (car.rightTurret) count++;
                }
            }
            return count;
        }
    }

    public int AvailableTurretSlots => freeTurretCharges;

    public float MoneyMultiplier
    {
        get
        {
            float multiplier = 1f;
            for (int i = 0; i < MAX_CARS; i++)
                if (cars[i] != null && cars[i].carType == CarType.Passenger && !carBreached[i])
                    multiplier += 0.25f;
            return multiplier;
        }
    }

    public int PassengerCarInventory => passengerCarInventory;
    public int DefensiveCarInventory => defensiveCarInventory;

    public int PlacedCarCount
    {
        get
        {
            int count = 0;
            foreach (CarData car in cars)
                if (car != null && car.carType != CarType.None) count++;
            return count;
        }
    }

    public CarData GetCar(int index)
    {
        if (index < 0 || index >= MAX_CARS) return null;
        return cars[index];
    }

    public void AddToInventory(CarType type)
    {
        if (type == CarType.Passenger) passengerCarInventory++;
        else if (type == CarType.Defensive) defensiveCarInventory++;
        Save();
        OnTrainChanged?.Invoke();
    }

    public bool PlaceCar(CarType type)
    {
        if (type == CarType.Passenger && passengerCarInventory <= 0)
        {
            return false;
        }
        if (type == CarType.Defensive && defensiveCarInventory <= 0)
        {
            return false;
        }
        if (PlacedCarCount >= MAX_CARS)
        {
            return false;
        }

        for (int i = 0; i < MAX_CARS; i++)
        {
            if (cars[i] == null || cars[i].carType == CarType.None)
            {
                cars[i] = new CarData { carType = type, leftTurret = false, rightTurret = false };
                if (type == CarType.Passenger) passengerCarInventory--;
                else if (type == CarType.Defensive) defensiveCarInventory--;
                Save();
                OnTrainChanged?.Invoke();
                return true;
            }
        }
        return false;
    }

    public bool RemoveCar(int index)
    {
        if (index < 0 || index >= MAX_CARS) return false;
        if (cars[index] == null || cars[index].carType == CarType.None) return false;

        if (cars[index].carType == CarType.Passenger) passengerCarInventory++;
        else if (cars[index].carType == CarType.Defensive) defensiveCarInventory++;

        cars[index] = null;
        carBreached[index] = false;
        Save();
        OnTrainChanged?.Invoke();
        return true;
    }

    public bool PlaceTurret(int carIndex, TurretType turretType = TurretType.Medium)
    {
        if (carIndex < 0 || carIndex >= MAX_CARS)
        {
            return false;
        }
        if (cars[carIndex] == null || cars[carIndex].carType != CarType.Defensive)
        {
            return false;
        }
        if (freeTurretCharges <= 0)
        {
            return false;
        }

        if (!cars[carIndex].leftTurret)
        {
            cars[carIndex].leftTurret = true;
            cars[carIndex].leftTurretType = turretType;
        }
        else if (!cars[carIndex].rightTurret)
        {
            cars[carIndex].rightTurret = true;
            cars[carIndex].rightTurretType = turretType;
        }
        else
        {
            return false;
        }

        freeTurretCharges--;
        Save();
        OnTrainChanged?.Invoke();
        return true;
    }

    public bool RemoveTurret(int carIndex)
    {
        if (cars[carIndex] == null) return false;

        if (cars[carIndex].rightTurret)
            cars[carIndex].rightTurret = false;
        else if (cars[carIndex].leftTurret)
            cars[carIndex].leftTurret = false;
        else
            return false;

        freeTurretCharges++;
        Save();
        OnTrainChanged?.Invoke();
        return true;
    }

    public void AddTurretCharge(int amount = 1)
    {
        freeTurretCharges += amount;
        Save();
        OnTrainChanged?.Invoke();
    }

    public void RemoveMultiplier(int carIndex)
    {
        carBreached[carIndex] = true;
        OnTrainChanged?.Invoke();
    }

    [ContextMenu("Reset To Default")]
    public void ResetToDefault()
    {
        for (int i = 0; i < MAX_CARS; i++)
        {
            cars[i] = null;
            carBreached[i] = false;
        }
        cars[0] = new CarData { carType = CarType.Passenger, leftTurret = false, rightTurret = false };
        cars[1] = new CarData { carType = CarType.Defensive, leftTurret = false, rightTurret = false };
        passengerCarInventory = 0;
        defensiveCarInventory = 0;
        freeTurretCharges = 2;
        PlayerPrefs.DeleteKey("TrainDefaultsSet");
        Save();
        OnTrainChanged?.Invoke();
    }

    void SetupDefaultCars()
    {
        if (PlayerPrefs.GetInt("TrainDefaultsSet", 0) == 1) return;

        cars[0] = new CarData { carType = CarType.Passenger, leftTurret = false, rightTurret = false };
        cars[1] = new CarData { carType = CarType.Defensive, leftTurret = false, rightTurret = false };
        freeTurretCharges = 2;

        PlayerPrefs.SetInt("TrainDefaultsSet", 1);
        Save();
    }

    void Save()
    {
        PlayerPrefs.SetInt("FreeTurretCharges", freeTurretCharges);
        PlayerPrefs.SetInt("PassengerInventory", passengerCarInventory);
        PlayerPrefs.SetInt("DefensiveInventory", defensiveCarInventory);
        for (int i = 0; i < MAX_CARS; i++)
        {
            if (cars[i] == null)
            {
                PlayerPrefs.SetInt("Car_" + i + "_Type", (int)CarType.None);
                PlayerPrefs.SetInt("Car_" + i + "_Left", 0);
                PlayerPrefs.SetInt("Car_" + i + "_Right", 0);
                PlayerPrefs.SetInt("Car_" + i + "_LeftType", 0);
                PlayerPrefs.SetInt("Car_" + i + "_RightType", 0);
            }
            else
            {
                PlayerPrefs.SetInt("Car_" + i + "_Type", (int)cars[i].carType);
                PlayerPrefs.SetInt("Car_" + i + "_Left", cars[i].leftTurret ? 1 : 0);
                PlayerPrefs.SetInt("Car_" + i + "_Right", cars[i].rightTurret ? 1 : 0);
                PlayerPrefs.SetInt("Car_" + i + "_LeftType", (int)cars[i].leftTurretType);
                PlayerPrefs.SetInt("Car_" + i + "_RightType", (int)cars[i].rightTurretType);
            }
        }
        PlayerPrefs.Save();
    }

    void Load()
    {
        freeTurretCharges = PlayerPrefs.GetInt("FreeTurretCharges", 0);
        passengerCarInventory = PlayerPrefs.GetInt("PassengerInventory", 0);
        defensiveCarInventory = PlayerPrefs.GetInt("DefensiveInventory", 0);
        for (int i = 0; i < MAX_CARS; i++)
        {
            CarType type = (CarType)PlayerPrefs.GetInt("Car_" + i + "_Type", (int)CarType.None);
            if (type == CarType.None)
                cars[i] = null;
            else
                cars[i] = new CarData
                {
                    carType = type,
                    leftTurret = PlayerPrefs.GetInt("Car_" + i + "_Left", 0) == 1,
                    rightTurret = PlayerPrefs.GetInt("Car_" + i + "_Right", 0) == 1,
                    leftTurretType = (TurretType)PlayerPrefs.GetInt("Car_" + i + "_LeftType", 0),
                    rightTurretType = (TurretType)PlayerPrefs.GetInt("Car_" + i + "_RightType", 0)
                };
        }
        SetupDefaultCars();
    }

    public bool PlaceCarAtSlot(int index, CarType type)
    {
        if (index < 0 || index >= MAX_CARS) return false;
        if (cars[index] != null && cars[index].carType != CarType.None)
        {
            Debug.Log("Slot " + index + " already occupied!");
            return false;
        }
        if (type == CarType.Passenger && passengerCarInventory <= 0) return false;
        if (type == CarType.Defensive && defensiveCarInventory <= 0) return false;

        cars[index] = new CarData { carType = type, leftTurret = false, rightTurret = false };
        if (type == CarType.Passenger) passengerCarInventory--;
        else if (type == CarType.Defensive) defensiveCarInventory--;

        Save();
        OnTrainChanged?.Invoke();
        return true;
    }
}