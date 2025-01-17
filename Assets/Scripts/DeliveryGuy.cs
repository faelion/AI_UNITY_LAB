using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DeliveryGuy : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject currentPeasant;
    public float proximityThreshold = 1.5f;
    public float maxTimePerPeasant = 10.0f;

    private float timeWithCurrentPeasant;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timeWithCurrentPeasant = 0f;
    }

    void Update()
    {
        if (currentPeasant == null || timeWithCurrentPeasant >= maxTimePerPeasant)
        {
            GetNextPeasant();
        }
        else if (Vector3.Distance(transform.position, currentPeasant.transform.position) <= proximityThreshold)
        {
            GetNextPeasant();
        }

        timeWithCurrentPeasant += Time.deltaTime;
        MoveToNextPeasant();
    }

    void MoveToNextPeasant()
    {
        if (currentPeasant != null)
        {
            agent.SetDestination(currentPeasant.transform.position);
        }
        else
        {
            Debug.LogWarning("No peasants available to move to.");
        }
    }

    void GetNextPeasant()
    {
        currentPeasant = PeasantManager.Instance.GetRandomPeasant();
        timeWithCurrentPeasant = 0f;
    }
}
