using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.VFX;

public class FireBomb : MonoBehaviour
{

    public LayerMask enemyMask;
    public int immediateDamage = 50;
    public int damageOverTime = 50;
    public float dotDuration = 3f;
    public float dotTickRate = 1f;
    [SerializeField] public float radiusNum;
    public GameObject FireBombZonePrefab;
    public VisualEffect explosion;
    public Material onFire;

    public void FireRadius(Vector3 worldPosition)
    {
        float radius = radiusNum;
        int dot = damageOverTime;
        float duration = dotDuration;

        if (ModeSelector.SelectedMode == GameMode.Infinite && InfiniteUpgradeManager.Instance != null)
        {
            radius += InfiniteUpgradeManager.Instance.firebombRadiusBonus;
            dot += Mathf.RoundToInt(InfiniteUpgradeManager.Instance.firebombDOTBonus);
            duration += InfiniteUpgradeManager.Instance.firebombDurationBonus;
        }

        Collider[] enemies = Physics.OverlapSphere(worldPosition, radiusNum, enemyMask);
        Debug.Log("Firebomb hit " + enemies.Length + " enemies at radius: " + radius);

        Debug.Log("Enemies: " + enemies.Length);
        Debug.Log("In fireradius");

        foreach (Collider enemy in enemies)
        {
            if (enemy == null) continue;

            vampireHealth health = enemy.GetComponent<vampireHealth>();
            if (health != null)
            {
                health.TakeDamage(immediateDamage);
                continue;
            }

            InfiniteVampireHealth infiniteHealth = enemy.GetComponent<InfiniteVampireHealth>();
            if (infiniteHealth != null)
                infiniteHealth.TakeDamage(immediateDamage);
        }
        FireZone(worldPosition, radius, dot, duration);
    }

    public void FireZone(Vector3 position, float radius, int dot, float duration)
    {
        if (FireBombZonePrefab != null)
        {
            GameObject zone = Instantiate(FireBombZonePrefab, position, Quaternion.identity);
            FireBombZone zoneScript = zone.GetComponent<FireBombZone>();
            VisualEffect effect = zone.GetComponent<VisualEffect>();
            effect.visualEffectAsset = explosion.visualEffectAsset;
            if (zoneScript != null)
            {
                zoneScript.Initialize(radius, enemyMask, dot, dotTickRate, duration, effect, onFire);
            }
        }
        else
        {
            GameObject zone = new GameObject("FireBombZone");
            zone.transform.position = position;
            FireBombZone zoneScript = zone.AddComponent<FireBombZone>();
            VisualEffect effect = zone.AddComponent<VisualEffect>();
            effect.visualEffectAsset = explosion.visualEffectAsset;
            zoneScript.Initialize(radius, enemyMask, dot, dotTickRate, duration, effect, onFire);
        }
    }
}
