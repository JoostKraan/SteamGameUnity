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
                //button.ChangeColor();
                carManager.StartIgnition();
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
                        FuelManager fuel = player.currentItem.GetComponent<FuelManager>();
                        carManager.InsertFuel();
                        Destroy(player.currentItem.gameObject);
                        player.equipped = false;
                        PlayerMovement.slotFull = false;
                    }
                    else
                    {
                        Debug.Log("You are not holding any Fuel");

                    }
                }
                if (player.currentItem.name == "WaterContainer")
                {
                    FuelManager waterContainer = player.currentItem.GetComponent<FuelManager>();
                    carManager.InsertWater();
                    waterContainer.UpdateWaterStatus();
                    if  (carManager.remainingWater > 0)
                    {
                        waterContainer.waterValue = carManager.remainingWater;
                        return;
                    }
                    Destroy(player.currentItem.gameObject);
                    player.equipped = false;
                    PlayerMovement.slotFull = false;
                }

                if (hit.collider.CompareTag("FireStarter"))
                {
                    Debug.Log("Burning has started");
                    carManager.StartFireBurner();
                }
            }
        }
    }
}



