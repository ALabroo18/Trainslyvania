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


    [Header("Caltrops")]
    public int maxCaltrops = 5;
    [SerializeField] private int caltropCharges = 0;
    public event Action<int> OnCaltropsChanged;


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
        caltropCharges = Mathf.Clamp(fireballCharges, 0, maxFireball);
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

    public int CaltropsUses => caltropCharges;
    public bool CanBuyCaltrops => caltropCharges < maxCaltrops;
    public void AddCaltrops(int amount)
    {
        caltropCharges = Mathf.Clamp(caltropCharges + amount, 0, caltropCharges);
        Save();
        OnCaltropsChanged?.Invoke(caltropCharges);
        Debug.Log("Fireball: " + caltropCharges + "/" + maxCaltrops);
    }

    public void ConsumeCaltrops()
    {
        caltropCharges = Mathf.Max(caltropCharges - 1, 0);
        Save();
        OnCaltropsChanged?.Invoke(caltropCharges);
        Debug.Log("Fireball left: " + caltropCharges + "/" + maxCaltrops);
    }

    void Save()
    {
        PlayerPrefs.SetInt("HolyWaterCharges", holyWaterCharges);
        PlayerPrefs.SetInt("FireballCharges", fireballCharges);
        PlayerPrefs.SetInt("CaltropCharges", caltropCharges);
        PlayerPrefs.Save();
    }

    void Load()
    {
        holyWaterCharges = PlayerPrefs.GetInt("HolyWaterCharges", 0);
        fireballCharges = PlayerPrefs.GetInt("FireballCharges", 0);
        fireballCharges = PlayerPrefs.GetInt("caltropCharges", 0);
    }
}
