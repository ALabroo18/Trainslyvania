using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.VFX;

public class HolyWaterZone : MonoBehaviour
{
    private float radius;
    private int damagePerTick;
    private float tickRate;
    private float duration;
    private LayerMask enemyLayer;
    private VisualEffect shaderEffect;
    private Material _material;
    
    
    private Dictionary<GameObject, Dictionary<Renderer, Material[]>> vampireOriginalMaterials = new Dictionary<GameObject, Dictionary<Renderer, Material[]>>();
    private Dictionary<GameObject, Coroutine> tintedVampires = new Dictionary<GameObject, Coroutine>();

    private LineRenderer circleRenderer;
    private float elapsed = 0f;

    public void Initialize(float radius, int damagePerTick, float tickRate, float duration, LayerMask enemyLayer, VisualEffect shaderEffect, Material _material)
    {
        this.radius = radius;
        this.damagePerTick = damagePerTick;
        this.tickRate = tickRate;
        this.duration = duration;
        this.enemyLayer = enemyLayer;
        this.shaderEffect = shaderEffect;
        this._material = _material;

        DrawCircle();
        StartCoroutine(DOTRoutine());
    }

    void DrawCircle()
    {
        if(shaderEffect!=null)
        {
            shaderEffect.SetFloat("Zone Size", radius * 2);
            shaderEffect.SetFloat("Zone Lifetime", duration);
            Vector3 newPos = new Vector3(transform.position.x, .02f ,transform.position.z);
            gameObject.transform.position = newPos;
            shaderEffect.Play();
           
        }
        else
        {
            Debug.Log("Shader is cooked");
        }
        
        /*
        circleRenderer = gameObject.AddComponent<LineRenderer>();
        circleRenderer.loop = true;
        circleRenderer.startWidth = 0.15f;
        circleRenderer.endWidth = 0.15f;
        circleRenderer.material = new Material(Shader.Find("Sprites/Default"));
        circleRenderer.startColor = new Color(0.4f, 0.8f, 1f, 0.9f);
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
       */

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
                {
                    health.TakeDamage(damagePerTick);
                    Debug.Log("Taking DOT tick");
                    if (!tintedVampires.ContainsKey(col.gameObject))
                    {
                        Coroutine tintRoutine = StartCoroutine(TintVampire(col.gameObject));
                        tintedVampires[col.gameObject] = tintRoutine;
                    }
                    continue;
                }

                InfiniteVampireHealth infiniteHealth = col.GetComponent<InfiniteVampireHealth>();
                if (infiniteHealth != null)
                {
                    infiniteHealth.TakeDamage(damagePerTick);
                    if (!tintedVampires.ContainsKey(col.gameObject))
                    {
                        Coroutine tintRoutine = StartCoroutine(TintVampire(col.gameObject));
                        tintedVampires[col.gameObject] = tintRoutine;
                    }
                }
            }

            elapsed += tickRate;

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
        //grabs the render attached to the vampire 
        Renderer[] renderers = vampire.GetComponentsInChildren<Renderer>();
        
        // Now we need to created a copy of the renderer array so that we can modify the the array and swap it back
        Dictionary<Renderer, Material[]> originalColors = new Dictionary<Renderer, Material[]>();

        Dictionary<Renderer, Material[]> newColors = new Dictionary<Renderer, Material[]>();

        foreach (Renderer r in renderers)
        {
            //creates a material array that will be one size larger than the object's array
            Material[] copy = new Material[r.materials.Length + 1];
            Material [] storedColors = r.sharedMaterials;
            originalColors[r] = storedColors;
            
            //for every renderer hit by the dot grab its amterial library and copy it over to the copy arrray
            for (int i = 0; i < storedColors.Length; i++)
            {
                copy[i] = r.sharedMaterials[i];
            }
            
            Material tintMaterial = new Material(_material);
            //add the new material to the end of the copy array
            copy[storedColors.Length] = tintMaterial;
            
            //set the material renderer to the new copy array
            newColors[r] = copy;
        }
        // now we have added copies of the renderer material libraries both new and old
        vampireOriginalMaterials[vampire] = originalColors;


        foreach (var kvp in newColors)
        {
            if (kvp.Key != null)
                kvp.Key.sharedMaterials = kvp.Value;
        }
        
        yield return new WaitForSeconds(duration - elapsed);

        if (tintedVampires.ContainsKey(vampire))
            tintedVampires.Remove(vampire);
    }

    void RevertVampireColor(GameObject vampire)
    {
        if (!vampireOriginalMaterials.ContainsKey(vampire))
            return;
        
        Renderer[] renderers = vampire.GetComponentsInChildren<Renderer>();
        Dictionary<Renderer, Material[]> originalMats = vampireOriginalMaterials[vampire];
        
        foreach (Renderer r in renderers)
        {
            r.sharedMaterials = originalMats[r];
        }
        
        vampireOriginalMaterials.Remove(vampire);
        
        // Stop and remove the coroutine reference
        if (tintedVampires.ContainsKey(vampire))
        {
            if (tintedVampires[vampire] != null)
                StopCoroutine(tintedVampires[vampire]);
            tintedVampires.Remove(vampire);
        }
    }
}