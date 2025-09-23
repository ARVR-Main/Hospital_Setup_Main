using UnityEngine;

public class EyeDropReceiver : MonoBehaviour
{
    public EyeController eyeController;
    public bool isLeftEye; 

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EyeDrop"))
        {
            if (eyeController != null)
            {
                if (isLeftEye) eyeController.RegisterDropLeft();
                else eyeController.RegisterDropRight();
            }
            Destroy(other.gameObject);
        }
    }
}
