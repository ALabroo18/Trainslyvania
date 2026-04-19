using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TrainShopUI : MonoBehaviour
{
    [Header("Prices")]
    public int passengerCarPrice = 200;
    public int defensiveCarPrice = 300;
    public int turretPrice = 100;

    [Header("Stats Display")]
    public TextMeshProUGUI turretSlotsText;
    public TextMeshProUGUI moneyMultiplierText;
    public TextMeshProUGUI inventoryText;

    [Header("Car Slots")]
    public CarSlotUI[] carSlots;

    [Header("Add Car Menu")]
    public GameObject addCarMenuPanel;
    public TextMeshProUGUI passengerCarPriceText;
    public TextMeshProUGUI defensiveCarPriceText;
    public TextMeshProUGUI passengerInventoryText;
    public TextMeshProUGUI defensiveInventoryText;
    private int selectedSlotIndex = -1;

    [Header("Turret Menu")]
    public GameObject turretMenuPanel;
    public TextMeshProUGUI turretSlotStatusText;
    private int selectedCarIndex = -1;

    void OnEnable()
    {
        TrainManager.Instance.OnTrainChanged += RefreshUI;
        RefreshUI();
    }

    void OnDisable()
    {
        TrainManager.Instance.OnTrainChanged -= RefreshUI;
    }

    void Start()
    {
        if (addCarMenuPanel != null) addCarMenuPanel.SetActive(false);
        if (turretMenuPanel != null) turretMenuPanel.SetActive(false);
    }

    void RefreshUI()
    {
        if (turretSlotsText != null)
            turretSlotsText.text = "Turrets to place: " + TrainManager.Instance.AvailableTurretSlots
                + " | Total slots: " + TrainManager.Instance.TotalTurretSlots
                + " | Placed: " + TrainManager.Instance.PlacedTurrets;

        if (moneyMultiplierText != null)
            moneyMultiplierText.text = "Money Bonus: x" + TrainManager.Instance.MoneyMultiplier.ToString("F2");

        if (inventoryText != null)
            inventoryText.text = "Passenger: " + TrainManager.Instance.PassengerCarInventory + " | Defensive: " + TrainManager.Instance.DefensiveCarInventory;

        for (int i = 0; i < carSlots.Length; i++)
            if (carSlots[i] != null)
                carSlots[i].Refresh(i, this);

        //refresh turret menu status if open
        if (turretMenuPanel != null && turretMenuPanel.activeSelf && selectedCarIndex >= 0)
        {
            CarData car = TrainManager.Instance.GetCar(selectedCarIndex);
            if (turretSlotStatusText != null && car != null)
            {
                string left = car.leftTurret ? "[T]" : "[_]";
                string right = car.rightTurret ? "[T]" : "[_]";
                turretSlotStatusText.text = "Slots: " + left + " " + right;
            }
        }

        //refresh add car menu inventory counts if open
        if (addCarMenuPanel != null && addCarMenuPanel.activeSelf)
        {
            if (passengerInventoryText != null)
                passengerInventoryText.text = "Owned: " + TrainManager.Instance.PassengerCarInventory;
            if (defensiveInventoryText != null)
                defensiveInventoryText.text = "Owned: " + TrainManager.Instance.DefensiveCarInventory;
        }
    }

    public void OpenAddCarMenu(int slotIndex)
    {
        selectedSlotIndex = slotIndex;

        if (passengerCarPriceText != null)
            passengerCarPriceText.text = "$" + passengerCarPrice;
        if (defensiveCarPriceText != null)
            defensiveCarPriceText.text = "$" + defensiveCarPrice;
        if (passengerInventoryText != null)
            passengerInventoryText.text = "Owned: " + TrainManager.Instance.PassengerCarInventory;
        if (defensiveInventoryText != null)
            defensiveInventoryText.text = "Owned: " + TrainManager.Instance.DefensiveCarInventory;

        if (addCarMenuPanel != null)
            addCarMenuPanel.SetActive(true);
        if (turretMenuPanel != null)
            turretMenuPanel.SetActive(false);
    }

    public void CloseAddCarMenu()
    {
        selectedSlotIndex = -1;
        if (addCarMenuPanel != null)
            addCarMenuPanel.SetActive(false);
    }

    //buy and immediately place into selected slot
    public void BuyAndPlacePassenger()
    {
        if (selectedSlotIndex < 0) return;

        //check inventory first, buy if none
        if (TrainManager.Instance.PassengerCarInventory <= 0)
        {
            if (!CurrencyManager.Instance.SubCurrency(passengerCarPrice))
            {
                return;
            }
            TrainManager.Instance.AddToInventory(CarType.Passenger);
        }

        TrainManager.Instance.PlaceCarAtSlot(selectedSlotIndex, CarType.Passenger);
        CloseAddCarMenu();
    }

    public void BuyAndPlaceDefensive()
    {
        if (selectedSlotIndex < 0) return;

        if (TrainManager.Instance.DefensiveCarInventory <= 0)
        {
            if (!CurrencyManager.Instance.SubCurrency(defensiveCarPrice))
            {
                return;
            }
            TrainManager.Instance.AddToInventory(CarType.Defensive);
        }

        TrainManager.Instance.PlaceCarAtSlot(selectedSlotIndex, CarType.Defensive);
        CloseAddCarMenu();
    }

    public void OpenTurretMenu(int carIndex)
    {
        selectedCarIndex = carIndex;
        if (turretMenuPanel != null)
            turretMenuPanel.SetActive(true);
        if (addCarMenuPanel != null)
            addCarMenuPanel.SetActive(false);

        CarData car = TrainManager.Instance.GetCar(carIndex);
        if (turretSlotStatusText != null && car != null)
        {
            string left = car.leftTurret ? "[T]" : "[_]";
            string right = car.rightTurret ? "[T]" : "[_]";
            turretSlotStatusText.text = "Slots: " + left + " " + right;
        }
    }

    public void CloseTurretMenu()
    {
        selectedCarIndex = -1;
        if (turretMenuPanel != null)
            turretMenuPanel.SetActive(false);
    }

    public void BuyAndPlaceTurret()
    {
        if (selectedCarIndex < 0)
        {
            return; // if car doesnt exist leave this function
        }

        if (TrainManager.Instance.FreeTurretCharges > 0)
        {
            if (!TrainManager.Instance.PlaceTurret(selectedCarIndex))
                return;
        }    

        if (!CurrencyManager.Instance.SubCurrency(turretPrice))
        {
            Debug.Log("You're broke.");
            return; // if broke leave this function
            
        }

        TrainManager.Instance.AddTurretCharge(1);
        if (!TrainManager.Instance.PlaceTurret(selectedCarIndex))
        {
            TrainManager.Instance.AddTurretCharge(-1);
            CurrencyManager.Instance.AddCurrency(turretPrice);
        }
    }

    //buttons to add
    public void RemoveTurret()
    {
        if (selectedCarIndex < 0) return;
        TrainManager.Instance.RemoveTurret(selectedCarIndex);
    }

    public void RemoveLastCar()
    {
        for (int i = TrainManager.MAX_CARS - 1; i >= 2; i--)
        {
            CarData car = TrainManager.Instance.GetCar(i);
            if (car != null && car.carType != CarType.None)
            {
                TrainManager.Instance.RemoveCar(i);
                return;
            }
        }
    }

    public void RemoveAllCars()
    {
        for (int i = 2; i < TrainManager.MAX_CARS; i++)
        {
            CarData car = TrainManager.Instance.GetCar(i);
            if (car != null && car.carType != CarType.None)
                TrainManager.Instance.RemoveCar(i);
        }
    }
}