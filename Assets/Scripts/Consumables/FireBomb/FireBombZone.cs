using UnityEngine;
using UnityEngine.VFX;
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
    private VisualEffect explosion;
    private Material _material;
    
    
    private Dictionary<GameObject, Dictionary<Renderer, Material[]>> vampireOriginalMaterials = new Dictionary<GameObject, Dictionary<Renderer, Material[]>>();
    private Dictionary<GameObject, Coroutine> tintedVampires = new Dictionary<GameObject, Coroutine>();

    public void Initialize(float radiusNum, LayerMask enemyMask, int damagePerTick, float tickRate, float duration, VisualEffect explosion, Material _material)
    {
        this.radiusNum = radiusNum;
        this.enemyMask = enemyMask;
        this.damagePerTick = damagePerTick;
        this.tickRate = tickRate;
        this.duration = duration;
        this.explosion = explosion;
        this._material = _material;

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
                    // DOT is called
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
            
            //add the new material to the end of the copy array
            copy[storedColors.Length] = _material; 
            
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
    
    // Now plays the explosion VFX

    void DrawCircle(Vector3 center)
    {

        if (explosion != null)
        {
            explosion.SetFloat("Flash Size", radiusNum * 2);
            explosion.Play();

        }
        else
        {
            Debug.Log("Shader is cooked");
        }



    }
        
}
