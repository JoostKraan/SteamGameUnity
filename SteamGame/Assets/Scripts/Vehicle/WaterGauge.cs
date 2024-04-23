using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterGauge : MonoBehaviour
{
    private CarManager carManager;
    public float maxSpeed = 0.0f;

    public float minSpeedarrowAngle;
    public float maxSpeedarrowAngle;
    public Transform arrow;
    private float state = 0.0f;

    public void Start()
    {
        carManager = GameObject.FindObjectOfType<CarManager>();
        Debug.Log(carManager.waterLevel);
    }
    private void Update()
    {
        state = carManager.waterLevel;

        if (arrow != null && maxSpeed != 0f)
        {
            float angle = Mathf.Lerp(minSpeedarrowAngle, maxSpeedarrowAngle, state / maxSpeed);
            if (!float.IsNaN(angle))
            {
                Vector3 newEulerAngles = new Vector3(angle, 90, 0);
                arrow.localEulerAngles = newEulerAngles;
            }
            else
            {
                Debug.LogError("Angle is NaN!");
            }
        }
    }
}
