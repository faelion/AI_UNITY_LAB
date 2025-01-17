using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PeasantSpawner : MonoBehaviour
{
    public GameObject prefab;
    public int numberOfPrefabs = 10;
    public Vector2 spawnAreaSize = new Vector2(10f, 10f);

    void Start()
    {
        SpawnPrefabs();
    }

    void SpawnPrefabs()
    {
        int spawnedCount = 0;
        int attempts = 0;
        while (spawnedCount < numberOfPrefabs && attempts < numberOfPrefabs * 10)
        {
            Vector3 randomPosition = GetRandomNavMeshPositionWithinArea();
            if (randomPosition != Vector3.zero)
            {
                var peasant = Instantiate(prefab, randomPosition, Quaternion.identity);
                PeasantManager.Instance.RegisterPeasant(peasant);
                spawnedCount++;
            }
            attempts++;
        }

        if (spawnedCount < numberOfPrefabs)
        {
            Debug.LogWarning($"Only {spawnedCount} out of {numberOfPrefabs} prefabs could be spawned.");
        }
    }

    Vector3 GetRandomNavMeshPositionWithinArea()
    {
        float randomX = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
        float randomZ = Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);
        Vector3 randomPosition = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, spawnAreaSize.x / 2, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return Vector3.zero;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnAreaSize.x, 0.1f, spawnAreaSize.y));
    }
}
