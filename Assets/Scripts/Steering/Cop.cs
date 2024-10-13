using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Cop : MonoBehaviour
{
    private GameObject[] waypoints;
    public Transform target;
    private NavMeshAgent agent;

    public float pursuePredictionTime = 1.0f;
    public float smoothingFactor = 0.1f;
    public float safeDistance = 10.0f;

    private Vector3 currentMoveDirection;
    private Vector3 previousTargetPosition;
    private Vector3 estimatedTargetVelocity;

    private Transform currentWaypoint;
    private Transform nextWaypoint;
    private int waypointIndex = 0;
    private bool movingForward = true;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        waypoints = GameObject.FindGameObjectsWithTag("waypoint");
        if (waypoints.Length == 0)
        {
            Debug.LogError("No waypoints have been assigned to the patrol agent.");
            return;
        }

        target = GameObject.FindGameObjectWithTag("robber").transform;
        previousTargetPosition = target.position;

        waypointIndex = Random.Range(0, waypoints.Length);
        movingForward = (Random.value > 0.5f);

        currentWaypoint = waypoints[waypointIndex].transform;
        currentMoveDirection = transform.forward;
        SetNextWaypoint();
    }

    void Update()
    {
        estimatedTargetVelocity = (target.position - previousTargetPosition) / Time.deltaTime;
        previousTargetPosition = target.position;

        if (target != null && Vector3.Distance(target.position, transform.position) < safeDistance && CanSeeTargetWithAngles())
        {
            Pursue();
        }
        else if (currentWaypoint != null)
        {
            Patrol();
        }
    }

    void Seek()
    {
        Debug.Log("Cop: Seek");
        agent.SetDestination(target.position);
    }

    void Pursue()
    {
        Debug.Log("Cop: Persue");
        Vector3 futurePosition = target.position + estimatedTargetVelocity * pursuePredictionTime;
        agent.SetDestination(futurePosition);
    }
    void Patrol()
    {
        Debug.Log("Cop: Patrol");
        Vector3 directionToWaypoint = (currentWaypoint.position - transform.position).normalized;

        currentMoveDirection = Vector3.Slerp(currentMoveDirection, directionToWaypoint, smoothingFactor);
        Vector3 movePosition = transform.position + currentMoveDirection.normalized * agent.speed * Time.deltaTime;
        agent.SetDestination(currentWaypoint.position);

        if (Vector3.Distance(transform.position, currentWaypoint.position) < 0.2f)
        {
            SetNextWaypoint();
        }
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

    bool CanSeeTargetWithAngles()
    {
        RaycastHit raycastInfo;
        Vector3 rayToTarget = target.transform.position - this.transform.position;
        float lookAngle = Vector3.Angle(this.transform.forward, rayToTarget);
        if (lookAngle < 90 && Physics.Raycast(this.transform.position, rayToTarget, out raycastInfo))
        {
            if (raycastInfo.transform.gameObject.CompareTag("robber")) return true;
        }
        return false;
    }
}
