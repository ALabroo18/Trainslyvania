using UnityEngine;

public class HolyWater : MonoBehaviour
{
    [Header("Splash Settings")]
    public int splashDamage = 150;
    public float splashRadius = 4f;

    [Header("DOT Settings")]
    public int dotDamagePerTick = 25;
    public float dotTickRate = 0.5f;
    public float dotDuration = 5f;

    [Header("Bless Settings")]
    public float blessDuration = 10f;
    public float blessFireRateMultiplier = 2f;

    [Header("Layers")]
    public LayerMask enemyLayer;
    public LayerMask turretLayer;

    [Header("DOT Zone")]
    public GameObject holyWaterZonePrefab;

    public void SplashArea(Vector3 center)
    {
        Collider[] hits = Physics.OverlapSphere(center, splashRadius, enemyLayer);
        foreach (Collider col in hits)
        {
            vampireHealth health = col.GetComponent<vampireHealth>();
            if (health != null)
                health.TakeDamage(splashDamage);
        }

        if (holyWaterZonePrefab != null)
        {
            GameObject zone = Instantiate(holyWaterZonePrefab, center, Quaternion.identity);
            HolyWaterZone zoneScript = zone.GetComponent<HolyWaterZone>();
            if (zoneScript != null)
            {
                zoneScript.Initialize(splashRadius, dotDamagePerTick, dotTickRate, dotDuration, enemyLayer);
            }
        }
        else
        {
            GameObject zone = new GameObject("HolyWaterZone");
            zone.transform.position = center;
            HolyWaterZone zoneScript = zone.AddComponent<HolyWaterZone>();
            zoneScript.Initialize(splashRadius, dotDamagePerTick, dotTickRate, dotDuration, enemyLayer);
        }

        Debug.Log("Holy Water splashed, hit " + hits.Length + " vampires");
    }

    public void BlessTurret(mediumTurret turret, Vector3 point)
    {
        turret.ApplyBless(blessDuration, blessFireRateMultiplier);
        Debug.Log("Blessed: " + turret.gameObject.name);
    }
}