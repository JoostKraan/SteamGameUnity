using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CarManager : MonoBehaviour
{
    //TO start engine 
    //Fuel amount has to be at least 75mj. waterHeat is not dependent to start the engine as long as the steam pressure is above 800 kPa.

    //Insert fuel into furnace > fuel value goes up > over time waterTemp value goes up dependent on amount of fuel > when water reaches 100C steam gets produced and rises in pressure >
    // when the steam pressure hits 800 kPa the car becomes drivable at a relatively slow speed.

    [Header("Script References")]
    private FuelManager fuelscript;
    private PlayerMovement player;
    private FuelManager Burner;
    private FuelManager Boiler;
    private FuelManager WaterTank;


    [Header("States")]
    [SerializeField] public bool canDrive = false;
    [SerializeField] private bool engineRunning = false;
    [SerializeField] public bool isHeating = false; //if the burner is burning fuel and generating heat for the boiler
    [Header("Steam")]
    [SerializeField]private float optimalSteampressure = 800; //kPa
    [SerializeField] private float currentSteampressure = 0; //kPa
    [Header("Water")]
    
    [SerializeField] public float waterTemp = 0f; //Celcius
    [SerializeField] public float waterLevel = 0f; //current water level
    [SerializeField] private float waterBoilinglevel = 100f; // at this level steam is produced
    [HideInInspector] public float remainingWater = 0f; //temporary value for calculating remaining water when container has more that the max storage
    private float currentWaterLerpSpeed = 0f; 
    private float maxWaterLerpSpeed = 68; 
    private float lerpSpeedIncrement = 2;
    private float waterConsumption = 0.02f;
    private float heatRate = 10f;
    private float maxWaterlevel = 110f;
    public float cooldownRate = 0.2f;

    [Header("Fuel")]
    
    [SerializeField] public float burnRate = 1;
    [SerializeField] private float maxfuelAmount = 100f;
    [SerializeField] public float fuelAmount = 0f; //megaJoules
    [HideInInspector] public float remainingFuel = 0f;

    [Header("Transforms")]
    public Transform wFR;
    public Transform wFL;
    public Transform wRR;
    public Transform wRL;
    public Transform seatingPos;
    public Transform exitPos;
    [Header("Parts")]
    public Transform boilerT;
    public Transform engineT;
    public Transform tankT;

    [Header("Wheels Colliders")]
    [SerializeField] WheelCollider FrontRight;
    [SerializeField] WheelCollider FrontLeft;
    [SerializeField] WheelCollider RearRight;
    [SerializeField] WheelCollider RearLeft;
    [Header("Car Stats")]
    [SerializeField]private float acceleration = 500f;
    [SerializeField] private float brakeForce = 300f;
    [SerializeField] private float maxTurnangle = 35f;
    [SerializeField] private float currentAcceleration = 0f;
    [SerializeField] private float currentBrakeForce = 0f;
    [SerializeField] private float currentTurnangle = 0f;
    private void Start()
    {
        exitPos = GameObject.Find("ExitPos").transform;
        seatingPos = GameObject.Find("SeatingPos").transform;
        Burner = GameObject.Find("Burner").GetComponent<FuelManager>();
        Boiler = GameObject.Find("Boiler").GetComponent<FuelManager>();
        WaterTank = GameObject.Find("WaterTank").GetComponent<FuelManager>();
        
        player = FindAnyObjectByType<PlayerMovement>();
    }
    public void InsertFuel()
    {
        fuelscript = player.fuelContainer;

        if (fuelAmount + fuelscript.fuelValue > maxfuelAmount)
        {
            remainingFuel = (fuelAmount + fuelscript.fuelValue) - maxfuelAmount;
            fuelAmount = maxfuelAmount;
            Debug.Log("Max capacity reached");
        }
        else
        {
            fuelAmount += fuelscript.fuelValue;
        }

        if (remainingFuel > 0)
        {
            Debug.Log("Remaining fuel: " + remainingFuel + "MJ. Tank cannot hold more than " + maxfuelAmount + "MJ.");
            fuelscript.fuelValue = remainingFuel;
        }
    }

    public IEnumerator HeatWater()
    {
        while (isHeating && fuelAmount > 0 && waterLevel > 0)
        {
            // Calculate the change in fuel and water heat over time
            float fuelChange = Time.deltaTime * burnRate;
            float waterHeatChange = Time.deltaTime * heatRate;

            // Burn fuel
            fuelAmount -= fuelChange;

            // Convert water level to water heat
            waterTemp = Mathf.Lerp(waterTemp, waterTemp + waterHeatChange, Time.deltaTime * currentWaterLerpSpeed);

            // Check if water temperature exceeds boiling level
            if (waterTemp >= waterBoilinglevel)
            {
                waterLevel -= waterHeatChange * waterConsumption; // Adjust water consumption factor
            }
            else
            {
                // If not boiling, decrease water level gradually
                waterLevel -= Mathf.Lerp(0, waterHeatChange * waterConsumption, Time.deltaTime * currentWaterLerpSpeed);
            }

            // Update UI
            Burner.UpdateFuelStatus();
            WaterTank.UpdateWaterStatus();
            Boiler.UpdateBoilerStatus();

            // Increase current water lerp speed gradually until max value
            currentWaterLerpSpeed = Mathf.Min(currentWaterLerpSpeed + Time.deltaTime * lerpSpeedIncrement, maxWaterLerpSpeed);

            yield return null; // Wait for the next frame
        }

        // If heating stops, slowly decrease water temperature
        while (!isHeating && waterTemp > 0)
        {
            waterTemp -= Time.deltaTime * cooldownRate; // Adjust cooldown rate as needed
            Boiler.UpdateBoilerStatus();
            yield return null;
        }

        // If fuel or water runs out, stop heating
        fuelAmount = Mathf.Max(0, fuelAmount);
        isHeating = false;
    }








    public void InsertWater()
    {
        fuelscript = player.fuelContainer;
        if (waterLevel + fuelscript.waterValue > maxWaterlevel)
        {
            remainingWater = (waterLevel + fuelscript.waterValue) - maxWaterlevel;
            waterLevel = maxWaterlevel;
            Debug.Log("Max capacity reached");
        }
        else
        {
            waterLevel += fuelscript.waterValue;
        }

        if (remainingWater > 0)
        {
            Debug.Log("Remaining water: " + remainingWater + "L. Tank cannot hold more than " + maxWaterlevel + "L.");
        }
    }

    public void StartFireBurner()
    {
        isHeating = true;
        StartCoroutine(HeatWater());
        
        
       
    }

    
    public void StartIgnition()
    {
        if (currentSteampressure >= optimalSteampressure && fuelAmount >= 75)
        {
            engineRunning = true;
            Debug.Log("Running");
            
        }
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
            FrontLeft.motorTorque = currentAcceleration;
            FrontRight.motorTorque = currentAcceleration;


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
