using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [Header("Player Data")]
    private Player player;

    [Header("Enemy Stats")]
    [SerializeField] private float health = 100f;
    [SerializeField] private float attackDamage = 5f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float stoppingDistance = 4f;

    [SerializeField, Tooltip("Tempo de delay entre cada ataque")]
    private float fireRateEnemy = 2.0f; // Customizable time between attacks
    private static float nextTimeToFire = 0f;

    public bool isDefeated = false;
    public bool isBoss = false;

 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
        else
        {
            enemyAttack();
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

    public void enemyTakeDmg( float amount)
    {
        health -= amount;

        if (health <= 0)
        {
            isDefeated = true;
        }
    }
}
