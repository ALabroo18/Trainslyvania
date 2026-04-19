using UnityEngine;
using TMPro;

public class WaveShopStatsDisplay : MonoBehaviour
{
    [Header("Turret Stats")]
    public TextMeshProUGUI turretDamageText;
    public TextMeshProUGUI turretFireRateText;

    [Header("Defensive Car Stats")]
    public TextMeshProUGUI defensiveCarHPText;
    public TextMeshProUGUI defensiveCarTurretsText;

    [Header("Passenger Car Stats")]
    public TextMeshProUGUI passengerMultiplierText;
    public TextMeshProUGUI passengerFlatBonusText;

    [Header("Holy Water Stats")]
    public TextMeshProUGUI holyWaterRadiusText;
    public TextMeshProUGUI holyWaterDOTText;
    public TextMeshProUGUI holyWaterDurationText;
    public TextMeshProUGUI blessFireRateText;

    [Header("Firebomb Stats")]
    public TextMeshProUGUI firebombRadiusText;
    public TextMeshProUGUI firebombDOTText;
    public TextMeshProUGUI firebombDurationText;

    [Header("Base Values")]
    public float baseHolyWaterRadius = 5f;
    public float baseHolyWaterDOT = 10f;
    public float baseHolyWaterDuration = 5f;
    public float baseFirebombRadius = 5f;
    public float baseFirebombDOT = 50f;
    public float baseFirebombDuration = 5f;
    public float baseHolyWaterBlessFireRate = 2.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (InfiniteUpgradeManager.Instance != null)
            InfiniteUpgradeManager.Instance.OnUpgradesChanged += RefreshStats;
        RefreshStats();
    }

    void OnDestroy()
    {
        if (InfiniteUpgradeManager.Instance != null)
            InfiniteUpgradeManager.Instance.OnUpgradesChanged -= RefreshStats;
    }

    void RefreshStats()
    {
        if (InfiniteUpgradeManager.Instance == null) return;
        InfiniteUpgradeManager u = InfiniteUpgradeManager.Instance;

        //turret stats
        mediumTurret[] turrets = FindObjectsByType<mediumTurret>(FindObjectsSortMode.None);
        if (turrets.Length > 0)
        {
            if (turretDamageText != null)
                turretDamageText.text = "Damage: " + turrets[0].damagePerShot;
            if (turretFireRateText != null)
                turretFireRateText.text = "Fire Rate: " + turrets[0].shotsPerSecond.ToString("F1") + "/s";
        }
        else
        {
            if (turretDamageText != null)
                turretDamageText.text = "Damage: x" + u.turretDamageMultiplier.ToString("F2");
            if (turretFireRateText != null)
                turretFireRateText.text = "Fire Rate: x" + u.turretFireRateMultiplier.ToString("F2");
        }

        //defensive car stats
        trainHealth[] allHealth = FindObjectsByType<trainHealth>(FindObjectsSortMode.None);
        int totalHP = 0;
        int currentHP = 0;
        foreach (trainHealth h in allHealth)
        {
            totalHP += h.maxHealth;
            currentHP += h.currentHealth;
        }

        if (defensiveCarHPText != null)
            defensiveCarHPText.text = "Train HP: " + currentHP + "/" + totalHP;

        if (defensiveCarTurretsText != null)
        {
            int turretCount = InfiniteTurretPlacer.Instance != null
                ? InfiniteTurretPlacer.Instance.PlacedTurretCount
                : FindObjectsByType<mediumTurret>(FindObjectsSortMode.None).Length;
            defensiveCarTurretsText.text = "Turrets: " + turretCount;
        }

        //passenger car stats
        if (passengerMultiplierText != null)
            passengerMultiplierText.text = "Money Multiplier: x" + u.TotalMoneyMultiplier.ToString("F2");

        if (passengerFlatBonusText != null)
            passengerFlatBonusText.text = "Wave Bonus: +" + (WaveManager.Instance != null ? WaveManager.Instance.FlatRewardBonus : 0);


        //holy water stats
        if (holyWaterRadiusText != null)
            holyWaterRadiusText.text = "Radius: " + (baseHolyWaterRadius + u.holyWaterRadiusBonus).ToString("F1");
        if (holyWaterDOTText != null)
            holyWaterDOTText.text = "DOT: " + (baseHolyWaterDOT + u.holyWaterDOTBonus).ToString("F0");
        if (holyWaterDurationText != null)
            holyWaterDurationText.text = "Duration: " + (baseHolyWaterDuration + u.holyWaterDurationBonus).ToString("F1") + "s";
        if (blessFireRateText != null)
            blessFireRateText.text = "Bless Fire Rate: x" + (baseHolyWaterBlessFireRate + InfiniteUpgradeManager.Instance.blessFireRateBonus).ToString("F2");

        //firebomb stats
        if (firebombRadiusText != null)
            firebombRadiusText.text = "Radius: " + (baseFirebombRadius + u.firebombRadiusBonus).ToString("F1");
        if (firebombDOTText != null)
            firebombDOTText.text = "DOT: " + (baseFirebombDOT + u.firebombDOTBonus).ToString("F0");
        if (firebombDurationText != null)
            firebombDurationText.text = "Duration: " + (baseFirebombDuration + u.firebombDurationBonus).ToString("F1") + "s";
    }
}