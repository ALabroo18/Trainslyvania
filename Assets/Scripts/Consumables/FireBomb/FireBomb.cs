using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireBomb : MonoBehaviour
{

    public LayerMask enemyMask;
    public int immediateDamage = 50;
    public int damageOverTime = 50;
    public float dotDuration = 3f;
    public float dotTickRate = 1f;
    [SerializeField] public float radiusNum;
    public GameObject FireBombZonePrefab;

    public void FireRadius(Vector3 worldPosition)
    {

        Collider[] enemies = Physics.OverlapSphere(worldPosition, radiusNum, enemyMask);

        Debug.Log("Enemies: " + enemies.Length);
        Debug.Log("In fireradius");
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
            {

                foreach (Collider enemy in enemies)
                {
                    vampireHealth health = enemy.GetComponent<vampireHealth>();
                    health.TakeDamage(100);
                    while (health.GetHealth() > 0)
                    {
                        StartCoroutine(TimeDelay(1, health));
                    }
                }
            }
        }
        FireZone(worldPosition);
    }


    private IEnumerator TimeDelay(int delay, vampireHealth health)
    {
        health.TakeDamage(damageOverTime);
        yield return new WaitForSeconds(delay);
    }


    public void FireZone(Vector3 position)
    {
        if (FireBombZonePrefab != null)
        {
            GameObject zone = Instantiate(FireBombZonePrefab, position, Quaternion.identity);
            FireBombZone zoneScript = zone.GetComponent<FireBombZone>();
            if (zoneScript != null)
            {
                zoneScript.Initialize(radiusNum, enemyMask, damageOverTime, dotTickRate, dotDuration);
            }
        }
        else
        {
            GameObject zone = new GameObject("FireBombZone");
            zone.transform.position = position;
            FireBombZone zoneScript = zone.AddComponent<FireBombZone>();
            zoneScript.Initialize(radiusNum, enemyMask, damageOverTime, dotTickRate, dotDuration);
        }
    }
}
