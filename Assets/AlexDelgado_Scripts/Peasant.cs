using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Peasant : MonoBehaviour
{
    private NavMeshAgent agent;
    public float wanderRadius = 10.0f;
    public float wanderInterval = 3.0f;

    private float wanderTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        wanderTimer = wanderInterval; 
    }

    void Update()
    {
        wanderTimer += Time.deltaTime;

        if (wanderTimer >= wanderInterval)
        {
            Wander();
            wanderTimer = 0;
        }
    }

    void Wander()
    {
        Debug.Log("Peasant: Wander");
        Vector3 wanderPosition = RandomNavSphere(transform.position, wanderRadius, -1);

        agent.SetDestination(wanderPosition);
    }

    Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;

        randomDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);

        return navHit.position;
    }
}
