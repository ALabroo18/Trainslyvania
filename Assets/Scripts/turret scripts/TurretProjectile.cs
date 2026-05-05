using UnityEngine;
using System.Collections;

public class TurretProjectile : MonoBehaviour
{
    public float speed = 30f;
    public int damage = 0;
    public LayerMask enemyLayer;
    public float activationDelay = 0.05f;

    private bool isSplash = false;
    private float splashRadius = 0f;
    private LayerMask splashLayer;
    private Vector3 targetPosition;

    private Vector3 direction;
    private bool active = false;
    private Collider projectileCollider;
    private GameObject intendedTarget;

    public void Initialize(Vector3 dir, int dmg, LayerMask layer, GameObject target)
    {
        direction = dir.normalized;
        damage = dmg;
        enemyLayer = layer;
        intendedTarget = target;
        transform.localScale = Vector3.one * 20f;
        Destroy(gameObject, 2f);

        projectileCollider = GetComponent<Collider>();
        if (projectileCollider != null)
            projectileCollider.enabled = false;

        StartCoroutine(ActivateAfterDelay());
    }

    public void InitializeSplash(Vector3 dir, int dmg, float radius, LayerMask layer, Vector3 target)
    {
        direction = dir.normalized;
        damage = dmg;
        splashRadius = radius;
        splashLayer = layer;
        targetPosition = target;
        isSplash = true;
        Destroy(gameObject, 5f);

        StartCoroutine(FlyToTarget());
    }

    IEnumerator FlyToTarget()
    {
        float travelTime = Vector3.Distance(transform.position, targetPosition) / speed;
        float elapsed = 0f;
        Vector3 startPos = transform.position;

        while (elapsed < travelTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / travelTime;

            Vector3 pos = Vector3.Lerp(startPos, targetPosition, t);
            pos.y += Mathf.Sin(t * Mathf.PI) * 5f;
            transform.position = pos;

            if (elapsed > Time.deltaTime)
            {
                Vector3 moveDir = (pos - transform.position).normalized;
                if (moveDir != Vector3.zero)
                    transform.rotation = Quaternion.LookRotation(moveDir);
            }

            yield return null;
        }

        SplashOnImpact();
    }


    void SplashOnImpact()
    {
        GameObject impactObj = new GameObject("SplashImpact");
        impactObj.transform.position = targetPosition;
        LineRenderer lr = impactObj.AddComponent<LineRenderer>();
        lr.loop = true;
        lr.startWidth = 0.15f;
        lr.endWidth = 0.15f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = new Color(1f, 0.5f, 0f, 0.9f);
        lr.endColor = new Color(1f, 0.5f, 0f, 0.9f);
        lr.positionCount = 32;
        lr.useWorldSpace = true;
        for (int i = 0; i < 32; i++)
        {
            float angle = i / 32f * Mathf.PI * 2f;
            lr.SetPosition(i, targetPosition + new Vector3(Mathf.Cos(angle) * splashRadius, 0.1f, Mathf.Sin(angle) * splashRadius));
        }
        Destroy(impactObj, 0.5f);

        Collider[] hits = Physics.OverlapSphere(targetPosition, splashRadius, splashLayer);
        foreach (Collider col in hits)
        {
            vampireHealth health = col.GetComponent<vampireHealth>();
            if (health != null) { health.TakeDamage(damage); continue; }

            InfiniteVampireHealth infiniteHealth = col.GetComponent<InfiniteVampireHealth>();
            if (infiniteHealth != null) infiniteHealth.TakeDamage(damage);
        }

        Debug.Log("Catapult splash hit " + hits.Length + " enemies at radius " + splashRadius);
        Destroy(gameObject);
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
        if (isSplash) return;
        if (!active) return;
        transform.position += direction * speed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("fuck you");
        if (!active) return;
        if (((1 << other.gameObject.layer) & enemyLayer) == 0) return;

        vampireHealth health = other.GetComponentInParent<vampireHealth>();
        if (health != null)
        {
            mediumTurret.ClearQueuedDamage(other.transform.root.gameObject);
            health.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        InfiniteVampireHealth infiniteHealth = other.GetComponentInParent<InfiniteVampireHealth>();
        if (infiniteHealth != null)
        {
            mediumTurret.ClearQueuedDamage(other.transform.root.gameObject);
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