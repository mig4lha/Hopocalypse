using UnityEngine;

public class Boss : Enemy
{
    protected override void Start()
    {
        base.Start();
        isBoss = true;
    }

    protected override void Update()
    {
        // Direction to the player
        Vector3 direction = player.transform.position - transform.position;
        direction.y = 0; // Ignore vertical difference (optional)

        // Rotate toward the player
        if (direction.magnitude > 0)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

        // implementar attack pattern pro boss
    }

}
