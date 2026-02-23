using UnityEngine;

public class vampireDamage : MonoBehaviour
{
    //Mason Kuhn

    public int damage = 10;
    public float attackCooldown = 10f;

    private float lastAttackTime;
    private trainHealth trainHealth;

    void OnTriggerEnter(Collider other)
    {
        trainHealth th = other.GetComponentInParent<trainHealth>();
        if (th)
        {
            trainHealth = th;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!trainHealth) return;

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            trainHealth.TakeDamage(damage);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<trainHealth>() == trainHealth)
        {
            trainHealth = null;
        }
    }
}