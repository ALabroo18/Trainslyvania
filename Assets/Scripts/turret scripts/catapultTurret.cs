using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using System.Collections.Generic;

public class catapultTurret : MonoBehaviour
{
    //Mason Kuhn

    [Header("Firing")]
    public int damagePerShot = 50;
    public float shotsPerSecond = 3.5f;
    public Transform firePoint;

    [Header("Catapult")]
    public bool isCatapult = true;
    public float splashRadius = 3f;
    public int splashDamage = 75;

    [Header("Targeting")]
    public LayerMask enemyLayer;
    public float searchRadius = 100f;

    [Header("Aiming")]
    public float turnSpeed = 10f;

    [Header("Break Behavior")]
    public Transform turretHead;
    public float brokenAngle = 45f;
    public float breakRotateTime = 0.4f;

    [Header("Bless Visual")]
    public Color blessHaloColor = new Color(0.4f, 0.8f, 1f, 0.8f);
    public float blessHaloRadius = 1.5f;
    public float blessHaloHeight = 2f;
    public float blessPulseSpeed = 2f;

    private LineRenderer blessHalo;
    private Coroutine blessPulseCoroutine;

    [Header("Projectile")]
    public GameObject projectilePrefab;

    private Coroutine blessCoroutine;

    private Transform currentTarget;
    private float fireCooldown;

    public trainHealth owningCar;
    private bool isBroken;

    [Header("Audio")]
    public AudioClip shootAudio;
    public AudioSource audioSource;

    private static Dictionary<GameObject, int> queuedDamage = new Dictionary<GameObject, int>();

    [HideInInspector] public int baseDamagePerShot;
    [HideInInspector] public float baseShotsPerSecond;

    void Start()
    {
        // if (audioSource == null)
        //     audioSource = Object.FindAnyObjectByType<AudioSource>();
        if (owningCar != null)
            owningCar.OnBreached += BreakTurret;

        baseDamagePerShot = damagePerShot;
        baseShotsPerSecond = shotsPerSecond;
    }

    void OnDestroy()
    {
        if (owningCar != null)
            owningCar.OnBreached -= BreakTurret;
    }

    void BreakTurret()
    {
        if (isBroken) return;

        isBroken = true;
        currentTarget = null;

        StopAllCoroutines();
        StartCoroutine(DropTurret());
    }

    void Update()
    {
        if (isBroken) return;

        if (currentTarget == null || !currentTarget.gameObject.activeInHierarchy)
        {
            currentTarget = null;
            FindClosestEnemy();
            return;
        }

        {
            Transform nextTarget = FindNextTarget(currentTarget);
            if (nextTarget != null)
                currentTarget = nextTarget;
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
        if (currentTarget == null) return;

        Vector3 origin = firePoint != null ? firePoint.position : transform.position;
        Vector3 direction = (currentTarget.position - origin).normalized;

        if (projectilePrefab != null)
        {

            GameObject proj = Instantiate(projectilePrefab, origin, Quaternion.LookRotation(direction));
            TurretProjectile projectile = proj.GetComponent<TurretProjectile>();

            if (projectile != null)
            {
                if (isCatapult)
                    projectile.InitializeSplash(direction, splashDamage, splashRadius, enemyLayer, currentTarget.position);
                else
                    projectile.Initialize(direction, damagePerShot, enemyLayer, currentTarget.gameObject);
            }
        }

        else
            Debug.LogWarning("No projectile prefab assigned on " + gameObject.name);

        audioSource.PlayOneShot(shootAudio);
    }

    void FindClosestEnemy()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, searchRadius, enemyLayer);
        Debug.Log("found " + hits.Length + " enemies in range");
        float closestDist = Mathf.Infinity;
        Transform closest = null;

        foreach (Collider c in hits)
        {
            Debug.Log("enemy found " + c.gameObject.name + "layer " + LayerMask.LayerToName(c.gameObject.layer));
            if (!c.transform.root.gameObject.activeInHierarchy) continue;

            float dist = Vector3.Distance(transform.position, c.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = c.transform;
            }
        }

        if (closest == null)
        {
            foreach (Collider c in hits)
            {
                if (!c.transform.root.gameObject.activeInHierarchy) continue;
                float dist = Vector3.Distance(transform.position, c.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = c.transform.root;
                }
            }
        }

        currentTarget = closest;
    }

    Transform FindNextTarget(Transform currentDyingTarget)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, searchRadius, enemyLayer);

        float closestDist = Mathf.Infinity;
        Transform closest = null;

        foreach (Collider c in hits)
        {
            if (c.transform == currentDyingTarget) continue;
            if (!c.transform.root.gameObject.activeInHierarchy) continue;

            float dist = Vector3.Distance(transform.position, c.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = c.transform.root;
            }
        }

        return closest;
    }

    void AimAtTarget()
    {
        if (currentTarget == null) return;

        Vector3 flatDirection = currentTarget.position - transform.position;
        flatDirection.y = 0f;

        if (flatDirection == Vector3.zero) return;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(flatDirection), Time.deltaTime * turnSpeed);

    }

    IEnumerator DropTurret()
    {
        if (turretHead == null)
            yield break;

        Quaternion startRot = turretHead.localRotation;
        Quaternion targetRot = Quaternion.Euler(brokenAngle, 0f, 0f);

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / breakRotateTime;
            turretHead.localRotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        turretHead.localRotation = targetRot;
    }

    public void ApplyBless(float duration, float multiplier)
    {
        if (isBroken) return;
        if (blessCoroutine != null)
            StopCoroutine(blessCoroutine);
        blessCoroutine = StartCoroutine(BlessRoutine(duration, multiplier));
    }

    IEnumerator BlessRoutine(float duration, float multiplier)
    {
        float originalFireRate = shotsPerSecond;
        shotsPerSecond *= multiplier;

        CreateBlessHalo();
        blessPulseCoroutine = StartCoroutine(PulseHalo());

        Debug.Log(gameObject.name + " blessed! Fire rate: " + shotsPerSecond);

        yield return new WaitForSeconds(duration);

        shotsPerSecond = originalFireRate;
        blessCoroutine = null;

        DestroyBlessHalo();
        Debug.Log(gameObject.name + " blessing wore off");
    }

    void CreateBlessHalo()
    {
        GameObject haloObj = new GameObject("BlessHalo");
        haloObj.transform.SetParent(transform);
        haloObj.transform.localPosition = Vector3.up * blessHaloHeight;

        blessHalo = haloObj.AddComponent<LineRenderer>();
        blessHalo.loop = true;
        blessHalo.startWidth = 0.08f;
        blessHalo.endWidth = 0.08f;
        blessHalo.material = new Material(Shader.Find("Sprites/Default"));
        blessHalo.startColor = blessHaloColor;
        blessHalo.endColor = blessHaloColor;
        blessHalo.positionCount = 32;
        blessHalo.useWorldSpace = false;

        for (int i = 0; i < 32; i++)
        {
            float angle = i / 32f * Mathf.PI * 2f;
            float x = Mathf.Cos(angle) * blessHaloRadius;
            float z = Mathf.Sin(angle) * blessHaloRadius;
            blessHalo.SetPosition(i, new Vector3(x, 0f, z));
        }
    }

    void DestroyBlessHalo()
    {
        if (blessHalo != null)
        {
            Destroy(blessHalo.gameObject);
            blessHalo = null;
        }
        if (blessPulseCoroutine != null)
        {
            StopCoroutine(blessPulseCoroutine);
            blessPulseCoroutine = null;
        }
    }

    IEnumerator PulseHalo()
    {
        while (blessHalo != null)
        {
            float t = (Mathf.Sin(Time.time * blessPulseSpeed) + 1f) / 2f;
            Color pulsedColor = new Color(
                blessHaloColor.r,
                blessHaloColor.g,
                blessHaloColor.b,
                Mathf.Lerp(0.2f, 1f, t)
            );
            blessHalo.startColor = pulsedColor;
            blessHalo.endColor = pulsedColor;

            float pulseRadius = blessHaloRadius + Mathf.Sin(Time.time * blessPulseSpeed) * 0.2f;
            for (int i = 0; i < 32; i++)
            {
                float angle = i / 32f * Mathf.PI * 2f;
                blessHalo.SetPosition(i, new Vector3(
                    Mathf.Cos(angle) * pulseRadius,
                    0f,
                    Mathf.Sin(angle) * pulseRadius
                ));
            }

            yield return null;
        }
    }
}