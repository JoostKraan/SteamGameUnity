using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    private CarManager carManager;
    private PlayerMovement player;
    private PhysicalButton button;
    private Fuel fuel;

    public void Start()
    {
        fuel = FindAnyObjectByType<Fuel>();
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
                        Fuel fuel = player.currentItem.GetComponent<Fuel>();
                        carManager.InsertFuel(fuel.fuelValue);
                        Destroy(player.currentItem.gameObject);
                        player.equipped = false;
                        PlayerMovement.slotFull = false;
                    }
                    else
                    {
                        Debug.Log("You are not holding any Fuel");

                    }
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



