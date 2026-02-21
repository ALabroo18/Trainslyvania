using UnityEngine;

public class mediumTurret : MonoBehaviour
{
    //Mason Kuhn

    [Header("Firing")]
    public int damagePerShot = 50;
    public float shotsPerSecond = 3.5f;

    [Header("Targeting")]
    public LayerMask enemyLayer;
    public float searchRadius = 100f;

    [Header("Aiming")]
    public float turnSpeed = 10f;

    private Transform currentTarget;
    private float fireCooldown;

    void Update()
    {
        if (currentTarget == null)
        {
            FindClosestEnemy();
            return;
        }

        if (!currentTarget.gameObject.activeInHierarchy)
        {
            currentTarget = null;
            return;
        }

        AimAtTarget();
        HandleFiring();
    }

    void HandleFiring()
    {
        fireCooldown -= Time.deltaTime;

        if (fireCooldown <= 0f)
        {
            FireShot();
            fireCooldown = 1f / shotsPerSecond;
        }
    }

    void FireShot()
    {
        Vector3 origin = transform.position + Vector3.up * 1f;
        Vector3 direction = (currentTarget.position - origin).normalized;

        RaycastHit hit;

        if (Physics.Raycast(origin, direction, out hit, Mathf.Infinity, enemyLayer))
        {
            vampireHealth health = hit.collider.GetComponent<vampireHealth>();
            if (health != null)
            {
                health.TakeDamage(damagePerShot);
            }
        }

        Debug.DrawRay(origin, direction * 20f, Color.red, 0.15f);
    }

    void FindClosestEnemy()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            searchRadius,
            enemyLayer
        );

        float closestDist = Mathf.Infinity;
        Transform closest = null;

        foreach (Collider c in hits)
        {
            float dist = Vector3.Distance(transform.position, c.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = c.transform;
            }
        }

        currentTarget = closest;
    }

    void AimAtTarget()
    {
        Vector3 direction = currentTarget.position - transform.position;
        direction.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * turnSpeed
        );
    }
}
