using UnityEngine;
using UnityEngine.InputSystem; // 1. Input Setup: Required for the new Input System (0:55)

public class FlashlightToggle : MonoBehaviour
{
    private Light flashlight; // 2. Reference the Light component (1:57)

    private void Awake()
    {
        // 2. Assign the Light component attached to this GameObject (1:57)
        flashlight = GetComponent<Light>();

        if (flashlight == null)
        {
            Debug.LogWarning("No Light component found on " + gameObject.name);
        }
    }

    private void Update()
    {
        // Don't toggle flashlight if the game is currently paused
        if (PauseManuScript.isPaused) return;

        // 1. Check if the 'F' key was pressed this frame (2:03)
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            ToggleFlashlight();
        }
    }

    private void ToggleFlashlight()
    {
        if (flashlight != null)
        {
            // 3. Flip the enabled state: if true -> false, if false -> true (1:42)
            flashlight.enabled = !flashlight.enabled;
        }
    }
}