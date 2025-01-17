using BBUnity.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.GlobalIllumination;

public class Crosswalk
{
    public Transform center;
    public float containRadius;
    public float stopRadius;
    public Transform sideA;
    public Transform sideB;
    public bool isWalkable;
    public GameObject prefab;
    public GameObject obstacle;
    public NavMeshObstacle navMeshObstacle;
    public Light carLight;
    public Light peasantLight;

    public void ToggleObstacle()
    {
        if (navMeshObstacle != null)
        {
            isWalkable = !isWalkable;
            navMeshObstacle.enabled = !isWalkable;
            if(isWalkable)
            {
                carLight.color = Color.red;
                peasantLight.color = Color.green;
            }
            else
            {
                carLight.color = Color.green;
                peasantLight.color = Color.red;
            }
            Debug.Log("Obstacle: " + obstacle.name + " is now " + (navMeshObstacle.enabled ? "enabled" : "disabled") + "(Crosswalk)");
            Debug.Log("Is walkable: " + isWalkable);
        }
        else
        {
            Debug.LogWarning("NavMeshObstacle no encontrado en el prefab del crosswalk: " + prefab.name);
        }
    }
}

public class CrosswalkManager : MonoBehaviour
{
    public static CrosswalkManager Instance;

    public List<Crosswalk> crosswalks = new List<Crosswalk>();

    public float minCycleTime = 5.0f; // Tiempo mínimo entre cambios de semáforo
    public float maxCycleTime = 10.0f; // Tiempo máximo entre cambios de semáforo

    public float containRadius = 10.0f;
    public float stopRadius = 5.0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        foreach(var crossingroad in GameObject.FindGameObjectsWithTag("crosswalk"))
        {
            Crosswalk crosswalk = new Crosswalk();
            crosswalk.center = crossingroad.transform;
            crosswalk.containRadius = containRadius;
            crosswalk.stopRadius = stopRadius;
            crosswalk.sideA = crossingroad.transform.Find("side_a");
            crosswalk.sideB = crossingroad.transform.Find("side_b");
            crosswalk.isWalkable = true;
            crosswalk.prefab = crossingroad;
            crosswalk.obstacle = crossingroad.transform.Find("Obstacle").gameObject;
            crosswalk.navMeshObstacle = crosswalk.obstacle.GetComponent<NavMeshObstacle>();
            crosswalk.carLight = crossingroad.transform.Find("CarLight").GetComponent<Light>();
            crosswalk.peasantLight = crossingroad.transform.Find("PeasantLight").GetComponent<Light>();
            crosswalks.Add(crosswalk);
        }

        foreach (var crosswalk in crosswalks)
        {
            StartCoroutine(CrosswalkCycleRoutine(crosswalk));
        }
    }

    private IEnumerator CrosswalkCycleRoutine(Crosswalk crosswalk)
    {
        while (true)
        {
            crosswalk.ToggleObstacle();
            Debug.Log("Crosswalk: " + crosswalk.prefab.name + " is now " + (crosswalk.isWalkable ? "walkable" : "not walkable") + "(CrosswalkManager)");
            float waitTime = Random.Range(minCycleTime, maxCycleTime);
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void OnDrawGizmos()
    {
        foreach (var crosswalk in crosswalks)
        {
            Gizmos.color = crosswalk.isWalkable ? Color.green : Color.red;
            Gizmos.DrawWireSphere(crosswalk.center.position, crosswalk.containRadius);


            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(crosswalk.center.position, crosswalk.stopRadius);

            if (crosswalk.sideA != null && crosswalk.sideB != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(crosswalk.center.position, crosswalk.sideA.position);
                Gizmos.DrawLine(crosswalk.center.position, crosswalk.sideB.position);
            }
        }
    }

    public Crosswalk GetCrosswalkContainingPosition(Vector3 position)
    {
        foreach (var crosswalk in crosswalks)
        {
            if (Vector3.Distance(position, crosswalk.center.position) <= crosswalk.containRadius)
            {
                return crosswalk;
            }
        }
        return null;
    }

    public bool IsObjectInAnyCrosswalk(Vector3 position)
    {
        foreach (var crosswalk in crosswalks)
        {
            if (Vector3.Distance(position, crosswalk.center.position) <= crosswalk.stopRadius)
            {
                return true;
            }
        }
        return false;
    }
}