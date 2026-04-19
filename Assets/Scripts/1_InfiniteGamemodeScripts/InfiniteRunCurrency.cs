using UnityEngine;
using System;

public class InfiniteRunCurrency : MonoBehaviour
{
    public static InfiniteRunCurrency Instance;

    [SerializeField] private int currency = 0;
    public event Action<int> OnCurrencyChanged;

    public int Currency => currency;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            currency = 0;
        }
        else Destroy(gameObject);
    }

    public void AddCurrency(int amount)
    {
        currency += amount;
        OnCurrencyChanged?.Invoke(currency);
        Debug.Log("Run currency: " + currency);
    }

    public bool SpendCurrency(int amount)
    {
        if (currency < amount)
        {
            Debug.Log("Not enough run currency!");
            return false;
        }
        currency -= amount;
        OnCurrencyChanged?.Invoke(currency);
        return true;
    }
}