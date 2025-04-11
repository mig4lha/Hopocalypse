using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private GameManager gameManager;

    private void Awake()
    {
        // Lock e hide do cursor do rato no jogo
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        gameManager = GameManager.instance;
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene.");
        }
    }

    public void OpenCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void PlayGame()
    {
        gameManager.LoadLevel(0);
    }

    public void OpenOptions()
    {
        SceneManager.LoadScene("Options");
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game closed");
    }
}
