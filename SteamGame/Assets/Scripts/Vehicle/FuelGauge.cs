using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class FuelGauge : MonoBehaviour
{
    private CarManager carManager;
    public float maxSpeed = 0.0f;
    
    public float minSpeedarrowAngle;
    public float maxSpeedarrowAngle;

    [Header("UI")]

  
    public Transform arrow;
    private float state = 0.0f;

    public void Start()
    {
        carManager = GameObject.FindObjectOfType<CarManager>();
        Debug.Log(carManager.fuelAmount);
    }
    private void Update()
    {
        state = carManager.fuelAmount;

        if (arrow != null && maxSpeed != 0f)
        {
            // Debug.Log("State: " + state);
            // Debug.Log("Max Speed: " + maxSpeed);
            float angle = Mathf.Lerp(minSpeedarrowAngle, maxSpeedarrowAngle, state / maxSpeed);
            // Debug.Log("Resulting Angle: " + angle);

            if (!float.IsNaN(angle))
            {
                Vector3 newEulerAngles = new Vector3(angle, 90, 0);
                // Debug.Log("New Euler Angles: " + newEulerAngles);
                arrow.localEulerAngles = newEulerAngles;
            }
            else
            {
                Debug.LogError("Angle is NaN!");
            }
        }
    }

}
