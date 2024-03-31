using UnityEngine;

public class WheelController : MonoBehaviour
{
    CarAttachments car;
    [SerializeField] WheelCollider FrontRight;
    [SerializeField] WheelCollider FrontLeft;
    [SerializeField] WheelCollider RearRight;
    [SerializeField] WheelCollider RearLeft;
    [SerializeField] bool canDrive = false;

    public float acceleration = 500f;
    public float brakeForce = 300f;
    private float maxTurnangle = 35f;

    private float currentAcceleration = 0f;
    private float currentBrakeForce = 0f;
    private float currentTurnangle = 0f;

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
            UpdateWheel(FrontLeft, car.wFL);
            UpdateWheel(FrontRight, car.wFR);
            UpdateWheel(RearLeft, car.wRL);
            UpdateWheel(RearRight, car.wRR);
        }    
    }
    private void Start()
    {
        car = GameObject.FindAnyObjectByType<CarAttachments>();
    }
    private void UpdateWheel(WheelCollider col, Transform trans)
    {
        if (trans == null) return;

        Vector3 position;
        Quaternion rotation;
        col.GetWorldPose(out position, out rotation);

        //trans.position = position;

        trans.rotation = rotation;

        print(trans.rotation);
    }
}
