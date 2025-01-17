using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Splines;

public class Car : MonoBehaviour
{
    public List<Transform> waypoints;
    public float speed = 5f;
    public float stopDistance = 1.5f;
    public float stopDistanceCrosswalk = 1.5f;
    public float decelerationRate = 0.5f;
    public float accelerationRate = 0.5f;

    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;
    private Coroutine speedCoroutine;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component is missing on the car.");
            return;
        }

        if (waypoints == null || waypoints.Count == 0)
        {
            Debug.LogError("Waypoints are not assigned to the car.");
            return;
        }

        agent.speed = speed;
        SetBlendedDestination();
    }

    void Update()
    {
        var crosswalk = CrosswalkManager.Instance.GetCrosswalkContainingPosition(transform.position);
        if (crosswalk != null && crosswalk.isWalkable)
        {
            StartSpeedChange(0f);
        }
        else
        {
            StartSpeedChange(speed);
            MoveAlongWaypoints();
        }
    }

    void MoveAlongWaypoints()
    {
        if (waypoints.Count == 0) return;

        if (!agent.pathPending && agent.remainingDistance < stopDistance)
        {
            SetNextWaypoint();
        }

        SetBlendedDestination();
    }

    void SetBlendedDestination()
    {
        if (waypoints.Count < 2) return;

        Transform currentWaypoint = waypoints[currentWaypointIndex];
        Transform nextWaypoint = waypoints[(currentWaypointIndex + 1) % waypoints.Count];

        float distanceToCurrent = Vector3.Distance(transform.position, currentWaypoint.position);
        float totalDistance = Vector3.Distance(currentWaypoint.position, nextWaypoint.position);
        float blendFactor = Mathf.Pow(1f - Mathf.Clamp01(distanceToCurrent / totalDistance), 2f); // Smoother transition at the end

        Vector3 blendedPosition = Vector3.Lerp(currentWaypoint.position, nextWaypoint.position, blendFactor);
        agent.SetDestination(blendedPosition);
    }

    void SetNextWaypoint()
    {
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
    }

    void StartSpeedChange(float targetSpeed)
    {
        if (speedCoroutine != null)
        {
            StopCoroutine(speedCoroutine);
        }
        speedCoroutine = StartCoroutine(ChangeSpeed(targetSpeed));
    }

    IEnumerator ChangeSpeed(float targetSpeed)
    {
        while (Mathf.Abs(agent.speed - targetSpeed) > 0.01f)
        {
            agent.speed = Mathf.MoveTowards(agent.speed, targetSpeed, (targetSpeed > agent.speed ? accelerationRate : decelerationRate) * Time.deltaTime);
            yield return null;
        }

        agent.speed = targetSpeed;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}
