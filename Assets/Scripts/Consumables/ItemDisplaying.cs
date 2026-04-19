using UnityEngine;
using TMPro;

public class ItemDisplaying : MonoBehaviour
{
    [Header("Holy Water")]
    public TextMeshProUGUI holyWaterText;

    [Header("Firebomb")]
    public TextMeshProUGUI firebombText;

    void Start()
    {
        if (ModeSelector.SelectedMode == GameMode.Infinite)
        {
            if (InfiniteConsumableManager.Instance != null)
            {
                InfiniteConsumableManager.Instance.OnChargesChanged += UpdateInfiniteDisplay;
                UpdateInfiniteDisplay(InfiniteConsumableManager.Instance != null ? InfiniteConsumableManager.Instance.HolyWaterCharges : 0, InfiniteConsumableManager.Instance != null ? InfiniteConsumableManager.Instance.FirebombCharges : 0);
            }
        }
        else
        {
            if (ItemManager.Instance != null)
            {
                ItemManager.Instance.OnHolyWaterChanged += UpdateHolyWater;
                ItemManager.Instance.OnFirebombChanged += UpdateFirebomb;
            }
            UpdateHolyWater(ItemManager.Instance != null ? ItemManager.Instance.HolyWaterUses : 0);
            UpdateFirebomb(ItemManager.Instance != null ? ItemManager.Instance.FirebombUses : 0);
        }
    }

    void OnDestroy()
    {
        if (ModeSelector.SelectedMode == GameMode.Infinite)
        {
            if (InfiniteConsumableManager.Instance != null)
                InfiniteConsumableManager.Instance.OnChargesChanged -= UpdateInfiniteDisplay;
        }
        else
        {
            if (ItemManager.Instance != null)
            {
                ItemManager.Instance.OnHolyWaterChanged -= UpdateHolyWater;
                ItemManager.Instance.OnFirebombChanged -= UpdateFirebomb;
            }
        }
    }

    void UpdateInfiniteDisplay(int holyWater, int firebomb)
    {
        if (holyWaterText != null)
            holyWaterText.text = holyWater.ToString();
        if (firebombText != null)
            firebombText.text = firebomb.ToString();
    }

    void UpdateHolyWater(int amount)
    {
        if (holyWaterText != null)
            holyWaterText.text = amount.ToString();
    }

    void UpdateFirebomb(int amount)
    {
        if (firebombText != null)
            firebombText.text = amount.ToString();
    }
}
