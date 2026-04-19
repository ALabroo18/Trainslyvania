using UnityEngine;
using System.Collections.Generic;

public class InfiniteTurretPlacer : MonoBehaviour
{
    public static InfiniteTurretPlacer Instance;

    [Header("Turret Prefab")]
    public GameObject turretPrefab;

    [Header("Train Collider")]
    public Collider trainCollider;

    [Header("Placement Settings")]
    public float turretHeightOffset = 0.1f;
    public float minSpacingBetweenTurrets = 0.1f;

    private List<GameObject> placedTurrets = new List<GameObject>();
    private List<Vector3> placedPositions = new List<Vector3>();
    public int PlacedTurretCount => placedTurrets.Count;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void PlaceTurret()
    {
        if (turretPrefab == null)
        {
            Debug.LogWarning("No turret prefab assigned!");
            return;
        }
        if (trainCollider == null)
        {
            Debug.LogWarning("No train collider assigned!");
            return;
        }

        Vector3 spawnPos = GetRandomTopPosition();
        if (spawnPos == Vector3.zero)
        {
            Debug.LogWarning("Could not find valid placement position!");
            return;
        }

        GameObject turret = Instantiate(turretPrefab, spawnPos, Quaternion.identity);
        placedTurrets.Add(turret);
        placedPositions.Add(spawnPos);

        trainHealth health = trainCollider.GetComponent<trainHealth>();
        if (health == null)
            health = trainCollider.GetComponentInParent<trainHealth>();
        if (health != null)
        {
            mediumTurret turretScript = turret.GetComponent<mediumTurret>();
            if (turretScript != null)
                turretScript.owningCar = health;
        }

        Debug.Log("Turret placed at: " + spawnPos + " total turrets: " + placedTurrets.Count);
    }

    public void PlaceMultipleTurrets(int count)
    {
        for (int i = 0; i < count; i++)
            PlaceTurret();
    }

    Vector3 GetRandomTopPosition()
    {
        Bounds bounds = trainCollider.bounds;

        float topY = bounds.max.y + turretHeightOffset;
        float minX = bounds.min.x;
        float maxX = bounds.max.x;
        float minZ = bounds.min.z;
        float maxZ = bounds.max.z;

        for (int attempt = 0; attempt < 20; attempt++)
        {
            float x = Random.Range(minX, maxX);
            float z = Random.Range(minZ, maxZ);
            Vector3 candidate = new Vector3(x, topY, z);

            bool tooClose = false;
            foreach (Vector3 existing in placedPositions)
            {
                if (Vector3.Distance(candidate, existing) < minSpacingBetweenTurrets)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
                return candidate;
        }

        float fallbackX = Random.Range(minX, maxX);
        float fallbackZ = Random.Range(minZ, maxZ);
        return new Vector3(fallbackX, topY, fallbackZ);
    }

    public void PlaceDefaultTurrets(int count = 2)
    {
        PlaceMultipleTurrets(count);
    }
}