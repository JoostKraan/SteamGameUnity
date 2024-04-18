using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FuelManager : MonoBehaviour
{
    public FuelType fuelType;
    public enum FuelType
    {
        water,
        fuel,
        none
    }
    public Reciever reciever;
    public enum Reciever
    {
        burner,
        watertank,
        none
    }
    private CarManager carManager;

    public TextMeshProUGUI waterContainerstatus;
    public TextMeshProUGUI fuelContainerstatus;
    public TextMeshProUGUI waterStatus;
    public TextMeshProUGUI fuelStatus;
    public  float waterValue = 30f;
    public float fuelValue = 40f;

    public void Start()
    {
        carManager = FindAnyObjectByType<CarManager>();
        UpdateFuelStatus();
    }

    public void UpdateFuelType()
    {
            if (fuelType == FuelType.water)
            {
            Debug.Log("test");
                UpdateWaterContainerStatus();
            }        
            if (fuelType == FuelType.fuel)
            {
                Debug.Log("test");
            UpdateFuelContainerStatus();
            }
            if (reciever == Reciever.burner)
            {
                Debug.Log("test");
            UpdateFuelStatus();
            }
            if (reciever == Reciever.watertank)
            {
                Debug.Log("test");
            UpdateWaterStatus();
            }
        

        
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
        {
            fuelStatus.text = "Fuel " + carManager.fuelAmount.ToString("F1") + " MJ";
        }
           
    }


}
