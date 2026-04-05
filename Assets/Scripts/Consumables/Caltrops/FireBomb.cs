using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireBomb : MonoBehaviour
{

    public LayerMask enemyMask;
    public int damageOverTime = 50;
    [SerializeField] public float radiusNum;
    public GameObject FireBombZonePrefab;

    public void FireRadius(Vector2 position)
    {

        Collider[] enemies = Physics.OverlapSphere(position, radiusNum, enemyMask);

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
        FireZone(position);
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
                zoneScript.Initialize(radiusNum, enemyMask);
            }
        }
        else
        {
            GameObject zone = new GameObject("FireBombZone");
            zone.transform.position = position;
            FireBombZone zoneScript = zone.AddComponent<FireBombZone>();
            zoneScript.Initialize(radiusNum, enemyMask);
        }
    }
}
