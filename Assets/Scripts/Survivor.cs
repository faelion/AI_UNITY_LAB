using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using TMPro;

public class Survivor : MonoBehaviour
{
    public float safeDistance = 20f;
    public float detectionRadius = 15f;
    public float wanderRadius = 10f;
    public float checkInterval = 0.2f;
    public LayerMask zombieMask;

    public float wanderSpeed = 15f;
    public float evadeSpeed = 15f;
    public float transitionDuration = 0.5f;

    private NavMeshAgent agent;
    private bool isTransitioning = false;
    private NavMeshPath storedPath;

    public float distanceMultiplier = 1f;

    private enum AgentState { Idle, Wandering, Evading }
    private AgentState currentState = AgentState.Idle;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine(CheckForZombies());
        ChangeState(AgentState.Wandering);
    }

    void Update()
    {
        switch (currentState)
        {
            case AgentState.Evading:
                EvadeZombies();
                break;
            case AgentState.Wandering:
                if (!agent.hasPath)
                {
                    SetNewWanderDestination();
                }
                break;
            case AgentState.Idle:
                agent.ResetPath();
                break;
        }
    }

    private IEnumerator CheckForZombies()
    {
        while (true)
        {
            Collider[] zombies = Physics.OverlapSphere(transform.position, detectionRadius, zombieMask);
            bool isZombieNear = false;

            foreach (Collider zombie in zombies)
            {
                float distanceToZombie = Vector3.Distance(transform.position, zombie.transform.position);
                if (distanceToZombie < safeDistance)
                {
                    isZombieNear = true;
                    ChangeState(AgentState.Evading);
                    break;
                }
            }

            if (!isZombieNear && currentState == AgentState.Evading)
            {
                ChangeState(AgentState.Wandering);
            }
            else if (!isZombieNear && currentState == AgentState.Idle)
            {
                ChangeState(AgentState.Wandering);
            }

            yield return new WaitForSeconds(checkInterval);
        }
    }

    private void ChangeState(AgentState newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        switch (currentState)
        {
            case AgentState.Idle:
                agent.speed = 0;
                agent.ResetPath();
                break;
            case AgentState.Wandering:
                agent.speed = wanderSpeed;
                SetNewWanderDestination();
                break;
            case AgentState.Evading:
                agent.speed = evadeSpeed;
                break;
        }
    }

    private void EvadeZombies()
    {
        Collider[] zombies = Physics.OverlapSphere(transform.position, detectionRadius, zombieMask);
        Vector3 evadeDirection = Vector3.zero;

        foreach (Collider zombie in zombies)
        {
            Vector3 directionAwayFromZombie = transform.position - zombie.transform.position;
            evadeDirection += directionAwayFromZombie.normalized;
        }

        evadeDirection = evadeDirection.normalized;
        Vector3 evadeVector = evadeDirection * 5f;

        if (NavMesh.SamplePosition(transform.position + evadeVector, out NavMeshHit navHit, safeDistance, 1 << NavMesh.GetAreaFromName("Walkable")) && NavMesh.CalculatePath(transform.position, transform.position + evadeVector, NavMesh.AllAreas, agent.path) && agent.path.status == NavMeshPathStatus.PathComplete)
        {
            if (!isTransitioning)
            {
                StartCoroutine(SmoothTransition(transform.position + (evadeVector * distanceMultiplier)));
            }
        }
        else
        {
            agent.ResetPath();
            if (!isTransitioning)
            {
                StartCoroutine(SmoothTransition(transform.position + transform.right * 3f - (evadeVector * distanceMultiplier) * 3f));
            }
        }
    }

    private void SetNewWanderDestination()
    {
        Vector3 wanderTarget = GetRandomWanderPosition();
        SetValidDestination(wanderTarget);
    }

    private void SetValidDestination(Vector3 targetPosition)
    {
        const int maxAttempts = 10;
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            if (NavMesh.SamplePosition(targetPosition, out NavMeshHit navHit, safeDistance, 1 << NavMesh.GetAreaFromName("Walkable")))
            {
                if (NavMesh.CalculatePath(transform.position, navHit.position, NavMesh.AllAreas, agent.path))
                {
                    if (!isTransitioning)
                    {
                        StartCoroutine(SmoothTransition(navHit.position));
                    }
                    return;
                }
            }
            else
            {
                targetPosition = GetRandomWanderPosition();
                attempts++;
            }
        }

    }
    private IEnumerator SmoothTransition(Vector3 newDestination)
    {
        isTransitioning = true;
        Vector3 startPosition = agent.destination;
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            Vector3 smoothPosition = Vector3.Lerp(startPosition, newDestination, elapsedTime / transitionDuration);
            agent.SetDestination(smoothPosition);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        agent.SetDestination(newDestination);
        isTransitioning = false;
    }

    private Vector3 GetRandomWanderPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * Random.Range(0f, wanderRadius);
        randomDirection += transform.position;

        return randomDirection;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
    }
}
