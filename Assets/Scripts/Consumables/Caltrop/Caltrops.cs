using UnityEngine;

public class Caltrops : MonoBehaviour
{
    public LayerMask enemyMask;
    public int immediateDamage = 50;
    public int damageOverTime = 50;
    public float dotDuration = 15f;
    public float dotTickRate = 1f;
    [SerializeField] public float radiusNum;
    public GameObject CaltropsZonePrefab;

    [SerializeField] private bool inRadius;

    public void FireRadius(Vector3 worldPosition)
    {
        
        float radius = radiusNum;
        int dot = damageOverTime;
        float duration = dotDuration;

        if (ModeSelector.SelectedMode == GameMode.Infinite && InfiniteUpgradeManager.Instance != null)
        {
            radius += InfiniteUpgradeManager.Instance.firebombRadiusBonus;
            dot += Mathf.RoundToInt(InfiniteUpgradeManager.Instance.firebombDOTBonus);
            duration += InfiniteUpgradeManager.Instance.firebombDurationBonus;
        }

        Collider[] enemies = Physics.OverlapSphere(worldPosition, radiusNum, enemyMask);
        Debug.Log("Caltrop hit " + enemies.Length + " enemies at radius: " + radius);

        Debug.Log("Enemies: " + enemies.Length);
        Debug.Log("In fireradius");

        foreach (Collider enemy in enemies)
        {
            Debug.Log("Inside Enemies");
            if (enemy == null) continue;

            vampireHealth health = enemy.GetComponent<vampireHealth>();
            if (health != null)
            {
                health.TakeDamage(immediateDamage);
                continue;
            }

            InfiniteVampireHealth infiniteHealth = enemy.GetComponent<InfiniteVampireHealth>();
            if (infiniteHealth != null)
                infiniteHealth.TakeDamage(immediateDamage);
        }
        FireZone(worldPosition, radius, dot, duration);
    }

    public void FireZone(Vector3 position, float radius, int dot, float duration)
    {
        
        if (CaltropsZonePrefab != null)
        {
            GameObject zone = Instantiate(CaltropsZonePrefab, position, Quaternion.identity);
            CaltropsZone zoneScript = zone.GetComponent<CaltropsZone>();
            if (zoneScript != null)
            {
                zoneScript.Initialize(radius, enemyMask, dot, dotTickRate, duration);
            }
        }
        else
        {
            GameObject zone = new GameObject("CaltropsZone");
            zone.transform.position = position;
            CaltropsZone zoneScript = zone.AddComponent<CaltropsZone>();
            zoneScript.Initialize(radius, enemyMask, dot, dotTickRate, duration);
            Debug.Log("Hello motherfucker");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == enemyMask)
        {
            vampireHealth health = other.GetComponent<vampireHealth>();
            vampireMovement movement = other.GetComponent<vampireMovement>();
            if (movement != null)
            {
                float originalSpeed = movement.moveSpeed;
                inRadius = true;
                while(inRadius == true)
                {
                    movement.moveSpeed *= 0.25f;
                }
                movement.moveSpeed = originalSpeed;
                
                
            }
        }
    }
    private void OnTriggerExit()
    {
        inRadius = false;
    }
    
}
