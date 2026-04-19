using UnityEngine;

public class InfiniteSceneSetup : MonoBehaviour
{
    public int defaultTurretCount = 2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (InfiniteTurretPlacer.Instance != null)
            InfiniteTurretPlacer.Instance.PlaceDefaultTurrets(defaultTurretCount);
        else
            Debug.LogWarning("InfiniteTurretPlacer not found!");
    }
}