using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    [SerializeField] WheelCollider FrontRight;
    [SerializeField] WheelCollider FrontLeft;
    [SerializeField] WheelCollider RearRight;
    [SerializeField] WheelCollider RearLeft;

    public float acceleration = 500f;
    public float brakeForce = 300f;

    private float currentAcceleration = 0f;
    private float currentBrakeForce = 0f;

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            currentBrakeForce = brakeForce;

        }
        else
        {
            currentBrakeForce = 0f;
        }
        FrontRight.motorTorque = currentAcceleration;
        FrontLeft.motorTorque = currentAcceleration;

        FrontRight.brakeTorque = currentBrakeForce;
        FrontLeft.brakeTorque = currentBrakeForce;
        RearRight.brakeTorque = currentBrakeForce;
        RearLeft.brakeTorque = currentBrakeForce;
    }
}
