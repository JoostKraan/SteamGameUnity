using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FuelManager : MonoBehaviour
{
    public FuelType fuelType;
    private IEnumerator coroutine;
    public enum FuelType
    {
       WaterTank,
        FuelTank,
        Boiler,
        Burner,
        WaterContainer,
        FuelContainer,
        none
    }
    private CarManager carManager;

    public TextMeshProUGUI waterContainerstatus;
    public TextMeshProUGUI fuelContainerstatus;
    public TextMeshProUGUI waterStatus;
    public TextMeshProUGUI fuelStatus;
    public TextMeshProUGUI boilerTempstatus;
    public  float waterValue = 30f;
    public float fuelValue = 40f;
   

    public void Start()
    {

        carManager = FindAnyObjectByType<CarManager>();
        UpdateFuelStatus();
        UpdateWaterStatus();
        UpdateFuelContainerStatus();
        UpdateWaterContainerStatus();

    }
        public void UpdateWaterContainerStatus()
     {
        if (waterContainerstatus != null)
        waterContainerstatus.text = "Water: " + waterValue.ToString("F1") + "L";
     }
    public void UpdateFuelContainerStatus()
    {
        if (fuelContainerstatus != null)
            fuelContainerstatus.text = "Fuel: " + fuelValue.ToString("F1") + "MJ";
    }
    public void UpdateWaterStatus()
    {
        if (waterStatus != null)
            waterStatus.text = "Water: " + carManager.waterLevel.ToString("F1") + "L";  
    }
    public void UpdateFuelStatus()
    {
        if (fuelStatus != null)
            fuelStatus.text = "Fuel " + carManager.fuelAmount.ToString("F1") + " MJ";
    }

    public void UpdateBoilerStatus()
    {
        if(boilerTempstatus != null)
        {
            boilerTempstatus.text = "Temperature \n" + carManager.waterTemp.ToString("F1") + " °C";
        }
    }


}
