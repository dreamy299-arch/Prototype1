using System.Collections;
using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip footstepClip;
    public float stepInterval = 0.35f;

    private FPController playerController;

    private void Start()
    {
        // Get reference to the player's movement script (2:50)
        playerController = GetComponent<FPController>();

        // Start the footstep loop coroutine
        StartCoroutine(play_footsteps());
    }

    // Coroutine to handle timed footstep audio (1:36)
    private IEnumerator play_footsteps()
    {
        // Infinite loop to continuously check movement (1:40)
        while (true)
        {
            // Check if player reference exists, game is not paused, movement magnitude > 0.1, and player is grounded (2:15, 3:31)
            if (playerController != null && 
                !PauseManuScript.isPaused && 
                playerController.moveInput.magnitude > 0.1f && 
                playerController.controller.isGrounded)
            {
                // Play audio clip through AudioManager singleton (2:27)
                if (AudioManager.instance != null && footstepClip != null)
                {
                    AudioManager.instance.play_sfx(footstepClip);
                }

                // Control interval between footsteps (2:32)
                yield return new WaitForSeconds(stepInterval);
            }
            else
            {
                // Wait for the next frame if player is stationary or in the air
                yield return null;
            }
        }
    }
}
