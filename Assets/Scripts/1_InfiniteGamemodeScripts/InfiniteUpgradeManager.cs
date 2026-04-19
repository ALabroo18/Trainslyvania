using UnityEngine;
using System;

public class InfiniteUpgradeManager : MonoBehaviour
{
    public static InfiniteUpgradeManager Instance;

    [Header("Turret Upgrades")]
    public float turretDamageMultiplier = 1f;
    public float turretFireRateMultiplier = 1f;
    public float turretDamageUpgradeAmount = 0.25f;
    public float turretFireRateUpgradeAmount = 0.25f;
    public int turretDamageCost = 50;
    public int turretFireRateCost = 50;

    [Header("Car Upgrades")]
    public int carUpgradeCost = 100;
    public int defensiveCarUpgradeLevel = 1;
    public int passengerCarUpgradeLevel = 1;
    public int passengerCarFlatBonus = 25;
    public float passengerCarMultiplierBonus = 1.1f;

    [Header("Money Multiplier")]
    public float baseMoneyMultiplier = 1f;
    public float moneyMultiplierUpgradeAmount = 0.1f;
    public int moneyMultiplierCost = 75;

    [Header("Consumable Upgrades")]
    public float holyWaterRadiusBonus = 0f;
    public float holyWaterDOTBonus = 0f;
    public float holyWaterDurationBonus = 0f;
    public float firebombRadiusBonus = 0f;
    public float firebombDOTBonus = 0f;
    public float firebombDurationBonus = 0f;
    public int holyWaterBuyCost = 40;
    public int firebombBuyCost = 40;
    public int holyWaterUpgradeCost = 30;
    public int firebombUpgradeCost = 30;
    public float consumableUpgradeScaling = 1.15f;

    [Header("Bless Upgrades")]
    public float blessFireRateBonus = 0f;
    public float blessFireRateUpgradeAmount = 0.075f;
    public int blessFireRateCost = 80;

    public event Action OnUpgradesChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            ResetCostsAndMultipliers();
        }
        else Destroy(gameObject);
    }

    public void ResetCostsAndMultipliers()
    {
        turretDamageMultiplier = 1f;
        turretFireRateMultiplier = 1f;
        baseMoneyMultiplier = 1f;
        defensiveCarUpgradeLevel = 1;
        passengerCarUpgradeLevel = 1;
        holyWaterRadiusBonus = 0f;
        holyWaterDOTBonus = 0f;
        holyWaterDurationBonus = 0f;
        firebombRadiusBonus = 0f;
        firebombDOTBonus = 0f;
        firebombDurationBonus = 0f;

        turretDamageCost = 50;
        turretFireRateCost = 50;
        carUpgradeCost = 100;
        moneyMultiplierCost = 75;

        holyWaterBuyCost = 40;
        firebombBuyCost = 40;
        holyWaterUpgradeCost = 30;
        firebombUpgradeCost = 30;
        blessFireRateBonus = 0f;
        blessFireRateCost = 80;

        OnUpgradesChanged?.Invoke();
    }

    public void ResetUpgrades()
    {
        ResetCostsAndMultipliers();

        mediumTurret[] turrets = FindObjectsByType<mediumTurret>(FindObjectsSortMode.None);
        foreach (mediumTurret turret in turrets)
        {
            if (turret.baseDamagePerShot > 0)
                turret.damagePerShot = turret.baseDamagePerShot;
            if (turret.baseShotsPerSecond > 0)
                turret.shotsPerSecond = turret.baseShotsPerSecond;
        }
        OnUpgradesChanged?.Invoke();
    }

    public float TotalMoneyMultiplier
    {
        get
        {
            float carMultiplier = TrainManager.Instance != null
                ? TrainManager.Instance.MoneyMultiplier
                : 1f;
            return carMultiplier * baseMoneyMultiplier;
        }
    }

    public bool UpgradeTurretDamage()
    {
        if (!InfiniteRunCurrency.Instance.SpendCurrency(turretDamageCost)) return false;
        turretDamageMultiplier += turretDamageUpgradeAmount;
        turretDamageCost = Mathf.RoundToInt(turretDamageCost * 1.3f);
        ApplyTurretUpgrades();
        OnUpgradesChanged?.Invoke();
        return true;
    }

    public bool UpgradeTurretFireRate()
    {
        if (!InfiniteRunCurrency.Instance.SpendCurrency(turretFireRateCost)) return false;
        turretFireRateMultiplier += turretFireRateUpgradeAmount;
        turretFireRateCost = Mathf.RoundToInt(turretFireRateCost * 1.30f);
        OnUpgradesChanged?.Invoke();
        ApplyTurretUpgrades();
        return true;
    }

    public bool UpgradeDefensiveCar()
    {
        if (!InfiniteRunCurrency.Instance.SpendCurrency(carUpgradeCost)) return false;

        defensiveCarUpgradeLevel++;
        carUpgradeCost = Mathf.RoundToInt(carUpgradeCost * 1.4f);

        bool isMilestone = defensiveCarUpgradeLevel % 5 == 0;
        int hpBonus = isMilestone ? 200 : 100;
        int turretBonus = isMilestone ? 2 : 1;

        //add hp to all defensive cars
        trainHealth[] allHealth = FindObjectsByType<trainHealth>(FindObjectsSortMode.None);
        foreach (trainHealth h in allHealth)
            h.AddMaxHealth(hpBonus);

        //auto place turrets
        if (InfiniteTurretPlacer.Instance != null)
            InfiniteTurretPlacer.Instance.PlaceMultipleTurrets(turretBonus);
        else
            Debug.LogWarning("InfiniteTurretPlacer not found in scene");

        Debug.Log("Defensive car upgraded level " + defensiveCarUpgradeLevel + " +" + hpBonus + "hp +" + turretBonus + " turret charges");
        OnUpgradesChanged?.Invoke();
        return true;
    }

    public bool UpgradePassengerCar()
    {
        if (!InfiniteRunCurrency.Instance.SpendCurrency(carUpgradeCost)) return false;

        passengerCarUpgradeLevel++;
        carUpgradeCost = Mathf.RoundToInt(carUpgradeCost * 1.4f);

        bool isMilestone = passengerCarUpgradeLevel % 5 == 0;

        if (isMilestone)
        {
            baseMoneyMultiplier += passengerCarMultiplierBonus;
            Debug.Log("Passenger milestone! Multiplier now: " + baseMoneyMultiplier);
        }
        else
        {
            WaveManager.Instance.AddFlatRewardBonus(passengerCarFlatBonus);
            Debug.Log("Passenger upgraded, flat bonus +" + passengerCarFlatBonus);
        }

        OnUpgradesChanged?.Invoke();
        return true;
    }

    public bool UpgradeHolyWater(string stat)
    {
        if (!InfiniteRunCurrency.Instance.SpendCurrency(holyWaterUpgradeCost)) return false;
        switch (stat)
        {
            case "radius": holyWaterRadiusBonus += 1f; break;
            case "dot": holyWaterDOTBonus += 5f; break;
            case "duration": holyWaterDurationBonus += 1f; break;
        }
        holyWaterUpgradeCost = Mathf.RoundToInt(holyWaterUpgradeCost * consumableUpgradeScaling);
        OnUpgradesChanged?.Invoke();
        return true;
    }

    public bool UpgradeBlessFireRate()
    {
        if (!InfiniteRunCurrency.Instance.SpendCurrency(blessFireRateCost)) return false;
        blessFireRateBonus += blessFireRateUpgradeAmount;
        blessFireRateCost = Mathf.RoundToInt(blessFireRateCost * 1.2f);
        OnUpgradesChanged?.Invoke();
        return true;
    }

    public bool UpgradeFirebomb(string stat)
    {
        if (!InfiniteRunCurrency.Instance.SpendCurrency(firebombUpgradeCost)) return false;
        switch (stat)
        {
            case "radius": firebombRadiusBonus += 1f; break;
            case "dot": firebombDOTBonus += 10f; break;
            case "duration": firebombDurationBonus += 1f; break;
        }
        firebombUpgradeCost = Mathf.RoundToInt(firebombUpgradeCost * consumableUpgradeScaling);
        OnUpgradesChanged?.Invoke();
        return true;
    }

    //upgrade applies to all active turrets
    void ApplyTurretUpgrades()
    {
        mediumTurret[] turrets = FindObjectsByType<mediumTurret>(FindObjectsSortMode.None);
        foreach (mediumTurret turret in turrets)
        {
            turret.damagePerShot = Mathf.RoundToInt(turret.baseDamagePerShot * turretDamageMultiplier);
            turret.shotsPerSecond = turret.baseShotsPerSecond * turretFireRateMultiplier;
        }
    }

    public bool BuyConsumable(string type)
    {
        int cost = type == "holywater" ? holyWaterBuyCost : firebombBuyCost;
        if (!InfiniteRunCurrency.Instance.SpendCurrency(cost)) return false;
        if (type == "holywater") InfiniteConsumableManager.Instance.AddHolyWater(1);
        else if (type == "firebomb") InfiniteConsumableManager.Instance.AddFirebomb(1);
        OnUpgradesChanged?.Invoke();
        return true;
    }
}