using UnityEngine;

public class ImagePopup : MonoBehaviour
{
    public GameObject popupPanel; // The panel holding the background image and text

    public void TogglePopup()
    {
        if (popupPanel != null)
        {
            bool isActive = popupPanel.activeSelf;
            popupPanel.SetActive(!isActive);
        }
    }
}