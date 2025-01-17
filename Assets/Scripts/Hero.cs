using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Hero : MonoBehaviour
{
    private GameObject[] waypoints;
    private NavMeshAgent agent;
    private Transform currentWaypoint;
    private int waypointIndex = 0;
    private bool movingForward = true;
    public float detectionRadius = 5.0f;
    public LayerMask zombieLayer;
    public float speed = 5.0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;

        waypoints = GameObject.FindGameObjectsWithTag("waypoint");
        if (waypoints.Length == 0)
        {
            Debug.LogError("No waypoints have been assigned to the patrol agent.");
            return;
        }

        waypointIndex = Random.Range(0, waypoints.Length);
        movingForward = (Random.value > 0.5f);
        currentWaypoint = waypoints[waypointIndex].transform;
        SetNextWaypoint();
    }

    void Update()
    {
        Patrol();
    }

    void Patrol()
    {
        Vector3 avoidanceDirection = AvoidZombies();
        Vector3 targetPosition = currentWaypoint.position + avoidanceDirection;
        agent.SetDestination(targetPosition);

        if (Vector3.Distance(transform.position, currentWaypoint.position) < 2f)
        {
            SetNextWaypoint();
        }
    }

    Vector3 AvoidZombies()
    {
        Collider[] zombies = Physics.OverlapSphere(transform.position, detectionRadius, zombieLayer);
        Vector3 avoidanceVector = Vector3.zero;

        foreach (Collider zombie in zombies)
        {
            Vector3 directionAwayFromZombie = transform.position - zombie.transform.position;
            avoidanceVector += directionAwayFromZombie.normalized / directionAwayFromZombie.magnitude;
        }

        return avoidanceVector;
    }

    void SetNextWaypoint()
    {
        if (movingForward)
        {
            waypointIndex++;
            if (waypointIndex >= waypoints.Length)
            {
                waypointIndex = waypoints.Length - 1;
                movingForward = false;
            }
        }
        else
        {
            waypointIndex--;
            if (waypointIndex < 0)
            {
                waypointIndex = 0;
                movingForward = true;
            }
        }

        currentWaypoint = waypoints[waypointIndex].transform;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
