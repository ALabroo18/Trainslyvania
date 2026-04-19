using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireBombZone : MonoBehaviour
{

    private LineRenderer circleRenderer;
    public LayerMask enemyMask;
    private float radiusNum;
    private int damagePerTick;
    private float tickRate;
    private float duration;
    private float elapsed = 0f;

    private Dictionary<GameObject, Coroutine> tintedVampires = new Dictionary<GameObject, Coroutine>();

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
        elapsed = 0f;

        while (elapsed < duration)
        {
            Collider[] enemies = Physics.OverlapSphere(transform.position, radiusNum, enemyMask);
            Debug.Log("DOT tick, enemies in zone: " + enemies.Length);

            foreach (Collider enemy in enemies)
            {
                if (!tintedVampires.ContainsKey(enemy.gameObject))
                {
                    Coroutine tintRoutine = StartCoroutine(TintVampire(enemy.gameObject));
                    tintedVampires[enemy.gameObject] = tintRoutine;
                }

                vampireHealth health = enemy.GetComponent<vampireHealth>();
                if (health != null)
                    health.TakeDamage(damagePerTick);

                InfiniteVampireHealth infiniteHealth = enemy.GetComponent<InfiniteVampireHealth>();
                if (infiniteHealth != null)
                    infiniteHealth.TakeDamage(damagePerTick);
            }

            elapsed += tickRate;

            float alpha = Mathf.Lerp(0.9f, 0f, elapsed / duration);
            if (circleRenderer != null)
            {
                Color faded = new Color(1f, 0.3f, 0f, alpha);
                circleRenderer.startColor = faded;
                circleRenderer.endColor = faded;
            }

            yield return new WaitForSeconds(tickRate);
        }

        foreach (var kvp in tintedVampires)
        {
            if (kvp.Key != null)
                RevertVampireColor(kvp.Key);
        }

        Destroy(gameObject);
    }

    IEnumerator TintVampire(GameObject vampire)
    {
        Renderer[] renderers = vampire.GetComponentsInChildren<Renderer>();
        Dictionary<Renderer, Color[]> originalColors = new Dictionary<Renderer, Color[]>();

        foreach (Renderer r in renderers)
        {
            Color[] colors = new Color[r.materials.Length];
            for (int i = 0; i < r.materials.Length; i++)
            {
                if (r.materials[i].HasProperty("_Color"))
                {
                    colors[i] = r.materials[i].color;
                    r.materials[i].color = new Color(1f, 0.3f, 0f);
                }
            }
            originalColors[r] = colors;
        }

        yield return new WaitForSeconds(duration - elapsed);

        foreach (var kvp in originalColors)
        {
            if (kvp.Key != null)
            {
                for (int i = 0; i < kvp.Key.materials.Length; i++)
                {
                    if (kvp.Key.materials[i].HasProperty("_Color"))
                        kvp.Key.materials[i].color = kvp.Value[i];
                }
            }
        }

        if (tintedVampires.ContainsKey(vampire))
            tintedVampires.Remove(vampire);
    }

    void RevertVampireColor(GameObject vampire)
    {
        Renderer[] renderers = vampire.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            foreach (Material m in r.materials)
            {
                if (m.HasProperty("_Color"))
                    m.color = Color.white;
            }
        }
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
        circleRenderer.startColor = new Color(1f, 0.3f, 0f, 0.9f);
        circleRenderer.endColor = new Color(1f, 0.3f, 0f, 0.9f);
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
