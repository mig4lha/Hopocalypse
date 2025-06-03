using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonMenus : MonoBehaviour
{
    private GameManager gameManager;

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
            AudioListener.volume = savedVolume;

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
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

}

