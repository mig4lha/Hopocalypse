using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AITarget : MonoBehaviour
{
    [Header("Player Data")]
    private Player player;
    public float AttackDistance = 3f;

    private NavMeshAgent m_Agent;
    private float m_Distance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
    }

    // Update is called once per frame
    void Update()
    {
        m_Distance = Vector3.Distance(m_Agent.transform.position, player.transform.position);
        if (m_Distance < AttackDistance)
        {
            m_Agent.isStopped = true;
        }
        else
        {
            m_Agent.isStopped = false;
            m_Agent.destination = player.transform.position;
        }
    }
}
