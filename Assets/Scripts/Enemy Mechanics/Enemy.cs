using System;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Enemy : MonoBehaviour
{
    protected Rigidbody rb;

    [Header("Player Data")]
    protected Player player;

    [Header("Wave Controller")]
    protected WaveController waveController;

    [Header("Enemy Stats")]
    [SerializeField] protected float health, maxHealth = 100f;
    [SerializeField] protected float attackDamage = 5f;
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float rotationSpeed = 5f;
    [SerializeField] protected float stoppingDistance = 4f;

    [SerializeField, Tooltip("Tempo de delay entre cada ataque")]
    protected float fireRateEnemy = 2.0f; // Customizable time between attacks
    protected static float nextTimeToFire = 0f;

    [SerializeField] protected float jumpForce = 10f;    // Force for the jump
    [SerializeField] protected LayerMask groundLayer;    // Layer mask to check for ground
    protected bool isGrounded = false;                   // Flag for checking if on ground

    FloatingHealthBar healthBar;
    Transform healthBarTransform;

    public bool isDefeated = false;
    protected bool isBoss = false;

    private Transform safeSpawnTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
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

            if (healthBar == null)
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

        GameObject safeSpawnGO = GameObject.Find("SafeSpawn");
        if (safeSpawnGO != null)
        {
            safeSpawnTransform = safeSpawnGO.transform;
        }
        else
        {
            Debug.LogWarning("SafeSpawn GameObject not found in the scene. Enemies will not be teleported correctly if they fall out.");
        }
    }

    private void TeleportToSafePosition()
    {
        if (safeSpawnTransform != null)
        {
            Debug.Log($"{gameObject.name} fell out of the map. Teleporting to SafeSpawn at {safeSpawnTransform.position}.");
            rb.linearVelocity = Vector3.zero;
            transform.position = safeSpawnTransform.position;
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} fell out of the map, but SafeSpawn is not set.");
        }
    }



    // Update is called once per frame
    protected virtual void Update()
    {
        if (player == null) return;

        if (transform.position.y < -20f) // Adjust threshold as needed
        {
            TeleportToSafePosition();
        }

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

    protected virtual void enemyAttack()
    {
        
        // Prevenir tiros antes do delay da fire rate
        if (Time.time < nextTimeToFire)
        {
            //Debug.Log("Enemy NOT attacked");
            
            return;
        }

        nextTimeToFire = Time.time + fireRateEnemy;

        Debug.Log("Enemy attacked");
        player.TakeDamage(attackDamage);
        

    }

    protected virtual void TryJumpOverObstacle()
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


    protected virtual void TryJump()
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

        if (health <= 0)
        {
            // Mark the enemy as defeated so further pellets don't count it again
            isDefeated = true;

            Destroy(this.gameObject);
            waveController.OnEnemyDefeated();

            //if the enemy defeated is the boss
            if (isBoss)
            {
                Debug.Log("Boss morto");
                waveController.OnBossDefeated();
            }
        }
    }
}
