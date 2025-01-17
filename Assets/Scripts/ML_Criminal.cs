using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ML_Criminal : Agent
{
    public Transform police;
    public List<Transform> cars;
    public float moveForce = 5f;
    public float turnForce = 100f;
    public float radius = 10f;

    private Rigidbody rb;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    public override void OnEpisodeBegin()
    {
        if (this.transform.localPosition.y < 0)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0, 1.5f, 0);
        }

        transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(police.position);
        sensor.AddObservation(Vector3.Distance(transform.position, police.position));

        foreach (var car in cars)
        {
            sensor.AddObservation(car.position);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float move = actions.ContinuousActions[0]; // Forward/backward
        float turn = actions.ContinuousActions[1]; // Left/right

        Vector3 turnTorque = transform.up * turn * turnForce;
        rb.AddTorque(turnTorque, ForceMode.VelocityChange);

        Vector3 forwardMove = transform.forward * move * moveForce;
        rb.AddForce(forwardMove, ForceMode.VelocityChange);

        if (Vector3.Distance(transform.position, police.position) > radius)
        {
            SetReward(0.1f);
        }

        if ((transform.position.y < -1))
        {
            EndEpisode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Car"))
        {
            SetReward(-0.5f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}