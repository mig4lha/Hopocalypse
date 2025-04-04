using System;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField, Tooltip("Vida do player")]
    private float health = 100f;
    [SerializeField, Tooltip("Valor de velocidade ao andar")]
    private float moveSpeed = 6f;
    [SerializeField, Tooltip("Multiplicador de velocidade de sprint")]
    private float sprintSpeedMultiplier = 1.5f;
    [SerializeField, Tooltip("Valor de força do salto")]
    private float jumpForce = 2.5f;
    [SerializeField, Tooltip("Valor de força do salto")]
    private float gravity = -80f;

    // Permite ao player saltar mesmo depois de sair do chão, basicamente facilita platforming no futuro
    // Valor default, deve ainda ser testado
    [SerializeField, Tooltip("Tempo (em segundos) para o jump buffer (coyote time)")]
    public float coyoteTime = 0.1f;

    [Header("Bunny Hop Settings")]
    [SerializeField, Tooltip("Tempo máximo entre saltos para considerar como consecutivos")]
    private float consecutiveJumpWindow = 0.4f;
    [SerializeField, Tooltip("Multiplicador máximo de velocidade por bunny hopping")]
    private float maxBhopMultiplier = 2f;
    [SerializeField, Tooltip("Aumento de multiplicador por salto consecutivo")]
    private float bhopMultiplierIncrement = 0.1f;
    [SerializeField, Tooltip("Tempo necessário no chão para iniciar um salto (evita spam de salto)")]
    private float jumpCooldown = 0.1f;
    [SerializeField, Tooltip("Controla o quão bem o jogador mantém a velocidade em curvas durante o bhop")]
    [Range(0f, 1f)] private float airControl = 0.7f;

    // Player variables
    public CharacterController characterController;
    private Vector3 velocity;
    public Vector2 moveInput;
    public float coyoteTimeCounter;
    public bool isSprinting = false;
    public bool isMoving;


    // Hop variables
    private float currentBhopMultiplier = 1.0f;
    private float lastJumpTime;
    private int consecutiveJumps = 0;
    public bool wasGrounded;
    private float timeSinceGrounded;
    private float landingTime;
    private Vector3 moveDirection;

    [SerializeField]
    private AxeGunController axeGunController;

    // Debug UI variables
    [SerializeField]
    private TMP_Text currentSpeedText;
    [SerializeField]
    private TMP_Text currentHopMultiplierText;
    [SerializeField]
    private TMP_Text currentConsecutiveJumpsText;

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        health = Mathf.Min(health + amount, 100f);
    }

    public void Shoot()
    {
        axeGunController.Shoot();
    }

    public void Die()
    {
        Debug.Log("Player has died.");
    }

    public void HandleMovement()
    {
        // Guardar o ultimo time que o player tocou no chao
        if (characterController.isGrounded)
        {
            timeSinceGrounded = 0;
            if (!wasGrounded)
            {
                landingTime = Time.time;
            }
        }
        else
        {
            timeSinceGrounded += Time.deltaTime;
        }

        // Diminui o coyote time counter over time a cada frame
        if (coyoteTimeCounter > 0) { coyoteTimeCounter -= Time.deltaTime; }

        // Calcular base speed com sprint multiplier
        float baseSpeed = isSprinting ? moveSpeed * sprintSpeedMultiplier : moveSpeed;

        // Aplicar o multiplicador de hop atual SE O MULTIPLIER FOR MAIOR QUE 1 ( fiquei preso no chão sem entender porque :) )
        float currentSpeed = baseSpeed * currentBhopMultiplier;

        // Criar vetor de movimento
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        // Normalizar para que diagonais não sejam mais rápidas senão fica estranho de controlar
        if (move.magnitude > 1f)
            move.Normalize();

        // Calcular direção do movimento com velocidade atual
        moveDirection = move * currentSpeed;

        // Testes para melhorar controlo do hop, tenho de validar melhor....
        if (!characterController.isGrounded)
        {
            // Preserva algum momentum da velocidade durante o hop ao interpolar entre a velocidade atual e a direção do movimento
            moveDirection = Vector3.Lerp(velocity, moveDirection, airControl * Time.deltaTime * 10f);
            moveDirection.y = 0; // Manter apenas o componente horizontal
        }

        // Processar o salto APENAS se estiver no chão e o tempo desde o último salto for maior que o cooldown definido!
        bool canJump = characterController.isGrounded && (Time.time - landingTime) >= jumpCooldown;
        if ((canJump || timeSinceGrounded <= coyoteTime) && coyoteTimeCounter > 0)
        {
            // Aplicar força de salto
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            coyoteTimeCounter = 0; // Resetar o coyote time
            lastJumpTime = Time.time;

            // Incrementar contagem de saltos consecutivos (dentro do range cooldown)
            if (Time.time - lastJumpTime < consecutiveJumpWindow)
            {
                consecutiveJumps++;
            }
            else
            {
                consecutiveJumps = 1; // Resetar para 1 se o timing não foi consecutivo
            }
        }

        // Gravidade
        velocity.y += gravity * Time.deltaTime;

        // Combinar movimento horizontal e vertical
        Vector3 finalMove = new Vector3(moveDirection.x, velocity.y, moveDirection.z) * Time.deltaTime;
        characterController.Move(finalMove);

        isMoving = characterController.velocity.magnitude > 0.1f;

        UpdateMovementUI();

        // Atualizar velocity para manter o momentum horizontal
        velocity = new Vector3(moveDirection.x, velocity.y, moveDirection.z);
    }

    public void HandleBunnyHopping()
    {
        // Reseta saltos e hop mult se o player nao saltar durante o tempo definido
        if (Time.time - lastJumpTime > consecutiveJumpWindow * 2)
        {
            currentBhopMultiplier = 1;
            consecutiveJumps = 0;
        }

        // Calcular hop multuplier consoante o num de saltos consecutivos
        if (consecutiveJumps > 1)
        {
            // Limitar o aumento do multiplier com base no maxBhopMultiplier
            float targetMultiplier = 1.0f + (consecutiveJumps - 1) * bhopMultiplierIncrement;
            targetMultiplier = Mathf.Min(targetMultiplier, maxBhopMultiplier);

            // Interpolar o multiplier gradualmente (se for isntantâneo, o bhop fica muito rígido e difícil de controlar)
            currentBhopMultiplier = Mathf.Lerp(currentBhopMultiplier, targetMultiplier, 0.5f);
        }

        // Bonus effects para pensar no futuro
        if (currentBhopMultiplier > 1.3f)
        {
            // Pensar em bonus de gameplay para persuadir o player a saltar...
        }

        // Debug log, retirar na build final!
        if (consecutiveJumps > 0)
        {
            //Debug.Log($"BHop: {consecutiveJumps} jumps, {currentBhopMultiplier:F2}x multiplier");
        }
    }

    private void UpdateMovementUI()
    {
        // Update no UI
        currentSpeedText.text = $"Speed: {characterController.velocity.magnitude:F2} m/s";
        currentHopMultiplierText.text = $"Hop Mult: {currentBhopMultiplier:F2}x";
        currentConsecutiveJumpsText.text = $"Jumps: {consecutiveJumps}";
    }
}
