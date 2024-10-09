using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private static GameObject[] hidingSpots;

    public GameObject copPrefab;
    public GameObject robberPrefab;
    public GameObject peasantPrefab;

    public GameObject[] GetHidingSpots()
    {
        return hidingSpots;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        hidingSpots = GameObject.FindGameObjectsWithTag("hide");
        SpawnAgent(copPrefab, "Cop");
        SpawnAgent(robberPrefab, "Robber");
        SpawnAgent(peasantPrefab, "Peasant");
    }

    void SpawnAgent(GameObject agentPrefab, string agentName)
    {
        Vector3 randomPosition = GetRandomPositionOnNavMesh();
        if (randomPosition != Vector3.zero)
        {
            GameObject agent = Instantiate(agentPrefab, randomPosition, Quaternion.identity);
            agent.name = agentName;
        }
        else
        {
            Debug.LogError($"No valid NavMesh position found for {agentName}");
        }
    }

    Vector3 GetRandomPositionOnNavMesh(float range = 50.0f)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(-range, range),
                0,
                Random.Range(-range, range)
            );

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPosition, out hit, range, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        return Vector3.zero;
    }
}

