using UnityEngine;
using System;

public class InfiniteConsumableManager : MonoBehaviour
{
    public static InfiniteConsumableManager Instance;

    [SerializeField] private int holyWaterCharges = 0;
    [SerializeField] private int firebombCharges = 0;

    public event Action<int, int> OnChargesChanged;

    public int HolyWaterCharges => holyWaterCharges;
    public int FirebombCharges => firebombCharges;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            holyWaterCharges = 0;
            firebombCharges = 0;
        }
        else Destroy(gameObject);
    }

    public void AddHolyWater(int amount)
    {
        holyWaterCharges += amount;
        OnChargesChanged?.Invoke(holyWaterCharges, firebombCharges);
        Debug.Log("Holy Water charges: " + holyWaterCharges);
    }
    public void AddFirebomb(int amount)
    {
        firebombCharges += amount;
        OnChargesChanged?.Invoke(holyWaterCharges, firebombCharges);
        Debug.Log("Firebomb charges: " + firebombCharges);
    }

    public bool UseHolyWater()
    {
        if (holyWaterCharges <= 0)
        {
            Debug.Log("No Holy Water charges!");
            return false;
        }
        holyWaterCharges--;
        OnChargesChanged?.Invoke(holyWaterCharges, firebombCharges);
        return true;
    }

    public bool UseFirebomb()
    {
        if (firebombCharges <= 0)
        {
            Debug.Log("No Firebomb charges!");
            return false;
        }
        firebombCharges--;
        OnChargesChanged?.Invoke(holyWaterCharges, firebombCharges);
        return true;
    }
}