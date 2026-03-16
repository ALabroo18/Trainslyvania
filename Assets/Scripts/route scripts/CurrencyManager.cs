using UnityEngine;
using System;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;

    public int currency;
    public int price;

    public event Action<int> OnCurrencyChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadCurrency();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCurrency(int amount)
    {
        currency += amount;
        SaveCurrency();
        OnCurrencyChanged?.Invoke(currency);
    }

    public bool canAfford()
    {
        if (currency >= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public void SubCurrency(int price)
    {
        if (!canAfford())
        {
            return;
        }
        currency -= price;
        SaveCurrency();
        OnCurrencyChanged?.Invoke(currency);
    }

   

    public void SaveCurrency()
    {
        PlayerPrefs.SetInt("Currency", currency);
        PlayerPrefs.Save();
    }

    void LoadCurrency()
    {
        currency = PlayerPrefs.GetInt("Currency", 0);
    }
}
