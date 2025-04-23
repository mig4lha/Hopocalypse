using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public static bool IsPaused = false;
    private const string pauseSceneName = "PauseMenu";

    private static PauseManager instance;

    // Refer�ncia � a��o "Pause" no Input System (por ex. tecla ESC)
    [SerializeField] private InputActionReference pauseAction;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // j� existe um, destr�i este
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        if (pauseAction != null)
        {
            pauseAction.action.Enable();
            pauseAction.action.performed += TogglePause;
        }
    }

    private void OnDisable()
    {
        if (pauseAction != null)
        {
            pauseAction.action.performed -= TogglePause;
            pauseAction.action.Disable();
        }
    }

    private void TogglePause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!IsPaused)
                PauseGame();
            else
                ResumeGame();
        }
    }

    public static void PauseGame()
    {
        if (SceneManager.GetSceneByName(pauseSceneName).isLoaded) return;

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadSceneAsync(pauseSceneName, LoadSceneMode.Additive);
        IsPaused = true;
    }

    public static void ResumeGame()
    {
        if (!SceneManager.GetSceneByName(pauseSceneName).isLoaded) return;

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SceneManager.UnloadSceneAsync(pauseSceneName);
        IsPaused = false;

        InputSystem.ResetHaptics(); // limpa vibra��o (se usarem gamepad)
        InputSystem.Update();       // for�a o refresh do estado de input
    }
}

