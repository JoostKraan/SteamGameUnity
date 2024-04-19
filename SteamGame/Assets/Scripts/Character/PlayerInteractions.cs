using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    private CarManager carManager;
    private PlayerMovement player;
    private PhysicalButton button;
    private FuelManager fuel;
   

    public void Start()
    {
        fuel = FindAnyObjectByType<FuelManager>();
        player = FindAnyObjectByType<PlayerMovement>();
        carManager = FindObjectOfType<CarManager>();
    }

    public void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit) && Input.GetMouseButtonDown(0))
        {
            button = hit.collider.transform.GetComponent<PhysicalButton>();
            if (hit.collider.CompareTag("StartButton"))
            {
                carManager.StartIgnition();
            }
            if (hit.collider.CompareTag("FireStarter"))
            {
               if (carManager.fuelAmount  >= 10)
                {
                    carManager.StartFireBurner();
                    Debug.Log("Burning has started");
                }
                
                   
                
            }

        if (player.currentItem != null)
        {

            if (Physics.Raycast(ray, out hit) && Input.GetMouseButtonDown(0))
            {
                if (hit.collider.CompareTag("Burner"))
                {
                    if (player.currentItem.name == ("Fuel"))
                    {
                        FuelManager burner = hit.collider.GetComponent<FuelManager>();
                        FuelManager fuel = player.currentItem.GetComponent<FuelManager>();
                        carManager.InsertFuel();
                        fuel.UpdateFuelStatus();
                        if (carManager.remainingFuel > 0)
                        {
                            fuel.UpdateFuelContainerStatus();
                            burner.UpdateFuelStatus();
                            fuel.fuelValue = carManager.remainingFuel;
                            fuel.UpdateFuelContainerStatus();
                            burner.UpdateFuelStatus();
                            return;
                        }
                        fuel.UpdateFuelContainerStatus();
                        burner.UpdateFuelStatus();
                        Destroy(player.currentItem.gameObject);
                        player.equipped = false;
                        PlayerMovement.slotFull = false;
                    }
                    
                }
                if (player.currentItem.name == "WaterContainer")
                {
                    if (hit.collider.CompareTag("WaterTank"))
                    {
                        FuelManager waterTank = hit.collider.GetComponent<FuelManager>();
                        FuelManager waterContainer = player.currentItem.GetComponent<FuelManager>();
                        carManager.InsertWater();
                        waterTank.UpdateWaterStatus();
                        if (carManager.remainingWater > 0)
                        {
                            waterContainer.waterValue = carManager.remainingWater;
                            waterTank.UpdateWaterStatus();
                            waterContainer.UpdateWaterContainerStatus();
                            return;
                        }
                        waterTank.UpdateWaterStatus();
                        Destroy(player.currentItem.gameObject);
                        player.equipped = false;
                        PlayerMovement.slotFull = false;
                    }
                }
                
                   
                    
                }
            }
        }
    }
}



