using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // Player variables
    internal Player player;
    internal CharacterController characterController;
    internal PlayerInput playerInput;
    private Vector3 velocity;
    internal Vector2 moveInput;
    internal float coyoteTimeCounter;
    internal bool isSprinting = false;
    private bool isMoving;
    public bool isDead = false;

    // Hop variables
    private float currentBhopMultiplier = 1.0f;
    private float lastJumpTime;
    private int consecutiveJumps = 0;
    internal bool wasGrounded;
    private float timeSinceGrounded;
    private float landingTime;
    private Vector3 moveDirection;

    // Buffs
    //public bool hasReloadBuff = false;

    public bool testing = false;

    [Header("AxeGun Data")]
    [SerializeField]
    private AxeGunController axeGunController;

    [Header("UI Controller Data")]
    [SerializeField]
    private UIController UIController;

    [Header("Wave Controller Data")]
    [SerializeField]
    private WaveController waveController;

    [Header("Player Stats Data")]
    public PlayerStats stats;

    [Header("Status Effect Controller Data")]
    public StatusEffectController statusEffectController;

    [Header("Flashlight Data")]
    [SerializeField, Tooltip("Flashlight GameObject")]
    private Light flashLight;

    private SlotMachineController slotMachine;

    // public float batteryLife = 100f;
    // public float drainRate = 5f;

    private void Awake()
    {
        if (stats == null)
        {
            stats = FindAnyObjectByType<PlayerStats>();
        }

        if (statusEffectController == null)
        {
            statusEffectController = FindAnyObjectByType<StatusEffectController>();
        }

        if(playerInput == null)
        {
            playerInput = GetComponent<PlayerInput>();
        }

        if (player == null)
        {
            player = GetComponent<Player>();
        }

        if (slotMachine == null)
        {
            slotMachine = FindAnyObjectByType<SlotMachineController>();
        }
    }

    public void TakeDamage(float damage)
    {
        if(isDead) return;

        stats.health -= damage;
        //UIController.UpdateHealthBar(stats.health, stats.maxHealth);
        UIController.UpdateHealthText(stats.health, stats.maxHealth);
        Debug.Log("Player took damage: " + damage + ", Health: " + stats.health);

        if (stats.health <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        stats.health = Mathf.Min(stats.health + amount, 100f);
    }

    public void Shoot()
    {
        axeGunController.Shoot();
    }

    public void Die()
    {
        isDead = true;
        playerInput.enabled = false;

        player.transform.Rotate(0, 0, -90);

        waveController.OnGameOver();
        Debug.Log("Player has died.");
    }

    public void HandleMovement()
    {
        // Guardar o ultimo tempo que o player tocou no chao
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
        float baseSpeed = isSprinting ? stats.moveSpeed * stats.sprintSpeedMultiplier : stats.moveSpeed;

        // Aplicar o multiplicador de hop atual SE O MULTIPLIER FOR MAIOR QUE 1 ( fiquei preso no ch�o sem entender porque :) )
        float currentSpeed = baseSpeed * currentBhopMultiplier;

        // Criar vetor de movimento
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        // Normalizar para que diagonais n�o sejam mais r�pidas sen�o fica estranho de controlar
        if (move.magnitude > 1f)
            move.Normalize();

        // Calcular dire��o do movimento com velocidade atual
        moveDirection = move * currentSpeed;

        // Testes para melhorar controlo do hop, tenho de validar melhor....
        if (!characterController.isGrounded)
        {
            // Preserva algum momentum da velocidade durante o hop ao interpolar entre a velocidade atual e a dire��o do movimento
            moveDirection = Vector3.Lerp(velocity, moveDirection, stats.airControl * Time.deltaTime * 10f);
            moveDirection.y = 0; // Manter apenas o componente horizontal
        }

        // Processar o salto APENAS se estiver no ch�o e o tempo desde o ultimo salto for maior que o cooldown definido!
        bool canJump = characterController.isGrounded && (Time.time - landingTime) >= stats.jumpCooldown;
        if ((canJump || timeSinceGrounded <= stats.coyoteTime) && coyoteTimeCounter > 0)
        {
            // Aplicar forca de salto
            velocity.y = Mathf.Sqrt(stats.jumpForce * -2f * stats.gravity);
            coyoteTimeCounter = 0; // Resetar o coyote time
            lastJumpTime = Time.time;

            // Incrementar contagem de saltos consecutivos (dentro do range cooldown)
            if (Time.time - lastJumpTime < stats.consecutiveJumpWindow)
            {
                consecutiveJumps++;
            }
            else
            {
                consecutiveJumps = 1; // Resetar para 1 se o timing n�o foi consecutivo
            }
        }

        // Gravidade
        velocity.y += stats.gravity * Time.deltaTime;

        // Combinar movimento horizontal e vertical
        Vector3 finalMove = new Vector3(moveDirection.x, velocity.y, moveDirection.z) * Time.deltaTime;
        characterController.Move(finalMove);

        isMoving = characterController.velocity.magnitude > 0.1f;

        UIController.UpdateMovementUI(characterController.velocity.magnitude, currentBhopMultiplier, consecutiveJumps);

        // Atualizar velocity para manter o momentum horizontal
        velocity = new Vector3(moveDirection.x, velocity.y, moveDirection.z);
    }

    public void HandleBunnyHopping()
    {
        // Reseta saltos e hop mult se o player nao saltar durante o tempo definido
        if (Time.time - lastJumpTime > stats.consecutiveJumpWindow * 2)
        {
            currentBhopMultiplier = 1;
            consecutiveJumps = 0;
        }

        // Calcular hop multuplier consoante o num de saltos consecutivos
        if (consecutiveJumps > 1)
        {
            // Limitar o aumento do multiplier com base no maxBhopMultiplier
            float targetMultiplier = 1.0f + (consecutiveJumps - 1) * stats.bhopMultiplierIncrement;
            targetMultiplier = Mathf.Min(targetMultiplier, stats.maxBhopMultiplier);

            // Interpolar o multiplier gradualmente (se for isntant�neo, o bhop fica muito r�gido e dif�cil de controlar)
            currentBhopMultiplier = Mathf.Lerp(currentBhopMultiplier, targetMultiplier, 0.5f);
        }

        // Retirado porque agora é um status effect no gamble
        //if (currentBhopMultiplier >= 2f)
        //{
        //    hasReloadBuff = true;
        //}
        //else
        //{
        //    hasReloadBuff = false;
        //}
    }

    // Literalmente uma test interaction
    internal void TestInteraction()
    {
        //if (!testing)
        //{
        //    statusEffectController.ApplyStatusEffect(statusEffectController.ReloadBoost);
        //    testing = true;
        //}
        //else
        //{
        //    statusEffectController.RemoveStatusEffect(statusEffectController.ReloadBoost);
        //    testing = false;
        //}

        statusEffectController.ApplyRandomStatusEffect();
    }

    public void ToggleFlashlight()
    {
        flashLight.enabled = !flashLight.enabled;

        Debug.Log("Flashlight toggled: " + flashLight.enabled);

        // Achei chato dar drain de bateria neste contexto...
        //if(flashLight.enabled)
        //{
        //    batteryLife -= drainRate * Time.deltaTime;
        //    if(batteryLife <= 0)
        //    {
        //        batteryLife = 0;
        //        flashLight.enabled = false;
        //    }
        //}
    }

    internal void Interact()
    {
        slotMachine.CheckDistanceAndInteract();
    }
}
