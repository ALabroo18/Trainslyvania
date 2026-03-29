using UnityEngine;
using System;

public class HolyWaterManager : MonoBehaviour
{
    public static HolyWaterManager Instance;

    public int maxUses = 5;

    [SerializeField] private int debugUses = 0;

    public event Action<int> OnUsesChanged;

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
        debugUses = Mathf.Clamp(debugUses, 0, maxUses);
    }

    public int Uses => debugUses;

    public bool CanBuy => debugUses < maxUses;

    public void AddUses(int amount)
    {
        debugUses = Mathf.Clamp(debugUses + amount, 0, maxUses);
        Save();
        OnUsesChanged?.Invoke(debugUses);
        Debug.Log("Holy Water uses: " + debugUses + "/" + maxUses);
    }

    public void ConsumeUse()
    {
        debugUses = Mathf.Max(debugUses - 1, 0);
        Save();
        OnUsesChanged?.Invoke(debugUses);
        Debug.Log("Holy Water uses left: " + debugUses + "/" + maxUses);
    }

    void Save()
    {
        PlayerPrefs.SetInt("HolyWaterUses", debugUses);
        PlayerPrefs.Save();
    }

    void Load()
    {
        debugUses = PlayerPrefs.GetInt("HolyWaterUses", 0);
    }
}