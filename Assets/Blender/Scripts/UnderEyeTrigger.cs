using UnityEngine;

public class UnderEyeTrigger : MonoBehaviour
{
    public SkinnedMeshRenderer faceMesh;  
    public string blendShapeName = "Key 3";

    private int blendShapeIndex;

    void Start()
    {
        if (faceMesh != null)
        {
            blendShapeIndex = faceMesh.sharedMesh.GetBlendShapeIndex(blendShapeName);

            if (blendShapeIndex < 0)
                Debug.LogError(" Blendshape not found: " + blendShapeName);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (faceMesh != null && blendShapeIndex >= 0)
        {
            Debug.Log(" Finger touched under-eye: " + other.name);
            faceMesh.SetBlendShapeWeight(blendShapeIndex, 100f); 
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (faceMesh != null && blendShapeIndex >= 0)
        {
            faceMesh.SetBlendShapeWeight(blendShapeIndex, 0f); 
        }
    }
}