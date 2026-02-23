using System.Collections;
using UnityEngine;

public class vampireSpawn : MonoBehaviour
{
    //Mason Kuhn

    public GameObject vampirePrefab;
    
    [Header("Train Settings")]
    public Transform trainCenter;
    public Collider trainCollider;

    [Header("Spawn Settings")]
    public float spawnInterval = 1f;
    public int enemiesPerSpawn = 2;

    

    [Header("Spawn Area")]
    public Transform spawnCenter;
    public Vector2 boxSize = new Vector2(30f, 30f); //x (width), z (depth)
    public float spawnHeight = 0f;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            for (int i = 0; i < enemiesPerSpawn; i++)
            {
                SpawnVampire();
                yield return new WaitForSeconds(spawnInterval);
                
            }
            
            
            
        }
    }

    public void SpawnVampire()
    {
        Vector3 spawnPos = GetRandomPointOnBoxEdge();
        GameObject vampire = Instantiate(vampirePrefab, spawnPos, Quaternion.identity);

        Collider vampireCol = vampire.GetComponent<Collider>();
        if (vampireCol != null)
        {
            float bottom = vampireCol.bounds.min.y;
            float offset = 0f - bottom;
            vampire.transform.position += Vector3.up * offset;
        }

        vampireMovement move = vampire.GetComponent<vampireMovement>();
        if (move != null)
        {
            move.target = trainCenter;
            move.targetCollider = trainCollider;
        }
    }

    Vector3 GetRandomPointOnBoxEdge()
    {
        float halfX = boxSize.x / 2f;
        float halfZ = boxSize.y / 2f;

        int side = Random.Range(0, 4);
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
