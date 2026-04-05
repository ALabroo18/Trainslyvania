using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Caltrops : MonoBehaviour
{

    public LayerMask enemyMask;
    public int damageOverTime = 50;
    [SerializeField] public float radiusNum;


    public GameObject CaltropZonePrefab;

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
        CaltropsZone(position);
    }


    private IEnumerator TimeDelay(int delay, vampireHealth health)
    {
        health.TakeDamage(damageOverTime);
        yield return new WaitForSeconds(delay);
    }


    public void CaltropsZone(Vector3 position)
    {
        if (CaltropZonePrefab != null)
        {
            GameObject zone = Instantiate(CaltropZonePrefab, position, Quaternion.identity);
            CaltropZone zoneScript = zone.GetComponent<CaltropZone>();
            if (zoneScript != null)
            {
                zoneScript.Initialize(radiusNum, enemyMask);
            }
        }
        else
        {
            GameObject zone = new GameObject("CaltropZone");
            zone.transform.position = position;
            CaltropZone zoneScript = zone.AddComponent<CaltropZone>();
            zoneScript.Initialize(radiusNum, enemyMask);
        }
    }
}
