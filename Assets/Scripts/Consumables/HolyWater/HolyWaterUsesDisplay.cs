using UnityEngine;
using TMPro;

public class HolyWaterUsesDisplay : MonoBehaviour
{
    public TextMeshProUGUI usesText;

    void OnEnable()
    {
        if (HolyWaterManager.Instance != null)
            HolyWaterManager.Instance.OnUsesChanged += UpdateDisplay;
        UpdateDisplay(HolyWaterManager.Instance.Uses);
    }

    void OnDisable()
    {
        if (HolyWaterManager.Instance != null)
            HolyWaterManager.Instance.OnUsesChanged -= UpdateDisplay;
    }

    void UpdateDisplay(int amount)
    {
        if (usesText != null)
            usesText.text = "Holy Water: " + amount + "/" + HolyWaterManager.Instance.maxUses;
    }
}