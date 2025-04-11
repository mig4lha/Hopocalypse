using System;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Enemy : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Player Data")]
    private Player player;

    [Header("Wave Controller")]
    private WaveController waveController;

    [Header("Enemy Stats")]
    [SerializeField] private float health, maxHealth = 100f;
    [SerializeField] private float attackDamage = 5f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float stoppingDistance = 4f;

    [SerializeField, Tooltip("Tempo de delay entre cada ataque")]
    private float fireRateEnemy = 2.0f; // Customizable time between attacks
    private static float nextTimeToFire = 0f;

    [SerializeField] private float jumpForce = 10f;    // Force for the jump
    [SerializeField] private LayerMask groundLayer;    // Layer mask to check for ground
    private bool isGrounded = false;                   // Flag for checking if on ground

    FloatingHealthBar healthBar;
    Transform healthBarTransform;

    public bool isDefeated = false;
    public bool isBoss = false;

 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        waveController = FindAnyObjectByType<WaveController>();

        // Find the canvas first
        Transform canvas = transform.Find("Canvas");

        if (canvas != null)
        {
            // Now find the floating health bar inside the canvas
            healthBarTransform = canvas.Find("healthBar");
            healthBar = healthBarTransform.GetComponent<FloatingHealthBar>();

            if (healthBar != null)
            {
                Debug.Log("Floating Health Bar Component found!");
            }
            else
            {
                Debug.LogWarning("Floating Health Bar not found under Canvas.");
            }
        }
        else
        {
            Debug.LogWarning("Canvas not found.");
        }

        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");

        if (playerGO != null)
        {
            player = playerGO.GetComponent<Player>();
        }
        else
        {
            Debug.LogWarning("Jogador não encontrado! Verifica se tem a tag 'Player'");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        // Ground check (raycast or spherecast)
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.1f, groundLayer);    

        // Direction to the player
        Vector3 direction = player.transform.position - transform.position;
        direction.y = 0; // Ignore vertical difference (optional)

        // Rotate toward the player
        if (direction.magnitude > 0)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

        // Move forward if not too close
        // OR, attack player
        if (direction.magnitude > stoppingDistance)
        {
            Vector3 newPosition = rb.position + transform.forward * moveSpeed * Time.deltaTime;
            rb.MovePosition(newPosition);
        }
        else
        {
            enemyAttack();
        }

        // Handle jump logic (if necessary, can be added to specific obstacle logic)
        if (!isGrounded && direction.magnitude > stoppingDistance)
        {
            // Apply jump when obstacle detected (simplified logic for demonstration)
            TryJumpOverObstacle();
        }
    }

    private void enemyAttack()
    {
        
        // Prevenir tiros antes do delay da fire rate
        if (Time.time < nextTimeToFire)
        {
            //Debug.Log("Enemy NOT attacked");
            
            return;
        }

        nextTimeToFire = Time.time + fireRateEnemy;
        player.TakeDamage(attackDamage);
        

    }

    private void TryJumpOverObstacle()
    {
        // Cast a ray or use a boxcast to detect obstacles ahead
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, stoppingDistance, groundLayer))
        {
            Debug.Log("jump over obstacle");
            // If an obstacle is detected, jump
            if (hit.collider != null && !isGrounded)
            {
                TryJump();
            }
        }
    }


    private void TryJump()
    {
        // If grounded, allow jump
        if (!isGrounded)
        {
            Debug.Log("jump");
            // Apply a force upwards to simulate the jump
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    public void TakeDmg( float amount)
    {
        health -= amount;

        healthBar.UpdateHealthBar(health, maxHealth);

        Debug.Log("Enemy took damage: " + amount + ", current health: " + health);

        if (health <= 0)
        {
            Debug.Log("Enemy defeated!");

            // Mark the enemy as defeated so further pellets don't count it again
            isDefeated = true;

            Destroy(this.gameObject);
            waveController.OnEnemyDefeated();

            //if the enemy defeated is the boss
            if (isBoss)
            {
                waveController.OnBossDefeated();
            }
        }
    }
}
