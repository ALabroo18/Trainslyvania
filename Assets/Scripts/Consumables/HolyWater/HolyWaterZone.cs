using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HolyWaterZone : MonoBehaviour
{
    private float radius;
    private int damagePerTick;
    private float tickRate;
    private float duration;
    private LayerMask enemyLayer;

    private Dictionary<GameObject, Coroutine> tintedVampires = new Dictionary<GameObject, Coroutine>();

    private LineRenderer circleRenderer;
    private float elapsed = 0f;

    public void Initialize(float radius, int damagePerTick, float tickRate, float duration, LayerMask enemyLayer)
    {
        this.radius = radius;
        this.damagePerTick = damagePerTick;
        this.tickRate = tickRate;
        this.duration = duration;
        this.enemyLayer = enemyLayer;

        DrawCircle();
        StartCoroutine(DOTRoutine());
    }

    void DrawCircle()
    {
        circleRenderer = gameObject.AddComponent<LineRenderer>();
        circleRenderer.loop = true;
        circleRenderer.startWidth = 0.15f;
        circleRenderer.endWidth = 0.15f;
        circleRenderer.material = new Material(Shader.Find("Sprites/Default"));
        circleRenderer.startColor = new Color(0.4f, 0.8f, 1f, 0.9f); // light blue
        circleRenderer.endColor = new Color(0.4f, 0.8f, 1f, 0.9f);
        circleRenderer.positionCount = 64;
        circleRenderer.useWorldSpace = true;

        for (int i = 0; i < 64; i++)
        {
            float angle = i / 64f * Mathf.PI * 2f;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            circleRenderer.SetPosition(i, transform.position + new Vector3(x, 0.1f, z));
        }
    }

    IEnumerator DOTRoutine()
    {
        while (elapsed < duration)
        {

            Collider[] hits = Physics.OverlapSphere(transform.position, radius, enemyLayer);
            foreach (Collider col in hits)
            {
                vampireHealth health = col.GetComponent<vampireHealth>();
                if (health != null)
                    health.TakeDamage(damagePerTick);

                if (!tintedVampires.ContainsKey(col.gameObject))
                {
                    Coroutine tintRoutine = StartCoroutine(TintVampire(col.gameObject));
                    tintedVampires[col.gameObject] = tintRoutine;
                }
            }

            elapsed += tickRate;
            float alpha = Mathf.Lerp(0.9f, 0f, elapsed / duration);
            Color fadedColor = new Color(0.4f, 0.8f, 1f, alpha);
            circleRenderer.startColor = fadedColor;
            circleRenderer.endColor = fadedColor;

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
                    r.materials[i].color = Color.black;
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
}