using UnityEngine;

public class OutlineHighlight : MonoBehaviour
{
    private Renderer[] renderers;
    [Header("Highlight Color")]
    [SerializeField] private Color highlightColor = Color.yellow;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

    public void Highlight()
    {
        foreach (var rend in renderers)
        {
            foreach (var mat in rend.materials)
            {
                if (mat.HasProperty("_Color"))
                {
                    mat.color = highlightColor;
                }
                else if (mat.HasProperty("_BaseColor")) // For URP shaders
                {
                    mat.SetColor("_BaseColor", highlightColor);
                }
            }
        }
    }

    public void UnHighlight()
    {
        foreach (var rend in renderers)
        {
            foreach (var mat in rend.materials)
            {
                if (mat.HasProperty("_Color"))
                {
                    mat.color = Color.white; // Resets back to default white/original
                }
                else if (mat.HasProperty("_BaseColor")) // For URP shaders
                {
                    mat.SetColor("_BaseColor", Color.white);
                }
            }
        }
    }
}