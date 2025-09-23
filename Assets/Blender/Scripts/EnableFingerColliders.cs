using UnityEngine;

public class EnableFingerColliders : MonoBehaviour
{
    void Start()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>(true);
        foreach (Collider col in colliders)
        {
            col.enabled = true;
        }
    }
}