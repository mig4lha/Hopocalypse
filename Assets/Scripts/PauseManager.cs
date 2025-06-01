using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public static bool IsPaused { get; private set; } = false;
    private const string pauseSceneName = "PauseMenu";

    private static PauseManager instance;

    [SerializeField] private InputActionReference pauseAction;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
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
        if (!context.performed) return;

        // Evita pausar se já estiver na PauseMenu
        if (IsPaused) return;

        PauseGame();
    }

    public static void PauseGame()
    {
        if (IsPaused) return;

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadSceneAsync(pauseSceneName, LoadSceneMode.Additive);
        IsPaused = true;
    }

    public static void ResumeGame()
    {
        if (!IsPaused) return;

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SceneManager.UnloadSceneAsync(pauseSceneName);
        IsPaused = false;

        InputSystem.ResetHaptics();
        InputSystem.Update();
    }
}
