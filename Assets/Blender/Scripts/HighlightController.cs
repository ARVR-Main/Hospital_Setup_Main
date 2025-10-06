using UnityEngine;

public class HighlightController : MonoBehaviour
{
    public Material highlightMaterial; // assign a glow or highlight material
    private Material originalMaterial;
    private Renderer objRenderer;

    private void Awake()
    {
        objRenderer = GetComponent<Renderer>();
        if (objRenderer != null)
        {
            originalMaterial = objRenderer.material;
        }
    }

    // Turn highlight on or off
    public void Highlight(bool on)
    {
        if (objRenderer == null) return;

        objRenderer.material = on ? highlightMaterial : originalMaterial;
    }
}
