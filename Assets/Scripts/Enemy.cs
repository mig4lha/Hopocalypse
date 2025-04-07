using UnityEngine;

public class Enemy : MonoBehaviour
{

    public Player player;
    //Vector3 playerPosition = new Vector3();
    public float moveSpeed = 5f;
    public float rotationSpeed = 5f;
    public float stoppingDistance = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //playerPosition = player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        if (player == null) return;

        // Direction to the player
        Vector3 playerPosition = player.transform.position;

        Debug.Log("Player position: " +  playerPosition);

        Vector3 direction = playerPosition - transform.position;
        direction.y = 0; // Ignore vertical difference (optional)

        Debug.Log("Direction: " + direction);

        // Rotate toward the player
        if (direction.magnitude > 0)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

        // Move forward if not too close
        if (direction.magnitude > stoppingDistance)
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
    }
}
