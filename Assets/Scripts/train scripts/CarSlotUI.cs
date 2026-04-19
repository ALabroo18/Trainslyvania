using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CarSlotUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image slotImage;
    public TextMeshProUGUI carTypeText;
    public TextMeshProUGUI turretsText;

    [Header("Sprites")]
    public Sprite emptySprite;
    public Sprite passengerSprite;
    public Sprite defensiveSprite;

    private int slotIndex;
    private TrainShopUI shopUI;

    public void Refresh(int index, TrainShopUI shop)
    {
        slotIndex = index;
        shopUI = shop;

        CarData car = TrainManager.Instance.GetCar(index);
        bool isEmpty = car == null || car.carType == CarType.None;
        bool isDefault = index == 0 || index == 1;

        if (slotImage != null)
        {
            if (isEmpty)
            {
                if (emptySprite != null) slotImage.sprite = emptySprite;
            }
            else if (car.carType == CarType.Passenger)
            {
                if (passengerSprite != null) slotImage.sprite = passengerSprite;
            }
            else if (car.carType == CarType.Defensive)
            {
                if (defensiveSprite != null) slotImage.sprite = defensiveSprite;
            }
        }

        if (carTypeText != null)
            carTypeText.text = isEmpty ? "Empty" : car.carType.ToString();

        if (turretsText != null)
        {
            if (!isEmpty && car.carType == CarType.Defensive)
            {
                string left = car.leftTurret ? "[T]" : "[_]";
                string right = car.rightTurret ? "[T]" : "[_]";
                turretsText.text = left + " " + right;
                turretsText.gameObject.SetActive(true);
            }
            else
            {
                turretsText.gameObject.SetActive(false);
            }
        }
    }

    public void OnSlotClicked()
    {
        if (shopUI == null)
            shopUI = FindFirstObjectByType<TrainShopUI>();

        CarData car = TrainManager.Instance.GetCar(slotIndex);

        bool isEmpty = car == null || car.carType == CarType.None;

        if (isEmpty)
            shopUI.OpenAddCarMenu(slotIndex);
        else if (car.carType == CarType.Defensive)
            shopUI.OpenTurretMenu(slotIndex);
    }
}