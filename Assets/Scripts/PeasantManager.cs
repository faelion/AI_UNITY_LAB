using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeasantManager : MonoBehaviour
{
    public static PeasantManager Instance;

    private List<GameObject> peasants = new List<GameObject>();

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

    public void RegisterPeasant(GameObject peasant)
    {
        if (!peasants.Contains(peasant))
        {
            peasants.Add(peasant);
        }
    }

    public void UnregisterPeasant(GameObject peasant)
    {
        if (peasants.Contains(peasant))
        {
            peasants.Remove(peasant);
        }
    }

    public GameObject GetRandomPeasant()
    {
        if (peasants.Count == 0)
        {
            return null;
        }

        int randomIndex = Random.Range(0, peasants.Count);
        return peasants[randomIndex];
    }
}
