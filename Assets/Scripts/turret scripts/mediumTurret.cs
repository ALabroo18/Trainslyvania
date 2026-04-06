using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class mediumTurret : MonoBehaviour
{
    //Mason Kuhn

    [Header("Firing")]
    public int damagePerShot = 50;
    public float shotsPerSecond = 3.5f;
    public Transform firePoint;

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
    public Color normalShotColor = Color.red;
    public Color blessedShotColor = Color.cyan;
    public float blessedShotWidth = 0.05f;
    public float normalShotWidth = 0.01f;

    private Coroutine blessCoroutine;

    private Transform currentTarget;
    private float fireCooldown;

    public trainHealth owningCar;
    private bool isBroken;
    private bool isBlessed = false;
    // [Header("Audio")]
    // public AudioClip shootAudio;
    // public AudioSource audioSource;


    void Start()
    {
        // if (audioSource == null)
        //     audioSource = Object.FindAnyObjectByType<AudioSource>();
        if (owningCar != null)
            owningCar.OnBreached += BreakTurret;
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
        if (isBroken)
            return;

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
        Vector3 origin = firePoint.position;
        Vector3 direction = (currentTarget.position - origin).normalized;
        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, Mathf.Infinity, enemyLayer))
        {
            vampireHealth health = hit.collider.GetComponent<vampireHealth>();
            if (health != null)
                health.TakeDamage(damagePerShot);
        }
        DrawShotVisual(origin, direction);
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

    void DrawShotVisual(Vector3 origin, Vector3 direction)
    {
        Color color = isBlessed ? blessedShotColor : normalShotColor;
        float width = isBlessed ? blessedShotWidth : normalShotWidth;
        Debug.DrawRay(origin, direction * searchRadius, color, 1f / shotsPerSecond);

        StartCoroutine(ShowShotLine(origin, origin + direction * searchRadius, color));
    }

    IEnumerator ShowShotLine(Vector3 from, Vector3 to, Color color)
    {
        LineRenderer lr = gameObject.AddComponent<LineRenderer>();
        lr.startWidth = isBlessed ? blessedShotWidth : normalShotWidth;
        lr.endWidth = lr.startWidth;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = color;
        lr.endColor = color;
        lr.SetPosition(0, from);
        lr.SetPosition(1, to);

        yield return new WaitForSeconds(0.1f);
        Destroy(lr);
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
        isBlessed = true;
        float originalFireRate = shotsPerSecond;
        shotsPerSecond *= multiplier;
        Debug.Log(gameObject.name + " blessed! Fire rate: " + shotsPerSecond);

        yield return new WaitForSeconds(duration);

        shotsPerSecond = originalFireRate;
        isBlessed = false;
        blessCoroutine = null;
        Debug.Log(gameObject.name + " blessing wore off");
    }
}