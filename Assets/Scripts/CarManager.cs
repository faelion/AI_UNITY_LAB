using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarManager : MonoBehaviour
{
    public List<Transform> cars;

    void Start()
    {
        cars = new List<Transform>();
        foreach (GameObject car in GameObject.FindGameObjectsWithTag("Car"))
        {
            cars.Add(car.transform);
        }
    }
}
