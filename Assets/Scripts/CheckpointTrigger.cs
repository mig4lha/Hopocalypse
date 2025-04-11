using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    private WaveController waveController;
    private bool checkpointTriggered = false;  // Flag to ensure single trigger

    private void Start()
    {
        // Find the WaveController instance in the scene.
        waveController = FindAnyObjectByType<WaveController>();
        if (waveController == null)
        {
            Debug.LogError("WaveController not found in the scene.");
        }
    }

    // Called when another collider enters this trigger collider
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to the player and if the checkpoint hasn't been triggered before.
        if (!checkpointTriggered && other.CompareTag("Player"))
        {
            checkpointTriggered = true;
            Debug.Log("Player has passed through the checkpoint trigger for the first time.");
            waveController.OnCheckpointReached();
        }

    }
}
