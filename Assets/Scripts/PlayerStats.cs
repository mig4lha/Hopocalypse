using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField, Tooltip("Vida do player")]
    public float health = 100f;
    [SerializeField, Tooltip("Valor de velocidade ao andar")]
    public float moveSpeed = 6f;
    [SerializeField, Tooltip("Multiplicador de velocidade de sprint")]
    public float sprintSpeedMultiplier = 1.5f;
    [SerializeField, Tooltip("Valor de for�a do salto")]
    public float jumpForce = 2.5f;
    [SerializeField, Tooltip("Valor de for�a do salto")]
    public float gravity = -80f;

    [Header("Jump Buffer Settings")]
    [SerializeField, Tooltip("Tempo (em segundos) para o jump buffer (coyote time)")]
    public float coyoteTime = 0.1f;

    [Header("Bunny Hop Settings")]
    [SerializeField, Tooltip("Tempo m�ximo entre saltos para considerar como consecutivos")]
    public float consecutiveJumpWindow = 0.4f;
    [SerializeField, Tooltip("Multiplicador m�ximo de velocidade por bunny hopping")]
    public float maxBhopMultiplier = 2f;
    [SerializeField, Tooltip("Aumento de multiplicador por salto consecutivo")]
    public float bhopMultiplierIncrement = 0.1f;
    [SerializeField, Tooltip("Tempo necess�rio no ch�o para iniciar um salto (evita spam de salto)")]
    public float jumpCooldown = 0.1f;
    [SerializeField, Tooltip("Controla o qu�o bem o jogador mant�m a velocidade em curvas durante o bhop")]
    [Range(0f, 1f)]
    public float airControl = 0.7f;
}
