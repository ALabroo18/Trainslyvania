using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Caltrops : MonoBehaviour
{

    private Vector3 Radius;
    public LayerMask enemyMask;
    public int damageOverTime = 50;
    [SerializeField] private int radiusNum;

    public void FireRadius(Vector3 position)
    {

        Collider[] enemies = Physics.OverlapSphere(position, radiusNum, enemyMask);

        Debug.Log("Enemies: " + enemies.Length);

        Debug.Log("In fireradius");
        for(int i = 0; i < enemies.Length; i++)
        {
            if(enemies[i] != null)
            {

                foreach(Collider enemy in enemies)
                {
                    vampireHealth health = enemy.GetComponent<vampireHealth>();
                    health.TakeDamage(100);
                    while(health.GetHealth() > 0 )
                    {
                        StartCoroutine(TimeDelay(5, health));
                    }
                }

            }
        }
    }


    private IEnumerator TimeDelay (int delay, vampireHealth health)
    {
        health.TakeDamage(damageOverTime);
        yield return new WaitForSeconds(delay);
    } 
}
