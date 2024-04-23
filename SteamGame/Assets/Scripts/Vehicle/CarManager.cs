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
    private bool producingSteam = false;
    [Header("Steam")]
    private float maxSteampressure = 1000f;
    private float steamProductionrate = 0.002f;
    private float optimalSteampressure = 350f; //
    [SerializeField] private float currentSteampressure = 0; //kPa

    [Header("Water")]
    [SerializeField] public float waterTemp = 0f; //Celcius
    [SerializeField] public float waterLevel = 100f; //current water level
    private float waterBoilinglevel = 100f; // at this level steam is produced
    [HideInInspector] public float remainingWater = 0f; //temporary value for calculating remaining water when container has more that the max storage
    private float currentWaterLerpSpeed = 0f;
    private float maxWaterLerpSpeed = 35f;
    private float lerpSpeedIncrement = 2;
    private float waterConsumption = 0.02f;
    private float heatRate = 10f;
    private float maxWaterlevel = 90f;
    private float cooldownRate = 0.2f;

    [Header("Fuel")]
    [SerializeField] public float fuelAmount = 100f; //megaJoules
    private float burnRate = 0.02f;
    [SerializeField] private float maxfuelAmount = 100f;
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
    [SerializeField] private float acceleration = 500f;
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

    public void Update()
    {
        if (waterTemp >= waterBoilinglevel)
        {
            producingSteam = true;
            StartCoroutine(ProduceSteam());
            
        }
        if (currentSteampressure >= optimalSteampressure)
        {
            canDrive = true;
        }
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
            float fuelChange = Time.deltaTime * burnRate;
            float waterHeatChange = Time.deltaTime * heatRate;
            fuelAmount -= fuelChange;
            waterTemp = Mathf.Lerp(waterTemp, waterTemp + waterHeatChange, Time.deltaTime * currentWaterLerpSpeed);

            if (waterTemp >= waterBoilinglevel)
            {
                waterLevel -= waterHeatChange * waterConsumption;
            }
            else
            {

                waterLevel -= Mathf.Lerp(0, waterHeatChange * waterConsumption, Time.deltaTime * currentWaterLerpSpeed);
            }

            Burner.UpdateFuelStatus();
            WaterTank.UpdateWaterStatus();
            Boiler.UpdateBoilerStatus();
            currentWaterLerpSpeed = Mathf.Min(currentWaterLerpSpeed + Time.deltaTime * lerpSpeedIncrement, maxWaterLerpSpeed);
            yield return null;
        }

        while (!isHeating && waterTemp > 0)
        {
            waterTemp -= Time.deltaTime * cooldownRate;
            Boiler.UpdateBoilerStatus();
            yield return null;
        }

        fuelAmount = Mathf.Max(0, fuelAmount);
        isHeating = false;

        
    }

    public IEnumerator ProduceSteam()
    {
        while (waterTemp >= waterBoilinglevel && producingSteam && currentSteampressure <= maxSteampressure)
        {
            currentSteampressure += steamProductionrate * Time.deltaTime;

            if (currentSteampressure > maxSteampressure)
            {
                Debug.Log("Your boiler is about to blow");
                
                producingSteam = false;
            }

            yield return null;
        }
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
          
            float steamInfluence = Mathf.Clamp(currentSteampressure / optimalSteampressure, 0f, 1f); 
            float steamModifiedAcceleration = acceleration * steamInfluence;
            float userInputAcceleration = Input.GetAxis("Vertical");
            float combinedAcceleration = Mathf.Min(steamModifiedAcceleration, 125) * userInputAcceleration;

            currentAcceleration = combinedAcceleration;

            if (Input.GetKey(KeyCode.Space))
            {
                currentBrakeForce = brakeForce;
            }
            else
            {
                currentBrakeForce = 0f;
            }

            // Apply the modified acceleration to all wheels
            RearRight.motorTorque = currentAcceleration;
            RearLeft.motorTorque = currentAcceleration;
            FrontLeft.motorTorque = currentAcceleration;
            FrontRight.motorTorque = currentAcceleration;

            // Apply brake force
            FrontRight.brakeTorque = currentBrakeForce;
            FrontLeft.brakeTorque = currentBrakeForce;
            RearRight.brakeTorque = currentBrakeForce;
            RearLeft.brakeTorque = currentBrakeForce;

            // Steering
            currentTurnangle = maxTurnangle * Input.GetAxis("Horizontal");
            FrontLeft.steerAngle = currentTurnangle;
            FrontRight.steerAngle = currentTurnangle;

            // Update wheel positions
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
