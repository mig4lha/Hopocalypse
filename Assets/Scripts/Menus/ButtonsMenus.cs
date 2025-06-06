using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonMenus : MonoBehaviour
{
    private GameManager gameManager;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider volumeSlider;
    private void Awake()
    {
        // Mostrar o cursor nos menus
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        gameManager = GameManager.instance;
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene.");
        }

        // Volume inicial
        if (volumeSlider != null)
        {
            float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            volumeSlider.value = savedVolume;
            float dB = Mathf.Log10(Mathf.Clamp(savedVolume, 0.0001f, 1f)) * 20f;
            audioMixer.SetFloat("MasterVolume", dB);

            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    // MAIN MENU
    public void PlayGame()
    {
        if (gameManager != null)
        {
            gameManager.LoadLevel(0);
        }
        else
        {
            Debug.LogWarning("GameManager is null, can't load level.");
        }
    }

    public void OpenOptions()
    {
        SceneManager.LoadScene("Options");
    }

    public void OpenCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void ExitGame()
    {
        Debug.Log("Game closed");
        Application.Quit();
    }

    // Return Main MENU
    public void ReturnToMainMenu()
    {
        Debug.Log("Returning to Main Menu");
        SceneManager.LoadScene("MainMenu");
    }

    // PAUSE MENU
    public void ResumeGame()
    {
        PauseManager.ResumeGame();
    }

    public void ReturnToMainMenuFromPause()
    {
        PauseManager.ResumeGame(); // retoma o jogo antes de mudar de scene
        SceneManager.LoadScene("MainMenu");
    }

    private void SetVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat("MasterVolume", dB);
        PlayerPrefs.SetFloat("MasterVolume", value);
    }
}

