using UnityEngine;
using TMPro;
using System.Collections;

public class WaveShopUI : MonoBehaviour
{
    [Header("Panel")]
    public RectTransform shopPanel;
    public float slideInTime = 0.5f;
    public Vector2 hiddenPosition;
    public Vector2 shownPosition;

    [Header("Display")]
    public TextMeshProUGUI currencyText;
    public TextMeshProUGUI upcomingWaveText;

    [Header("Victory Panel Text")]
    public TextMeshProUGUI victoryWaveText;
    public TextMeshProUGUI victoryRewardText;

    [Header("Upgrade Cost Texts")]
    public TextMeshProUGUI turretDamageCostText;
    public TextMeshProUGUI turretFireRateCostText;
    public TextMeshProUGUI defensiveCarUpgradeCostText;
    public TextMeshProUGUI passengerCarUpgradeCostText;
    public TextMeshProUGUI holyWaterBuyCostText;
    public TextMeshProUGUI firebombBuyCostText;
    public TextMeshProUGUI holyWaterUpgradeCostText;
    public TextMeshProUGUI firebombUpgradeCostText;
    public TextMeshProUGUI blessFireRateCostText;

    private int lastWaveReward = 0;

    void Start()
    {
        shopPanel.anchoredPosition = hiddenPosition;

        if (WaveManager.Instance != null)
            WaveManager.Instance.OnWaveCompleted += OnWaveCompleted;
        else
            Debug.LogWarning("WaveManager.Instance is null in WaveShopUI.Start!");

        if (InfiniteRunCurrency.Instance != null)
            InfiniteRunCurrency.Instance.OnCurrencyChanged += UpdateCurrencyText;
        else
            Debug.LogWarning("InfiniteRunCurrency.Instance is null in WaveShopUI.Start!");

        if (InfiniteUpgradeManager.Instance != null)
            InfiniteUpgradeManager.Instance.OnUpgradesChanged += RefreshCosts;
        else
            Debug.LogWarning("InfiniteUpgradeManager.Instance is null in WaveShopUI.Start!");

        RefreshCosts();
    }

    void OnDestroy()
    {
        if (WaveManager.Instance != null)
            WaveManager.Instance.OnWaveCompleted -= OnWaveCompleted;
        if (InfiniteRunCurrency.Instance != null)
            InfiniteRunCurrency.Instance.OnCurrencyChanged -= UpdateCurrencyText;
        if (InfiniteUpgradeManager.Instance != null)
            InfiniteUpgradeManager.Instance.OnUpgradesChanged -= RefreshCosts;
    }

    void OnWaveCompleted(int wave)
    {
        lastWaveReward = WaveManager.Instance.LastWaveReward;
        if (victoryWaveText != null)
            victoryWaveText.text = "Wave " + wave + " Complete!";
        if (victoryRewardText != null)
            victoryRewardText.text = "You earned: $" + lastWaveReward;

        UpdateCurrencyText(InfiniteRunCurrency.Instance.Currency);
        UpdateUpcomingWaveText(wave + 1);
        StartCoroutine(SlideIn());
    }

    void UpdateUpcomingWaveText(int nextWave)
    {
        if (upcomingWaveText == null) return;
        int enemyCount = WaveManager.Instance.GetExpectedEnemyCount(nextWave);
        int enemyHP = WaveManager.Instance.GetExpectedEnemyHP(nextWave);
        upcomingWaveText.text = "Next Wave: " + nextWave + " | ~" + enemyCount + " enemies | ~" + enemyHP + "hp";
    }

    IEnumerator SlideIn()
    {
        float elapsed = 0f;
        Vector2 start = shopPanel.anchoredPosition;

        while (elapsed < slideInTime)
        {
            elapsed += Time.deltaTime;
            shopPanel.anchoredPosition = Vector2.Lerp(start, shownPosition, elapsed / slideInTime);
            yield return null;
        }

        shopPanel.anchoredPosition = shownPosition;
    }

    IEnumerator SlideOut()
    {
        float elapsed = 0f;
        Vector2 start = shopPanel.anchoredPosition;

        while (elapsed < slideInTime)
        {
            elapsed += Time.deltaTime;
            shopPanel.anchoredPosition = Vector2.Lerp(start, hiddenPosition, elapsed / slideInTime);
            yield return null;
        }

        shopPanel.anchoredPosition = hiddenPosition;
        WaveManager.Instance.StartNextWaveAfterShop();
    }

    public void CloseShopAndStartWave()
    {
        StartCoroutine(SlideOut());
    }

    void UpdateCurrencyText(int amount)
    {
        if (currencyText != null)
            currencyText.text = "Gold: " + amount;
    }

    void RefreshCosts()
    {
        if (turretDamageCostText != null)
            turretDamageCostText.text = "$" + InfiniteUpgradeManager.Instance.turretDamageCost;
        if (turretFireRateCostText != null)
            turretFireRateCostText.text = "$" + InfiniteUpgradeManager.Instance.turretFireRateCost;
        if (defensiveCarUpgradeCostText != null)
            defensiveCarUpgradeCostText.text = "$" + InfiniteUpgradeManager.Instance.carUpgradeCost;
        if (passengerCarUpgradeCostText != null)
            passengerCarUpgradeCostText.text = "$" + InfiniteUpgradeManager.Instance.carUpgradeCost;
        if (holyWaterBuyCostText != null)
            holyWaterBuyCostText.text = "$" + InfiniteUpgradeManager.Instance.holyWaterBuyCost;
        if (firebombBuyCostText != null)
            firebombBuyCostText.text = "$" + InfiniteUpgradeManager.Instance.firebombBuyCost;
        if (holyWaterUpgradeCostText != null)
            holyWaterUpgradeCostText.text = "$" + InfiniteUpgradeManager.Instance.holyWaterUpgradeCost;
        if (firebombUpgradeCostText != null)
            firebombUpgradeCostText.text = "$" + InfiniteUpgradeManager.Instance.firebombUpgradeCost;
        if (blessFireRateCostText != null)
            blessFireRateCostText.text = "$" + InfiniteUpgradeManager.Instance.blessFireRateCost;
    }

    //buttons for the shop
    public void OnUpgradeTurretDamage() => InfiniteUpgradeManager.Instance.UpgradeTurretDamage();
    public void OnUpgradeTurretFireRate() => InfiniteUpgradeManager.Instance.UpgradeTurretFireRate();
    public void OnUpgradeDefensiveCar() => InfiniteUpgradeManager.Instance.UpgradeDefensiveCar();
    public void OnUpgradePassengerCar() => InfiniteUpgradeManager.Instance.UpgradePassengerCar();
    public void OnUpgradeHolyWaterRadius() => InfiniteUpgradeManager.Instance.UpgradeHolyWater("radius");
    public void OnUpgradeHolyWaterDOT() => InfiniteUpgradeManager.Instance.UpgradeHolyWater("dot");
    public void OnUpgradeHolyWaterDuration() => InfiniteUpgradeManager.Instance.UpgradeHolyWater("duration");
    public void OnUpgradeFirebombRadius() => InfiniteUpgradeManager.Instance.UpgradeFirebomb("radius");
    public void OnUpgradeFirebombDOT() => InfiniteUpgradeManager.Instance.UpgradeFirebomb("dot");
    public void OnUpgradeFirebombDuration() => InfiniteUpgradeManager.Instance.UpgradeFirebomb("duration");
    public void OnBuyHolyWater() => InfiniteUpgradeManager.Instance.BuyConsumable("holywater");
    public void OnBuyFirebomb() => InfiniteUpgradeManager.Instance.BuyConsumable("firebomb");
    public void OnUpgradeBlessFireRate() => InfiniteUpgradeManager.Instance.UpgradeBlessFireRate();
}