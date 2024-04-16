using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    private CarManager carManager;
    private PlayerMovement player;

    public void Start()
    {
        player = FindAnyObjectByType<PlayerMovement>();
        carManager = FindObjectOfType<CarManager>();
    }

    public void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit) && Input.GetMouseButtonDown(0))
        {

            if (hit.collider.CompareTag("StartButton"))
            {
                carManager.StartIgnition();
            }



        }

        if (player.currentItem != null)
        {
            if (player.currentItem.name == "Fuel")
            {
                if (Physics.Raycast(ray, out hit) && Input.GetMouseButtonDown(0))
                {
                    if (hit.collider.CompareTag("Burner"))
                    {
                        Destroy(player.currentItem.gameObject);
                        player.equipped = false;
                        PlayerMovement.slotFull = false;
                        carManager.fuelAmount += carManager.fuelValue;
                        
                    }
                }
            }
        }

        

    }
}



