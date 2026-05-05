using UnityEngine;
using UnityEngine.VFX;

public class HolyWater : MonoBehaviour
{
    [Header("Splash Settings")]
    public int splashDamage = 150;
    public float splashRadius = 4f;

    [Header("DOT Settings")]
    public int dotDamagePerTick = 25;
    public float dotTickRate = 0.5f;
    public float dotDuration = 50f;

    [Header("Bless Settings")]
    public float blessDuration = 10f;
    public float blessFireRateMultiplier = 2f;

    [Header("Layers")]
    public LayerMask enemyLayer;
    public LayerMask turretLayer;

    [Header("DOT Zone")]
    public GameObject holyWaterZonePrefab;

    [Header("DOT Effect")]
    public VisualEffect shaderEffect;

    public Material onHoly;

    public void SplashArea(Vector3 center)
    {
        float radius = splashRadius;
        int dot = dotDamagePerTick;
        float duration = dotDuration;

        if (ModeSelector.SelectedMode == GameMode.Infinite && InfiniteUpgradeManager.Instance != null)
        {
            radius += InfiniteUpgradeManager.Instance.holyWaterRadiusBonus;
            dot += Mathf.RoundToInt(InfiniteUpgradeManager.Instance.holyWaterDOTBonus);
            duration += InfiniteUpgradeManager.Instance.holyWaterDurationBonus;
        }

        Collider[] hits = Physics.OverlapSphere(center, radius, enemyLayer);
        foreach (Collider col in hits)
        {
            vampireHealth health = col.GetComponent<vampireHealth>();
            if (health != null)
                health.TakeDamage(splashDamage);

            InfiniteVampireHealth infiniteHealth = col.GetComponent<InfiniteVampireHealth>();
            if (infiniteHealth != null)
                infiniteHealth.TakeDamage(splashDamage);
        }

        if (holyWaterZonePrefab != null)
        {
            GameObject zone = Instantiate(holyWaterZonePrefab, center, Quaternion.identity);
            HolyWaterZone zoneScript = zone.GetComponent<HolyWaterZone>();
            VisualEffect effect = zone.AddComponent<VisualEffect>();
            effect.visualEffectAsset = shaderEffect.visualEffectAsset;
            if (zoneScript != null)
            {
               
                zoneScript.Initialize(radius, dot, dotTickRate, duration, enemyLayer, shaderEffect, onHoly);
                //shaderEffect.SetFloat("Zone Size", radius);
                //shaderEffect.SetFloat("Zone Lifetime", duration);
                //shaderEffect.Play();
            }

        }
        else
        {
            GameObject zone = new GameObject("HolyWaterZone");
            zone.transform.position = center;
            HolyWaterZone zoneScript = zone.AddComponent<HolyWaterZone>();
            VisualEffect effect = zone.AddComponent<VisualEffect>();
            effect.visualEffectAsset = shaderEffect.visualEffectAsset;
            zoneScript.Initialize(radius, dot, dotTickRate, duration, enemyLayer, shaderEffect, onHoly);
           // shaderEffect.SetFloat("Zone Size", radius);
           // shaderEffect.SetFloat("Zone Lifetime", duration);
           // shaderEffect.Play();
        }

        Debug.Log("Holy Water splashed - radius: " + radius + " DOT: " + dot + " duration: " + duration);
    }

    public void BlessTurret(mediumTurret turret, Vector3 point)
    {
        float duration = blessDuration;
        float fireRateMultiplier = blessFireRateMultiplier;

        if (ModeSelector.SelectedMode == GameMode.Infinite && InfiniteUpgradeManager.Instance != null)
        {
            duration += InfiniteUpgradeManager.Instance.holyWaterDurationBonus;
            fireRateMultiplier += InfiniteUpgradeManager.Instance.blessFireRateBonus;
        }

        turret.ApplyBless(duration, fireRateMultiplier);
        Debug.Log("Blessed: " + turret.gameObject.name + " duration: " + duration + " fireRate: " + fireRateMultiplier);
    }
}