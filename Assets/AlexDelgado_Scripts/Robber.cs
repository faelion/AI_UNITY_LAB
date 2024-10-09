using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Robber : MonoBehaviour
{
    public Transform target;
    public Transform peasant;
    private NavMeshAgent agent;

    public float evadePredictionTime = 1.0f;
    public float safeDistance = 10.0f;
    public float randomMovementIntensity = 20.0f;
    public float smoothingFactor = 0.1f;
    public float randomOffsetChangeInterval = 2.0f;

    private Vector3 previousTargetPosition;
    private Vector3 estimatedTargetVelocity;
    private Vector3 currentMoveDirection;  // Dirección actual del movimiento del ladrón
    private Vector3 randomOffset;

    private float timeSinceLastOffsetChange;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("cop").transform;
        peasant = GameObject.FindGameObjectWithTag("peasant").transform;
        previousTargetPosition = target.position;
        currentMoveDirection = transform.forward;

        randomOffset = Random.insideUnitSphere * randomMovementIntensity;
        randomOffset.y = 0;
    }

    void Update()
    {
        estimatedTargetVelocity = (target.position - previousTargetPosition) / Time.deltaTime;
        previousTargetPosition = target.position;

        timeSinceLastOffsetChange += Time.deltaTime;
        if (timeSinceLastOffsetChange > randomOffsetChangeInterval)
        {
            randomOffset = Random.insideUnitSphere * randomMovementIntensity;
            randomOffset.y = 0;
            timeSinceLastOffsetChange = 0f;
        }

        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            Evade();

            if (!coolDown)
            {
                if (CanSeeTargetWithAngles() && CanSeeMeWithAngles())
                {
                    CleverHide();
                    coolDown = true;
                    Invoke("BehaviourCooldown", 5);
                }
                else /*if (distanceToTarget > safeDistance)*/
                {
                    Pursue();
                }
            }
            //if (distanceToTarget < safeDistance)
            //{
            //    Flee();
            //}
            //else
            //{
            //    Evade();
            //}
        }
    }

    void Flee()
    {
        Debug.Log("Robber: Flee");
        Vector3 fleeDirection = (transform.position - target.position).normalized;
        Vector3 fleePosition = transform.position + fleeDirection * safeDistance;

        agent.SetDestination(fleePosition);
    }

    void Evade()
    {
        Debug.Log("Robber: Evade");
        Vector3 fleeDirection = (transform.position - target.position).normalized;

        Vector3 randomOffset = Vector3.zero;
        int maxAttempts = 10;
        for (int i = 0; i < maxAttempts; i++)
        {
            randomOffset = Random.insideUnitSphere * randomMovementIntensity;
            randomOffset.y = 0;

            float angleToTarget = Vector3.Angle(fleeDirection, randomOffset);
            if (angleToTarget > 90f)
            {
                break;
            }
        }

        Vector3 desiredMoveDirection = fleeDirection + randomOffset;

        currentMoveDirection = Vector3.Slerp(currentMoveDirection, desiredMoveDirection, smoothingFactor);

        Vector3 fleePosition = transform.position + (currentMoveDirection.normalized * safeDistance);

        agent.speed = 10f;
        agent.SetDestination(fleePosition);
    }

    void Pursue()
    {
        Debug.Log("Robber: Persue");
        Vector3 futurePosition = peasant.position + estimatedTargetVelocity * evadePredictionTime;
        agent.speed = 2f;
        agent.SetDestination(futurePosition);
    }

    void CleverHide()
    {

        float dist = Mathf.Infinity;
        Vector3 chosenSpot = Vector3.zero;
        Vector3 chosenDir = Vector3.zero;
        GameObject chosenGO = GameManager.Instance.GetHidingSpots()[0];

        // Find the best hiding spot based on distance
        for (int i = 0; i < GameManager.Instance.GetHidingSpots().Length; ++i)
        {

            Vector3 hideDir = GameManager.Instance.GetHidingSpots()[i].transform.position - target.transform.position;
            Vector3 hidePos = GameManager.Instance.GetHidingSpots()[i].transform.position + hideDir.normalized * 10.0f;

            if (Vector3.Distance(transform.position, hidePos) < dist)
            {

                chosenSpot = hidePos;
                chosenDir = hideDir;
                chosenGO = GameManager.Instance.GetHidingSpots()[i];
                dist = Vector3.Distance(transform.position, hidePos);
            }
        }

        Collider hideCol = chosenGO.GetComponent<Collider>();
        Ray backRay = new Ray(chosenSpot, -chosenDir.normalized);
        RaycastHit info;
        float distance = 100.0f;
        hideCol.Raycast(backRay, out info, distance);

        agent.SetDestination(info.point + chosenDir.normalized * 5.0f);
    }

    bool CanSeeTargetWithAngles()
    {
        RaycastHit raycastInfo;
        Vector3 rayToTarget = target.transform.position - this.transform.position;
        float lookAngle = Vector3.Angle(this.transform.forward, rayToTarget);
        if (lookAngle < 60 && Physics.Raycast(this.transform.position, rayToTarget, out raycastInfo))
        {
            if (raycastInfo.transform.gameObject.CompareTag("cop")) return true;
        }
        return false;
    }

    bool CanSeeMeWithAngles()
    {
        Vector3 rayToMe = this.transform.position - target.transform.position;
        float lookAngle = Vector3.Angle(target.transform.forward, rayToMe);
        if (lookAngle < 60)
        {
            return true;
        }
        return false;
    }

    bool coolDown = false;
    void BehaviourCooldown()
    {
        coolDown = false;
    }
}
