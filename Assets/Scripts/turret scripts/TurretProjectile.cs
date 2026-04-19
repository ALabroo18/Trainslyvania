using UnityEngine;
using System.Collections;

public class TurretProjectile : MonoBehaviour
{
    public float speed = 30f;
    public int damage = 0;
    public LayerMask enemyLayer;
    public float activationDelay = 0.05f;

    private Vector3 direction;
    private bool active = false;
    private Collider projectileCollider;
    private GameObject intendedTarget;

    public void Initialize(Vector3 dir, int dmg, LayerMask layer)
    {
        direction = dir.normalized;
        damage = dmg;
        enemyLayer = layer;
        transform.localScale = Vector3.one * 20f;
        Destroy(gameObject, 2f);

        projectileCollider = GetComponent<Collider>();
        if (projectileCollider != null)
            projectileCollider.enabled = false;

        StartCoroutine(ActivateAfterDelay());
    }

    IEnumerator ActivateAfterDelay()
    {
        yield return new WaitForSeconds(activationDelay);
        if (projectileCollider != null)
            projectileCollider.enabled = true;
        active = true;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!active) return;
        if (((1 << other.gameObject.layer) & enemyLayer) == 0) return;

        vampireHealth health = other.GetComponent<vampireHealth>();
        if (health != null)
        {
            mediumTurret.ClearQueuedDamage(other.gameObject);
            health.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        InfiniteVampireHealth infiniteHealth = other.GetComponent<InfiniteVampireHealth>();
        if (infiniteHealth != null)
        {
            mediumTurret.ClearQueuedDamage(other.gameObject);
            infiniteHealth.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (intendedTarget != null)
            mediumTurret.ClearQueuedDamage(intendedTarget);
    }
}
