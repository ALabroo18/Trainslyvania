using System.Collections;
using UnityEngine;

public class vampireSpawn : MonoBehaviour
{
    //Mason Kuhn

    public GameObject vampirePrefab;

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
                yield return new WaitForSeconds(spawnInterval);
            } 
        }
        spawningDone = true;
    }

    public bool HasReachedMaxSpawns()
    {
        return spawningDone;
    }

    public void SpawnVampire()
    {
        Vector3 spawnPos = GetRandomPointOnBoxEdge();
        GameObject vampire = Instantiate(vampirePrefab, spawnPos, Quaternion.identity);
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
