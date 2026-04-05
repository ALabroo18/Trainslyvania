using System.Collections;
using UnityEngine;

public class vampireSpawn : MonoBehaviour
{
    //Mason Kuhn

    [System.Serializable]
    public class VampireType
    {
        public GameObject prefab;
        public float weight = 1f; // chance to spawn
    }

    public VampireType[] vampireTypes;

    [Header("Spawn Settings")]
    public float spawnInterval = 1f;
    public int enemiesPerSpawn = 2;
    public int maxVampires = 30;

    [Header("Spawn Area")]
    public Transform spawnCenter;
    public Vector2 boxSize = new Vector2(30f, 30f); //x (width), z (depth)
    public float spawnHeight = 0f;

    private int totalSpawned = 0;
    private bool spawningDone = false;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (totalSpawned < maxVampires)
        {
            for (int i = 0; i < enemiesPerSpawn; i++)
            {
                if (totalSpawned >= maxVampires) break;
                SpawnVampire();
                Debug.Log(totalSpawned);
            }
            yield return new WaitForSeconds(spawnInterval);
        }
        spawningDone = true;
    }

    GameObject GetRandomVampirePrefab()
    {
        float totalWeight = 0f;

        foreach (var type in vampireTypes)
            totalWeight += type.weight;

        float random = Random.Range(0, totalWeight);

        foreach (var type in vampireTypes)
        {
            if (random < type.weight)
                return type.prefab;

            random -= type.weight;
        }

        return vampireTypes[0].prefab;
    }

    public bool HasReachedMaxSpawns()
    {
        return spawningDone;
    }

    public void SpawnVampire()
    {
        Vector3 spawnPos = GetRandomPointOnBoxEdge();
        GameObject prefab = GetRandomVampirePrefab();
        GameObject vampire = Instantiate(prefab, spawnPos, Quaternion.identity);
        totalSpawned++;

        Collider vampireCol = vampire.GetComponent<Collider>();
        if (vampireCol != null)
        {
            float bottom = vampireCol.bounds.min.y;
            float offset = - bottom;
            vampire.transform.position += Vector3.up * offset;
        }
    }

    Vector3 GetRandomPointOnBoxEdge()
    {
        float halfX = boxSize.x / 4f;
        float halfZ = boxSize.y / 4f;

        int side = 2;
        float x = 0f;
        float z = 0f;

        switch (side)
        {
            case 0: //top edge
                x = Random.Range(-halfX, halfX);
                z = halfZ;
                break;

            case 1: //bottom edge
                x = Random.Range(-halfX, halfX);
                z = -halfZ;
                break;

            case 2: //left edge
                x = -halfX;
                z = Random.Range(-halfZ, halfZ);
                break;

            case 3: //right edge
                x = halfX;
                z = Random.Range(-halfZ, halfZ);
                break;
        }

        return new Vector3(
            spawnCenter.position.x + x,
            spawnHeight,
            spawnCenter.position.z + z
        );
    }
}
