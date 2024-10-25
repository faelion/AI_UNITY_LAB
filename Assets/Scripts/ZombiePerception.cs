using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiePerception : MonoBehaviour
{
    public Camera frustum;
    public LayerMask detectionMask;
    public float alertRadius = 15f;
    public float alertInterval = 1f;
    public float detectionRange = 30f;
    private ZombieMovement zombieMovement;
    private bool playerDetected = false;
    private Transform player;
    private Coroutine alertCoroutine;

    private void Awake()
    {
        zombieMovement = GetComponent<ZombieMovement>();
    }

    void Start()
    {
        player = zombieMovement.player;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, player.position) >= detectionRange)
        {
            playerDetected = false;
            zombieMovement.StopChasing();
            StopAlerting();
        }
        else if (IsPlayerInFrustum())
        {
            if (!playerDetected)
            {
                playerDetected = true;
                zombieMovement.StartChasing();
                StartAlerting();
            }
        }
        //else if (playerDetected)
        //{
        //    playerDetected = false;
        //    zombieMovement.StopChasing();
        //    StopAlerting();
        //}
    }

    private bool IsPlayerInFrustum()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(frustum);
        Collider playerCollider = player.GetComponent<Collider>();

        if (playerCollider != null && GeometryUtility.TestPlanesAABB(planes, playerCollider.bounds))
        {
            RaycastHit hit;
            Vector3 directionToPlayer = (player.position - frustum.transform.position).normalized;
            Ray ray = new Ray(frustum.transform.position, directionToPlayer);

            if (Physics.Raycast(ray, out hit, frustum.farClipPlane, detectionMask))
            {
                return hit.collider.CompareTag("ZombieTarget");
            }
        }
        return false;
    }

    private void StartAlerting()
    {
        if (alertCoroutine == null)
        {
            alertCoroutine = StartCoroutine(AlertNearbyZombies());
        }
    }

    private void StopAlerting()
    {
        if (alertCoroutine != null)
        {
            StopCoroutine(alertCoroutine);
            alertCoroutine = null;
        }
    }

    private IEnumerator AlertNearbyZombies()
    {
        while (true)
        {
            Collider[] nearbyZombies = Physics.OverlapSphere(transform.position, alertRadius, detectionMask);

            foreach (Collider col in nearbyZombies)
            {
                ZombieMovement zombie = col.GetComponent<ZombieMovement>();
                if (zombie != null && zombie != zombieMovement)
                {
                    zombie.SendMessage("OnPlayerDetected", SendMessageOptions.DontRequireReceiver);
                }
            }

            yield return new WaitForSeconds(alertInterval);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, alertRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}