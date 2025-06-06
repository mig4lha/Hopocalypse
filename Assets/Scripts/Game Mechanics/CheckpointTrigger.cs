using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    private WaveController waveController;
    private bool checkpointTriggered = false;

    private void Start()
    {

        waveController = FindAnyObjectByType<WaveController>();
        if (waveController == null)
        {
            Debug.LogError("WaveController not found in the scene.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!checkpointTriggered && other.CompareTag("Player"))
        {
            checkpointTriggered = true;
            Debug.Log("Player has passed through the checkpoint trigger for the first time.");
            waveController.OnCheckpointReached();
        }

    }
}
