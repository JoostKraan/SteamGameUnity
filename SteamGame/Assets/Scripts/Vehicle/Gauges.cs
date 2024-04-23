using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gauges : MonoBehaviour
{
    public Rigidbody target;

    public float maxSpeed = 0.0f;

    public float minSpeedarrowAngle;
    public float maxSpeedarrowAngle;

    [Header("UI")]

    public Text speedLabel;
    public Transform arrow;
    private float speed = 0.0f;

    private void Update()
    {
        speed = target.velocity.magnitude * 3.6f;

        if (speedLabel != null)
        {
            speedLabel.text = ((int)speed) + "km/h";

        }
        if(arrow != null)
        {
            
            arrow.localEulerAngles  =
                new Vector3(Mathf.Lerp(minSpeedarrowAngle, maxSpeedarrowAngle, speed / maxSpeed), 90, 0);
        }
    }

}
