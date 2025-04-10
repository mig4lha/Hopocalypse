using UnityEngine;

public class BossAnimationController : MonoBehaviour
{
    private Animator animator;
    private float idleDuration = 10f;
    private float attackDuration = 1f;
    private float timer = 0f;

    private enum State
    {
        Idle,
        Attacking,
        Frozen
    }

    private State currentState = State.Idle;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        switch (currentState)
        {
            case State.Idle:
                if (timer >= idleDuration)
                {
                    StartAttack();
                }
                break;

            case State.Attacking:
                if (timer >= attackDuration)
                {
                    FreezeOnLastFrame();
                }
                break;

            case State.Frozen:
                break;
        }
    }

    void StartAttack()
    {
        currentState = State.Attacking;
        timer = 0f;

        // Decide qual ataque será realizado (Attack1 ou Attack2)
        //int attackChoice = Random.Range(0, 2); // 0 ou 1 para escolher entre os dois ataques

        animator.SetTrigger("Attack2Trigger");

        /*if (attackChoice == 0)
        {
            animator.SetTrigger("Attack1Trigger");
        }
        else
        {
            animator.SetTrigger("Attack2Trigger");
        }*/
    }

    void FreezeOnLastFrame()
    {
        currentState = State.Frozen;
        timer = 0f;

        animator.speed = 0f; // Congela a animação no último frame
    }
}
