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
        fuel
    }

    public TextMeshProUGUI waterStatus;
    public TextMeshProUGUI fuelStatus;
    public  float waterValue = 30f;
    public float fuelValue = 40f;

    public void Start()
    {

       UpdateFuelType();
    }

    public void UpdateFuelType()
    {
        if (fuelStatus != null)
        {
            if (fuelType == FuelType.water)
            {

                UpdateWaterStatus();
            }
        }

        if (waterStatus != null)
        {
            if (fuelType == FuelType.fuel)
            {

                UpdateFuelStatus();
            }
        }
         
    }

    public void UpdateWaterStatus()
     {
         waterStatus.text = "Water: " + waterValue.ToString("F1") + "L";
     }
    public void UpdateFuelStatus()
    {
        fuelStatus.text = "Fuel: " + fuelValue.ToString("F1") + "MJ";
    }


}
