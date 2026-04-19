using UnityEngine;
using System;

public class ItemManager : MonoBehaviour
{
    //create an instance so it is referencable elsewhere
    public static ItemManager Instance;

    [Header("Holy Water")]
    public int maxHolyWater = 5;
    [SerializeField] private int holyWaterCharges = 0;
    public event Action<int> OnHolyWaterChanged;

    [Header("Firebomb")]
    public int maxFirebomb = 5;
    [SerializeField] private int firebombCharges = 0;
    public event Action<int> OnFirebombChanged;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnValidate()
    {
        holyWaterCharges = Mathf.Clamp(holyWaterCharges, 0, maxHolyWater);
        firebombCharges = Mathf.Clamp(firebombCharges, 0, maxFirebomb);
    }

    public int HolyWaterUses => holyWaterCharges;
    public bool CanBuyHolyWater => holyWaterCharges < maxHolyWater;

    public void AddHolyWater(int amount)
    {
        holyWaterCharges = Mathf.Clamp(holyWaterCharges + amount, 0, maxHolyWater);
        Save();
        OnHolyWaterChanged?.Invoke(holyWaterCharges);
        Debug.Log("Holy Water: " + holyWaterCharges + "/" + maxHolyWater);
    }

    public void ConsumeHolyWater()
    {
        holyWaterCharges = Mathf.Max(holyWaterCharges - 1, 0);
        Save();
        OnHolyWaterChanged?.Invoke(holyWaterCharges);
        Debug.Log("Holy Water left: " + holyWaterCharges + "/" + maxHolyWater);
    }

    public int FirebombUses => firebombCharges;
    public bool CanBuyFirebomb => firebombCharges < maxFirebomb;

    public void AddFirebomb(int amount)
    {
        firebombCharges = Mathf.Clamp(firebombCharges + amount, 0, maxFirebomb);
        Save();
        OnFirebombChanged?.Invoke(firebombCharges);
        Debug.Log("Firebomb: " + firebombCharges + "/" + maxFirebomb);
    }

    public void ConsumeFirebomb()
    {
        firebombCharges = Mathf.Max(firebombCharges - 1, 0);
        Save();
        OnFirebombChanged?.Invoke(firebombCharges);
        Debug.Log("Firebomb left: " + firebombCharges + "/" + maxFirebomb);
    }

    void Save()
    {
        PlayerPrefs.SetInt("HolyWaterCharges", holyWaterCharges);
        PlayerPrefs.SetInt("FirebombCharges", firebombCharges);
        PlayerPrefs.Save();
    }

    void Load()
    {
        holyWaterCharges = PlayerPrefs.GetInt("HolyWaterCharges", 0);
        firebombCharges = PlayerPrefs.GetInt("FirebombCharges", 0);
    }
}
