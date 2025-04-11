using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsMenuManager : MonoBehaviour
{
    // This method returns to the main menu
    public void ReturnToMainMenu()
    {
        Debug.Log("Returning to Main Menu");
        SceneManager.LoadScene("MainMenu");
    }
}
