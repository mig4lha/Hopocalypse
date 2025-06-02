using NUnit.Framework;
using System;
using UnityEngine;

public class SlotMachineController : MonoBehaviour
{
    public Transform dropPoint;
    private Rigidbody rb;
    private bool hasLanded = false;
    private bool hasInteracted = false;

    public float proximityThreshold = 5f; // Adjust this value as needed

    private InteractableZone iz;
    private StatusEffectController statusEffectController;
    private WaveController waveController;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        iz = GetComponent<InteractableZone>();

        if(statusEffectController == null)
        {
            statusEffectController = FindAnyObjectByType<StatusEffectController>();
        }
        if (waveController == null)
        {
            waveController = FindAnyObjectByType<WaveController>();
        }

        // Começa em posição elevada
        transform.position = new Vector3(dropPoint.position.x, 100f, dropPoint.position.z);
    }

    public void Drop()
    {
        // Ativa a física real
        rb.isKinematic = false;
        rb.useGravity = true;
    }


    void OnCollisionEnter(Collision collision)
    {
        if (!hasLanded && collision.gameObject.CompareTag("Ground"))
        {
            hasLanded = true;

            // Corrigido: zera velocidades corretamente usando linearVelocity
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Congela completamente o Rigidbody
            rb.constraints = RigidbodyConstraints.FreezeAll;

            Debug.Log("Slot Machine pousou no chão.");
        }
    }

    public void CheckDistanceAndInteract()
    {
        Debug.Log("Checking distance and interaction...");

        if (iz.playerInside && !hasInteracted)
        {
            // Animação de rodar slot machine

            statusEffectController.ApplyRandomStatusEffect();

            hasInteracted = true;

            Debug.Log("Slot Machine Interacted. Applied Random Effect");

            //StartCoroutine(waveController.LoadEndScene());
            StartCoroutine(waveController.LoadNextLevel());
        } 
    }
}
