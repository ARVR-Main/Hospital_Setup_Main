using UnityEngine;

public class Drop : MonoBehaviour
{
    [Tooltip("Seconds before the drop auto-destroys")]
    public float lifeTime = 8f;

    [HideInInspector]
    public bool hasHit = false; 

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}