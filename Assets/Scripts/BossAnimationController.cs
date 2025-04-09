using UnityEngine;

public class BossAnimationController : MonoBehaviour
{
    private Animator animator;
    private float idleDuration = 10f;
    private float freezeDuration = 5f;
    private float attackDuration = 1f; // Tempo que leva para executar o ataque (121f -> 130f)

    private float timer = 0f;
    private State currentState = State.Idle;

    private enum State
    {
        Idle,
        Attacking,
        Frozen
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.Play("Take001", 0, 0f);  // Começa no frame 0 (Idle)
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
                if (timer >= freezeDuration)
                {
                    ReturnToIdle();
                }
                break;
        }
    }

    void StartAttack()
    {
        currentState = State.Attacking;
        timer = 0f;

        float normalizedStart = 121f / 130f;
        animator.speed = 0f; // Toca normalmente
        animator.Play("Take001", 0, normalizedStart);
    }

    void FreezeOnLastFrame()
    {
        currentState = State.Frozen;
        timer = 0f;

        animator.Play("Take001", 0, 1f); // Frame final
        animator.speed = 0f; // Congela no último frame
    }

    void ReturnToIdle()
    {
        currentState = State.Idle;
        timer = 0f;

        animator.speed = 1f;
        animator.Play("Take001", 0, 0f); // Recomeça do Idle (frame 0)
    }
}
