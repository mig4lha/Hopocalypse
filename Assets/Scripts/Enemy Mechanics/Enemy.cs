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

    [SerializeField] private float jumpForceEnemy = 0.5f;    // Force for the jump
    [SerializeField] private LayerMask groundLayer;    // Layer mask to check for ground
    private bool isGrounded = false;                   // Flag for checking if on ground

    FloatingHealthBar healthBar;
    Transform healthBarTransform;

    public bool isDefeated = false;
    public bool isBoss = false;

    // temporario apagar depois de teste
    public bool testJump = false;
    private float timeBetweenDoingSomething = 5f;  //Wait 5 seconds after we do something to do something again
    private float timeWhenWeNextDoSomething;  //The next time we do something

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // apagar, Temporario
        timeWhenWeNextDoSomething = Time.time + timeBetweenDoingSomething;



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
            Debug.LogWarning("Jogador n�o encontrado! Verifica se tem a tag 'Player'");
        }
    }

    // Update is called once per frame
    void Update()
    {
       


        if (timeWhenWeNextDoSomething <= Time.time)
        {
            //Do something here
            rb.AddForce(Vector3.up * jumpForceEnemy, ForceMode.Impulse);
            //rb.AddForce(Vector3.up * jumpForceEnemy, ForceMode.VelocityChange);
            //rb.velocity = new Vector3(rb.velocity.x, jumpForceEnemy, rb.velocity.z);
            Debug.Log("jump");
            timeWhenWeNextDoSomething = Time.time + timeBetweenDoingSomething;
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
            rb.AddForce(Vector3.up * jumpForceEnemy, ForceMode.Impulse);
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
                waveController.OnBossDefeated();
            }
        }
    }
}
