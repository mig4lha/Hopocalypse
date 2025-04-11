using UnityEngine;
using UnityEngine.SceneManagement;

public class ConfettiPop : MonoBehaviour
{
    // Reference to your confetti Particle System
    public ParticleSystem confettiSystem;

    // Reference to an AudioSource that has your sound clip
    public AudioSource audioSource;

    // Ensure the sound is played only once
    private bool soundPlayed = false;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        // When the particle system is emitting and sound hasn't been played yet...
        if (confettiSystem != null && confettiSystem.isEmitting && !soundPlayed)
        {
            // Play the sound (you can also use PlayOneShot if you prefer)
            audioSource.Play();
            soundPlayed = true;
        }
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
