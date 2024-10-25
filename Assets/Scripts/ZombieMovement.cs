using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class ZombieMovement : MonoBehaviour
{
    [HideInInspector]
    public Transform player;
    private NavMeshAgent agent;
    public float wanderRadius = 10f;
    public float minWanderInterval = 2f;
    public float maxWanderInterval = 5f;

    public float wanderSpeed = 1f;
    public float chaseSpeed = 2f;

    private enum ZombieState { Idle, Wandering, Chasing }
    private ZombieState currentState = ZombieState.Idle;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("ZombieTarget").transform;
    }

    void Start()
    {
        ChangeState(ZombieState.Wandering);
    }

    void Update()
    {
        if (currentState == ZombieState.Chasing)
        {
            agent.SetDestination(player.position);
        }
    }

    private void ChangeState(ZombieState newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        switch (currentState)
        {
            case ZombieState.Wandering:
                agent.speed = wanderSpeed;
                StartWandering();
                break;
            case ZombieState.Chasing:
                agent.speed = chaseSpeed;
                StopWandering();
                break;
            case ZombieState.Idle:
                StopAllCoroutines();
                agent.ResetPath();
                break;
        }
    }

    public void StartChasing()
    {
        ChangeState(ZombieState.Chasing);
    }

    public void StopChasing()
    {
        ChangeState(ZombieState.Wandering);
    }

    private void StartWandering()
    {
        StartCoroutine(WanderRoutine());
    }

    private void StopWandering()
    {
        StopAllCoroutines();
        agent.ResetPath();
    }

    private IEnumerator WanderRoutine()
    {
        while (currentState == ZombieState.Wandering)
        {
            Vector3 wanderTarget = GetRandomWanderPosition();
            agent.SetDestination(wanderTarget);

            float randomInterval = Random.Range(minWanderInterval, maxWanderInterval);
            yield return new WaitForSeconds(randomInterval);
        }
    }

    private Vector3 GetRandomWanderPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, wanderRadius, -1);
        return navHit.position;
    }

    private void OnPlayerDetected()
    {
        if (currentState != ZombieState.Chasing)
        {
            StartChasing();
        }
    }
}
