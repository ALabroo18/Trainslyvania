using UnityEngine;
using TMPro;
using System.Collections;

public class ShopManager : MonoBehaviour
{
    public TMP_Text itemName;
    public int price = 100;
    
    public void OnBuyButtonPressed()
    {
        if (CurrencyManager.Instance.SubCurrency(price))
        {
            Debug.Log("Purchased" + itemName);
        }
        else
        {
            Debug.Log("cannot afford" + itemName);
        }
    }
}
