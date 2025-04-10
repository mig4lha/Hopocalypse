using UnityEngine;

public class SlotMachineController : MonoBehaviour
{
    public Transform dropPoint;
    public float fallHeight = 10f;
    private Rigidbody rb;
    private bool hasLanded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
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
}
