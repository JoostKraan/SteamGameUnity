
using UnityEditor.Rendering;
using UnityEngine;

public class Interact : MonoBehaviour
{
    private BoxCollider coll;
    public Transform player, itemContainer, camera;
    private Rigidbody rb;
    Camera maincamera;
    private PlayerMovement playermovement;
    public CarAttachments car;
    public float pickUprange;
    public float dropForwardforce, dropUpwardforce;
  

    public bool equipped;
    public static bool slotFull;

    private void Start()
    {

        playermovement = player.GetComponent<PlayerMovement>();
        maincamera = Camera.main;
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

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Test");

            Ray ray = maincamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Clickable"))
                {
                    PickupItem();
                }
                if (hit.collider.CompareTag("Placeable"))
                {

                    Place();
                }
            }
        }
    }
    private void PickupItem()
    {

        gameObject.tag = "Untagged";
        playermovement.currentUsingItem = gameObject.transform;
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
        playermovement.currentUsingItem = null;
        equipped = false;
        slotFull = false;
        transform.SetParent(null);
        rb.velocity = rb.GetComponent<Rigidbody>().velocity;
        rb.AddForce(camera.forward * dropForwardforce, ForceMode.Impulse);
        rb.AddForce(camera.forward * dropUpwardforce, ForceMode.Impulse);
        float random = Random.Range(-1f, 1f);
        rb.AddTorque(new Vector3(random, random, random) * 10);
        rb.isKinematic = false;
        coll.isTrigger = false;
        gameObject.tag = "Clickable";
    }
    private void Place()
    {
        Ray ray = maincamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        rb.useGravity = false;
        rb.isKinematic = true;
        coll.isTrigger = false;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Placeable") && equipped)
            {
                
                Vector3 targetPosition = hit.transform.position;

                if (playermovement.currentUsingItem != null)
                {
                    
                    equipped = false;
                    slotFull = false;
                    Debug.Log(targetPosition);
                    coll.enabled = false;
                    Instantiate(playermovement.currentUsingItem.gameObject, targetPosition, Quaternion.identity, hit.transform);
                    coll.enabled = true;
                    Debug.Log(targetPosition);
                    gameObject.tag = "Clickable";
                    Destroy(gameObject);
                    playermovement.currentUsingItem = null;
                   
                }
            }
        }
    }
}
