using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AITarget : MonoBehaviour
{
    [Header("Player Data")]
    private Player player;

    [Header("Enemy Stats")]
    [SerializeField] protected float health, maxHealth = 100f;
    [SerializeField] protected float attackDamage = 5f;
    [SerializeField] protected float AttackDistance = 4f;
    [SerializeField, Tooltip("Tempo de delay entre cada ataque")]
    protected float fireRateEnemy = 2.0f; // Customizable time between attacks
    protected static float nextTimeToFire = 0f;

    private NavMeshAgent m_Agent;
    private float m_Distance;


    // barra da vida
    FloatingHealthBar healthBar;
    Transform healthBarTransform;

    // coisas para o wave controller / tipo de inimigo
    [Header("Wave Controller")]
    protected WaveController waveController;
    public bool isDefeated = false;
    protected bool isBoss = false;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        // wave controller
        waveController = FindAnyObjectByType<WaveController>();
        //enemy
        m_Agent = GetComponent<NavMeshAgent>();



        //player
        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");

        if (playerGO != null)
        {
            player = playerGO.GetComponent<Player>();
        }
        else
        {
            Debug.LogWarning("Jogador não encontrado! Verifica se tem a tag 'Player'");
        }



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
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (player == null) return;


        m_Distance = Vector3.Distance(m_Agent.transform.position, player.transform.position);
        if (m_Distance < AttackDistance)
        {
            m_Agent.isStopped = true;
            enemyAttack();
        }
        else
        {
            m_Agent.isStopped = false;
            m_Agent.destination = player.transform.position;
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



    public void TakeDmg(float amount)
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
