using UnityEngine;
using System.Collections;

public class FireBombZone : MonoBehaviour
{

    private LineRenderer circleRenderer;
    public LayerMask enemyMask;
    private float radiusNum;
    private int damagePerTick;
    private float tickRate;
    private float duration;

    public void Initialize(float radiusNum, LayerMask enemyMask, int damagePerTick, float tickRate, float duration)
    {
        this.radiusNum = radiusNum;
        this.enemyMask = enemyMask;
        this.damagePerTick = damagePerTick;
        this.tickRate = tickRate;
        this.duration = duration;

        DrawCircle(transform.position);
        StartCoroutine(DOTRoutine());
    }

    IEnumerator DOTRoutine()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Collider[] enemies = Physics.OverlapSphere(transform.position, radiusNum, enemyMask);
            Debug.Log("DOT tick, enemies in zone: " + enemies.Length);

            foreach (Collider enemy in enemies)
            {
                vampireHealth health = enemy.GetComponent<vampireHealth>();
                if (health != null)
                    health.TakeDamage(damagePerTick);
            }

            elapsed += tickRate;

            float alpha = Mathf.Lerp(0.9f, 0f, elapsed / duration);
            if (circleRenderer != null)
            {
                Color faded = new Color(1f, 0.3f, 0.7f, alpha);
                circleRenderer.startColor = faded;
                circleRenderer.endColor = faded;
            }

            yield return new WaitForSeconds(tickRate);
        }

        Destroy(gameObject);
    }

    void DrawCircle(Vector3 center)
    {
        circleRenderer = gameObject.GetComponent<LineRenderer>();
        if (circleRenderer == null)
            circleRenderer = gameObject.AddComponent<LineRenderer>();

        circleRenderer.loop = true;
        circleRenderer.startWidth = 0.15f;
        circleRenderer.endWidth = 0.15f;
        circleRenderer.material = new Material(Shader.Find("Sprites/Default"));
        circleRenderer.startColor = new Color(1f, 0.3f, 7f, 0.9f);
        circleRenderer.endColor = new Color(1f, 0.3f, 7f, 0.9f);
        circleRenderer.positionCount = 64;
        circleRenderer.useWorldSpace = true;

        for (int i = 0; i < 64; i++)
        {
            float angle = i / 64f * Mathf.PI * 2f;
            float x = Mathf.Cos(angle) * radiusNum;
            float z = Mathf.Sin(angle) * radiusNum;

            Vector3 point = new Vector3(center.x + x, 100f, center.z + z);
            if (Physics.Raycast(point, Vector3.down, out RaycastHit hit, 200f))
            {
                circleRenderer.SetPosition(i, hit.point + Vector3.up * 0.1f);
            }
            else
            {
                circleRenderer.SetPosition(i, new Vector3(center.x + x, center.y + 0.1f, center.z + z));
            }
        }
        Debug.Log("Circle Drawn");
    }
}
