using UnityEngine;
using TMPro;

public class HolyWaterShopItem : MonoBehaviour
{
    public int price = 75;
    public int usesPerPurchase = 1;
    public TextMeshProUGUI priceText;

    void Start()
    {
        if (priceText != null)
            priceText.text = "$" + price.ToString();
    }

    public void OnBuyButtonPressed()
    {
        if (!HolyWaterManager.Instance.CanBuy)
        {
            Debug.Log("Holy Water is full! (" + HolyWaterManager.Instance.maxUses + "/" + HolyWaterManager.Instance.maxUses + ")");
            return;
        }

        if (CurrencyManager.Instance.SubCurrency(price))
        {
            HolyWaterManager.Instance.AddUses(usesPerPurchase);
        }
        else
        {
            Debug.Log("Cannot afford Holy Water");
        }
    }
}