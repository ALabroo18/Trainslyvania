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

    [Header("Fireball")]
    public int maxFireball = 5;
    [SerializeField] private int fireballCharges = 0;
    public event Action<int> OnFireballChanged;


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
        fireballCharges = Mathf.Clamp(fireballCharges, 0, maxFireball);
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

    public int FireballUses => fireballCharges;
    public bool CanBuyFireball => fireballCharges < maxFireball;

    public void AddFireball(int amount)
    {
        fireballCharges = Mathf.Clamp(fireballCharges + amount, 0, maxFireball);
        Save();
        OnFireballChanged?.Invoke(fireballCharges);
        Debug.Log("Fireball: " + fireballCharges + "/" + maxFireball);
    }

    public void ConsumeFireball()
    {
        fireballCharges = Mathf.Max(fireballCharges - 1, 0);
        Save();
        OnFireballChanged?.Invoke(fireballCharges);
        Debug.Log("Fireball left: " + fireballCharges + "/" + maxFireball);
    }

    void Save()
    {
        PlayerPrefs.SetInt("HolyWaterCharges", holyWaterCharges);
        PlayerPrefs.SetInt("FireballCharges", fireballCharges);
        PlayerPrefs.Save();
    }

    void Load()
    {
        holyWaterCharges = PlayerPrefs.GetInt("HolyWaterCharges", 0);
        fireballCharges = PlayerPrefs.GetInt("FireballCharges", 0);
    }
}
