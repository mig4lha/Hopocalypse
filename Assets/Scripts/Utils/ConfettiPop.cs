using UnityEngine;
using UnityEngine.SceneManagement;

public class ConfettiPop : MonoBehaviour
{
    public ParticleSystem confettiSystem;
    public AudioSource audioSource;
    private bool soundPlayed = false;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        if (confettiSystem != null && confettiSystem.isEmitting && !soundPlayed)
        {
            audioSource.Play();
            soundPlayed = true;
        }
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
