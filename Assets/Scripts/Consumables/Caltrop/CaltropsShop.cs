using UnityEngine;
using TMPro;
public class CaltropsShop : MonoBehaviour
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
        if (!ItemManager.Instance.CanBuyCaltrops)
        {
            Debug.Log("Caltrops are full!");
            return;
        }


        if (CurrencyManager.Instance.SubCurrency(price))
        {
            ItemManager.Instance.AddCaltrops(usesPerPurchase);
        }
        else
        {
            Debug.Log("Cannot afford Caltrops");
        }
    }
}
