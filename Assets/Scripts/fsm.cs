using UnityEngine;
using System.Collections;

public class FSM : MonoBehaviour
{
    public Transform cop;
    public GameObject treasure;
    public float dist2Steal = 10f;
    public float evadeSpeed = 6f;
    public float wanderSpeed = 2f;
    public float approachSpeed = 2f;
    Moves moves;
    UnityEngine.AI.NavMeshAgent agent;
    private Animator anim;

    private WaitForSeconds wait = new WaitForSeconds(0.2f);
    delegate IEnumerator State();
    private State state;

    IEnumerator Start()
    {
        moves = gameObject.GetComponent<Moves>();
        agent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
        anim = gameObject.GetComponent<Animator>();

        yield return wait;

        state = Wander;

        while (enabled)
            yield return StartCoroutine(state());
    }

    IEnumerator Wander()
    {
        Debug.Log("Wander state");

        while (Vector3.Distance(cop.position, treasure.transform.position) < dist2Steal)
        {
            agent.speed = wanderSpeed;
            moves.Wander();
            yield return wait;
        };

        state = Approaching;
        anim.SetTrigger("approach");
    }

    IEnumerator Approaching()
    {
        Debug.Log("Approaching state");

        agent.speed = approachSpeed;
        moves.Seek(treasure.transform.position);

        bool stolen = false;
        while (Vector3.Distance(cop.position, treasure.transform.position) > dist2Steal)
        {
            if (Vector3.Distance(treasure.transform.position, transform.position) < 2f)
            {
                stolen = true;
                break;
            };
            yield return wait;
        };

        if (stolen)
        {
            treasure.GetComponent<Renderer>().enabled = false;
            Debug.Log("Stolen");
            state = Hiding;
            anim.SetTrigger("hide");
        }
        else
        {
            agent.speed = 1f;
            state = Wander;
            anim.SetTrigger("wander");
        }
    }


    IEnumerator Hiding()
    {
        Debug.Log("Hiding state");

        while (true)
        {
            agent.speed = evadeSpeed;
            moves.Hide();
            yield return wait;
        };
    }

    private void OnDrawGizmos()
    {
        if (treasure != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(treasure.transform.position, dist2Steal);
        }
    }
}

