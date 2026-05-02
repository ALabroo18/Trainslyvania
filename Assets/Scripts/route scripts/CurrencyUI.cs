using UnityEngine;
using TMPro;

public class CurrencyUI : MonoBehaviour
{
    public TMP_Text currencyText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateCurrency(CurrencyManager.Instance.currency);

        CurrencyManager.Instance.OnCurrencyChanged += UpdateCurrency;
    }

    void OnDestroy()
    {
        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.OnCurrencyChanged -= UpdateCurrency;
    }

    void UpdateCurrency(int amount)
    {
        currencyText.text = amount.ToString();
    }
}
