using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarManager : MonoBehaviour
{
    [Header("Stats")]
    public bool canDrive = false;
    public float fuelAmount;
    public float heatSourceTemperature;
    public float waterTemperature;
    public float steamPressure;
    
    [Header("Wheels Transforms")]
    public Transform wFR;
    public Transform wFL;
    public Transform wRR;
    public Transform wRL;
    [Header("Parts")]
    public Transform boiler;
    public Transform engine;
    public Transform tank;

    [Header("Wheels Colliders")]
    [SerializeField] WheelCollider FrontRight;
    [SerializeField] WheelCollider FrontLeft;
    [SerializeField] WheelCollider RearRight;
    [SerializeField] WheelCollider RearLeft;
    

    public float acceleration = 500f;
    public float brakeForce = 300f;
    private float maxTurnangle = 35f;

    private float currentAcceleration = 0f;
    private float currentBrakeForce = 0f;
    private float currentTurnangle = 0f;
    private void Update()
    {
        //if (steamPressure >)
    }
    private void FixedUpdate()
    {
        if (canDrive)
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
            RearRight.motorTorque = currentAcceleration;
            RearLeft.motorTorque = currentAcceleration;

            FrontRight.brakeTorque = currentBrakeForce;
            FrontLeft.brakeTorque = currentBrakeForce;
            RearRight.brakeTorque = currentBrakeForce;
            RearLeft.brakeTorque = currentBrakeForce;

            currentTurnangle = maxTurnangle * Input.GetAxis("Horizontal");
            FrontLeft.steerAngle = currentTurnangle;
            FrontRight.steerAngle = currentTurnangle;
            UpdateWheel(FrontLeft, wFL);
            UpdateWheel(FrontRight, wFR);
            UpdateWheel(RearLeft, wRL);
            UpdateWheel(RearRight, wRR);
        }
    }
    private void UpdateWheel(WheelCollider col, Transform trans)
    {
        if (trans == null) return;

        Vector3 position;
        Quaternion rotation;
        col.GetWorldPose(out position, out rotation);

        //trans.position = position;

        trans.rotation = rotation;

    }
}
