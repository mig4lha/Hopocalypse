using UnityEngine;

public class Enemy : MonoBehaviour
{

    [Header("Player Data")]
    [SerializeField] private Player player;

    [Header("Enemy Stats")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float stoppingDistance = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (player == null) return;

        // Direction to the player
        Vector3 playerPosition = player.transform.position;
        Vector3 direction = playerPosition - transform.position;
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

    void enemyAttack()
    {
        Debug.Log("Enemy attacked player");
    }
}
