using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    private BoxCollider coll;
    public Transform player, itemContainer, camera;
    private Rigidbody rb;


    public float pickUprange;
    public float dropForwardforce, dropUpwardforce;

    public bool equipped;
    public static bool slotFull;

    private void Start()
    {
        coll = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        if (!equipped)
        {
            rb.isKinematic = false;
            coll.isTrigger = false;
        }
        if (equipped)
        {
            rb.isKinematic = true;
            coll.isTrigger = true;
            slotFull = true;
        }

    }

    private void Update()
    {
        Vector3 distanceToplayer = player.position - transform.position;
        if (!equipped && distanceToplayer.magnitude <= pickUprange && Input.GetKeyDown(KeyCode.E) && !slotFull) PickupItem();

        if (equipped && Input.GetKeyDown(KeyCode.Q)) Drop();
    }
    private void PickupItem()
    {
        equipped = true;
        slotFull = true;

        transform.SetParent(itemContainer);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = Vector3.one;
        rb.isKinematic = true;
        coll.isTrigger = true;
    }

    private void Drop()
    {
        equipped = false;
        slotFull = false;
        transform.SetParent(null);
        rb.velocity = rb.GetComponent<Rigidbody>().velocity;
        rb.AddForce(camera.forward * dropForwardforce, ForceMode.Impulse);
        rb.AddForce(camera.forward * dropUpwardforce, ForceMode.Impulse);
        float random =  Random.Range(-1f,1f);
        rb.AddTorque(new Vector3(random, random, random)* 10);
        rb.isKinematic = false;
        coll.isTrigger = false;
    }
   
}
