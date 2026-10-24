using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Added for the new Input System

public class PauseManuScript : MonoBehaviour
{
    [SerializeField]
    private GameObject pauseMenu;

    public static bool isPaused;

    void Start()
    {
        pauseMenu.SetActive(false);
        isPaused = false;
    }

    void Update()
    {
        // Replaced legacy Input.GetKeyDown with Keyboard.current.escapeKey
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (!isPaused)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
    }

    private void PauseGame()
    {
        pauseMenu.SetActive(true);
        isPaused = true;
        Time.timeScale = 0f;

        // Unlock and show cursor for menu navigation
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        isPaused = false;
        Time.timeScale = 1f;

        // Lock and hide cursor again for first-person gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        // FIXED: Updated to match your exact scene name "Main manu"
        SceneManager.LoadScene("Main menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}