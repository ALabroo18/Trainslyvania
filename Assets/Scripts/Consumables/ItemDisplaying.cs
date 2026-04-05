using UnityEngine;
using TMPro;

public class ItemDisplaying : MonoBehaviour
{
    [Header("Holy Water")]
    public TextMeshProUGUI holyWaterText;

    [Header("Fireball")]
    public TextMeshProUGUI fireballText;

    void OnEnable()
    {
        if (ItemManager.Instance != null)
        {
            ItemManager.Instance.OnHolyWaterChanged += UpdateHolyWater;
            ItemManager.Instance.OnFireballChanged += UpdateFireball;
        }
        UpdateHolyWater(ItemManager.Instance.HolyWaterUses);
        UpdateFireball(ItemManager.Instance.FireballUses);
    }

    void OnDisable()
    {
        if (ItemManager.Instance != null)
        {
            ItemManager.Instance.OnHolyWaterChanged -= UpdateHolyWater;
            ItemManager.Instance.OnFireballChanged -= UpdateFireball;
        }
    }

    void UpdateHolyWater(int amount)
    {
        if (holyWaterText != null)
            holyWaterText.text = amount.ToString();
    }

    void UpdateFireball(int amount)
    {
        if (fireballText != null)
            fireballText.text = amount.ToString();
    }
}
