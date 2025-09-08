using UnityEngine;

public class EyeDropSpawner : MonoBehaviour
{
    public GameObject dropPrefab;       
    public Transform spawnPoint;        
    public float spawnDelay = 0.5f;      
    public float cooldownTime = 1.5f;      
    public float tiltThreshold = 70f;    

    private int dropCount = 0;
    private float nextSpawnTime = 0f;
    private bool onCooldown = false;

    void Update()
    {
       
        float tiltAngle = Vector3.Angle(transform.forward, Vector3.down);

        
        bool isTilted = tiltAngle < tiltThreshold;

        if (isTilted && !onCooldown && Time.time >= nextSpawnTime)
        {
            SpawnDrop();
            dropCount++;

            if (dropCount >= 3) 
            {
                onCooldown = true;
                dropCount = 0;
                nextSpawnTime = Time.time + cooldownTime;
            }
            else
            {
                nextSpawnTime = Time.time + spawnDelay;
            }
        }

       
        if (onCooldown && Time.time >= nextSpawnTime)
        {
            onCooldown = false;
        }
    }

    void SpawnDrop()
    {
        Instantiate(dropPrefab, spawnPoint.position, Quaternion.identity);
    }
}
