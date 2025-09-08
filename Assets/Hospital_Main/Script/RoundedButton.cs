// Example: Change Button border look dynamically
using UnityEngine;
using UnityEngine.UI;

public class RoundedButton : MonoBehaviour
{
    public Image buttonImage;

    void Start()
    {
        // Apply rounded sprite
        buttonImage.sprite = Resources.Load<Sprite>("rounded_sprite");
        buttonImage.type = Image.Type.Sliced;
    }
}
