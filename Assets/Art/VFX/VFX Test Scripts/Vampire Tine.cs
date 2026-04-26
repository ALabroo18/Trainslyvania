using UnityEngine;
using UnityEngine.Serialization;

public class VampireTine : MonoBehaviour
{
    public SkinnedMeshRenderer meshMats;
    public Material dmgMaterial;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Material[] newMats;
        newMats = new Material[meshMats.materials.Length + 1];

        for (int i = 0; i < meshMats.materials.Length; i++)
        {
            newMats[i] = meshMats.materials[i];

            if (i == newMats.Length - 1)
            {
                newMats[i] = dmgMaterial;
            }
        }
    }

    // Sets the mesh material array to include the on fire material
    public void SetFire()
    {
        
    }

    // removes the on fire material from the ar
    public void Extinguish()
    {
        
    }
}
