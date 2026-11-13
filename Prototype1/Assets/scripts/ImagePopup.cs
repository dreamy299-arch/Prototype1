using UnityEngine;

public class ImagePopup : MonoBehaviour
{
    public GameObject popupImageObject; // Drag your TutorialPopupImage here

    public void TogglePopup()
    {
        if (popupImageObject != null)
        {
            bool isActive = popupImageObject.activeSelf;
            popupImageObject.SetActive(!isActive);
        }
    }
}