using UnityEngine;
using TMPro;

public class FirebombShop : MonoBehaviour
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
        if (!ItemManager.Instance.CanBuyFireball)
        {
            Debug.Log("Fireball is full!");
            return;
        }

        if (CurrencyManager.Instance.SubCurrency(price))
        {
            ItemManager.Instance.AddFireball(usesPerPurchase);
        }
        else
        {
            Debug.Log("Cannot afford Fireball");
        }
    }
}
