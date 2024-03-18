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

    [SerializeField] Transform FrontRightTransform;
    [SerializeField] Transform FrontLeftTransform;
    [SerializeField] Transform RearRightTransform;
    [SerializeField] Transform RearLeftTransform;


    public float acceleration = 500f;
    public float brakeForce = 300f;
    private float maxTurnangle = 15f;

    private float currentAcceleration = 0f;
    private float currentBrakeForce = 0f;
    private float currentTurnangle = 0f;

    private void FixedUpdate()
    {

        currentAcceleration = acceleration * Input.GetAxis("Vertical");
        
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

        currentTurnangle = maxTurnangle * Input.GetAxis("Horizontal");
        FrontLeft.steerAngle = currentTurnangle;
        FrontRight.steerAngle = currentTurnangle;
        UpdateWheel(FrontLeft, FrontLeftTransform);
        UpdateWheel(FrontRight, FrontRightTransform);
        UpdateWheel(RearLeft, RearLeftTransform);
        UpdateWheel(RearRight, RearRightTransform);
    }
    private void UpdateWheel(WheelCollider col, Transform trans)
    {
        Vector3 position;
        Quaternion rotation;
        col.GetWorldPose(out position, out rotation);

        trans.position = position;
        trans.rotation = rotation;
    }

}
