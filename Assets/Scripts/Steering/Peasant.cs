using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Peasant : MonoBehaviour
{
    private NavMeshAgent agent;
    public float wanderRadius = 10.0f;
    public float minWanderInterval = 2.0f; // Tiempo mínimo entre wanderings
    public float maxWanderInterval = 5.0f; // Tiempo máximo entre wanderings
    public float minMoveDistance = 2.0f; // Distancia mínima para moverse

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine(WanderRoutine());
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minMoveDistance);
    }

    IEnumerator WanderRoutine()
    {
        while (true)
        {
            Wander();

            // Generar un tiempo aleatorio dentro del rango
            float waitTime = Random.Range(minWanderInterval, maxWanderInterval);
            yield return new WaitForSeconds(waitTime);
        }
    }

    void Wander()
    {
        Vector3 wanderPosition;
        do
        {
            wanderPosition = RandomNavSphere(transform.position, wanderRadius, -1);
        } while (Vector3.Distance(transform.position, wanderPosition) < minMoveDistance);

        // Consultar el CrosswalkManager
        var crosswalk = CrosswalkManager.Instance.GetCrosswalkContainingPosition(wanderPosition);
        if (crosswalk != null)
        {
            if (crosswalk.isWalkable)
            {
                // Determinar el lado opuesto dependiendo de la posición actual
                Transform oppositeSide = Vector3.Distance(transform.position, crosswalk.sideA.position) <
                                         Vector3.Distance(transform.position, crosswalk.sideB.position)
                    ? crosswalk.sideB
                    : crosswalk.sideA;

                wanderPosition = oppositeSide.position;
            }
            else
            {
                // Cancelar el movimiento si el semáforo está en rojo
                Debug.Log("Peasant: Crosswalk is not walkable");
                return;
            }
        }

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
