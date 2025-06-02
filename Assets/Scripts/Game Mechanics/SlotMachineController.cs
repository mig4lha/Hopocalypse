using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotMachineController : MonoBehaviour
{
    [SerializeField] private SlotMachineIconDisplay iconDisplay;
    [SerializeField] private Animator slotMachineAnimator;
    [SerializeField] private Animator gateAnimator;

    public Transform dropPoint;
    private Rigidbody rb;
    private bool hasLanded = false;
    private bool hasInteracted = false;

    public float proximityThreshold = 5f;

    private InteractableZone iz;
    private StatusEffectController statusEffectController;
    private WaveController waveController;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        iz = GetComponent<InteractableZone>();

        if (statusEffectController == null)
            statusEffectController = FindAnyObjectByType<StatusEffectController>();

        if (waveController == null)
            waveController = FindAnyObjectByType<WaveController>();

        // Inicia em posição elevada
        transform.position = new Vector3(dropPoint.position.x, 100f, dropPoint.position.z);
    }

    public void Drop()
    {
        rb.isKinematic = false;
        rb.useGravity = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!hasLanded && collision.gameObject.CompareTag("Ground"))
        {
            hasLanded = true;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeAll;

            Debug.Log("Slot Machine pousou no chão.");
        }
    }

    public void CheckDistanceAndInteract()
    {
        if (iz.playerInside && !hasInteracted)
        {
            hasInteracted = true;

            slotMachineAnimator.SetTrigger("Spin");
            gateAnimator.SetTrigger("Open");
            iconDisplay.StartSpinning();

            Invoke(nameof(EndSpinAndApplyEffect), 2f); // tempo da animação
        }
    }

    private void EndSpinAndApplyEffect()
    {
        iconDisplay.StopSpinningAndChooseFinal();

        var prize = iconDisplay.GetFinalPrize();
        if (prize != null)
        {
            ApplyFinalIconToMainSquares(prize.icon); // aplica apenas nos MainSquares

            statusEffectController.ApplyStatusEffect(prize.effectData);
            Debug.Log("Efeito aplicado: " + prize.effectData.name);
        }
    }

    private void ApplyFinalIconToMainSquares(Sprite icon)
    {
        for (int i = 1; i <= 3; i++)
        {
            Transform cilindro = GameObject.Find($"Cilindro{i}")?.transform;
            if (cilindro == null)
            {
                Debug.LogWarning($"Cilindro{i} não encontrado.");
                continue;
            }

            Transform mainSquare = cilindro.Find($"MainSquare{i}");
            if (mainSquare == null)
            {
                Debug.LogWarning($"MainSquare{i} não encontrado como filho de Cilindro{i}.");
                continue;
            }

            SpriteRenderer renderer = mainSquare.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.sprite = icon;
            }
            else
            {
                Debug.LogWarning($"MainSquare{i} não possui SpriteRenderer.");
            }
        }
    }
}
