using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PauseManager : MonoBehaviour
{
    public static bool IsPaused { get; private set; } = false;
    private const string pauseSceneName = "PauseMenu";

    private static PauseManager instance;

    [SerializeField] private InputActionReference pauseAction;
    private Volume pauseBlurVolume;
    private GameObject UI;
    private Coroutine resumeCoroutine;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (pauseBlurVolume == null)
        {
            GameObject blurObj = GameObject.FindWithTag("PauseBlur");
            if (blurObj != null)
            {
                pauseBlurVolume = blurObj.GetComponent<Volume>();
            }
                

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

        if (IsPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }


    public static void PauseGame()
    {
        if (IsPaused) return;

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (instance.pauseBlurVolume != null)
            instance.pauseBlurVolume.enabled = true;


        if (instance.UI != null) {
            instance.UI.SetActive(false);
        } 
        else {
            GameObject uiObject = GameObject.FindWithTag("UI");
            instance.UI = uiObject;
            instance.UI.SetActive(false);
        }

        SceneManager.LoadSceneAsync(pauseSceneName, LoadSceneMode.Additive);
        IsPaused = true;
    }

    public static void ResumeGame()
    {
        if (!IsPaused) return;

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (instance.pauseBlurVolume != null)
            instance.pauseBlurVolume.enabled = false;

        if (instance.UI != null)
        {
            instance.UI.SetActive(true);
        }

        SceneManager.UnloadSceneAsync(pauseSceneName);
        IsPaused = false;

        InputSystem.ResetHaptics();
    }


}
