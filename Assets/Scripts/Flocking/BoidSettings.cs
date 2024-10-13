using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BoidSettings : ScriptableObject {
    // Settings
    [Range(0.0f, 5.0f)]
    public float minSpeed = 2;
    [Range(0.0f, 5.0f)]
    public float maxSpeed = 5;
    [Range(0.0f, 5.0f)]
    public float perceptionRadius = 2.5f;
    [Range(0.0f, 5.0f)]
    public float avoidanceRadius = 1;
    [Range(0.0f, 5.0f)]
    public float maxSteerForce = 3;

    [Header("Weights")]
    [Range(0.0f, 5.0f)]
    public float alignWeight = 1;
    [Range(0.0f, 5.0f)]
    public float cohesionWeight = 1;
    [Range(0.0f, 5.0f)]
    public float seperateWeight = 1;

    [Range(0.0f, 20.0f)]
    public float targetWeight = 1;

    [Header ("Collisions")]
    public LayerMask obstacleMask;
    [Range(0.0f, 1.0f)]
    public float boundsRadius = .27f;
    [Range(0.0f, 20.0f)]
    public float avoidCollisionWeight = 10;
    [Range(0.0f, 10.0f)]
    public float collisionAvoidDst = 5;

}