using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform itemContainer, camera;
    public Transform player;
    public Collider capsuleCollider;
    public float pickUprange;
    private float dropForwardforce, dropUpwardforce;
    public bool equipped;
    public static bool slotFull;
    public Transform currentItem;
    public GameObject wheelObj;
    public GameObject boilerObj;
    private float walkingSpeed = 7.5f;
    private float runningSpeed = 11.5f;
    private float jumpSpeed = 8f;
    private float crouchJumpSpeed = 5f;
    private float crouchSpeed = 3.5f;
    private float lookSpeed = 2f;

    private float gravity = 20f;
    private float lookXLimit = 75f;
    private float crouchHeight = 0.5f;
    private float rotationX = 0f;

    private bool isCrouching = false;
    public bool canMove = true;
    public bool canMoveCamera = true;
    public FuelManager fuelContainer;
    private CarManager car;
    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private Dictionary<string, string> itemPrefabs = new Dictionary<string, string>();

    void Start()
    {
        capsuleCollider = GetComponentInChildren<CapsuleCollider>();
        itemPrefabs.Add("Tire", "TireSpot");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        characterController = GetComponent<CharacterController>();
        car = FindObjectOfType<CarManager>(); // Changed from GameObject.FindAnyObjectByType<CarManager>()
    }

    void Update()
    {
        if (canMoveCamera)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            Camera.main.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

            transform.Rotate(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        if (canMove)
        {
            Place();
            Vector3 distanceToPlayer = player.position - transform.position;
            if (!equipped && distanceToPlayer.magnitude <= pickUprange && Input.GetKeyDown(KeyCode.E) && !slotFull)
                PickupItem();

            if (equipped && Input.GetKeyDown(KeyCode.Q))
                Drop();

            if (!isCrouching)
            {
                Vector3 forward = transform.TransformDirection(Vector3.forward);
                Vector3 right = transform.TransformDirection(Vector3.right);
                bool isRunning = Input.GetKey(KeyCode.LeftShift);
                float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
                float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
                float movementDirectionY = moveDirection.y;
                moveDirection = (forward * curSpeedX) + (right * curSpeedY);

                if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
                    moveDirection.y = jumpSpeed;
                else
                    moveDirection.y = movementDirectionY;

                characterController.height = 2f;
                characterController.Move(moveDirection * Time.deltaTime);
            }
            else
            {
                Vector3 forward = transform.TransformDirection(Vector3.forward);
                Vector3 right = transform.TransformDirection(Vector3.right);
                float curSpeedX = crouchSpeed * Input.GetAxis("Vertical");
                float curSpeedY = crouchSpeed * Input.GetAxis("Horizontal");
                float movementDirectionY = moveDirection.y;
                moveDirection = (forward * curSpeedX) + (right * curSpeedY);

                if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
                    moveDirection.y = crouchJumpSpeed;
                else
                    moveDirection.y = movementDirectionY;

                characterController.height = crouchHeight * 2;
                characterController.Move(moveDirection * Time.deltaTime);
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
            isCrouching = true;

        if (Input.GetKeyUp(KeyCode.C))
            isCrouching = false;

        if (!characterController.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;
    }

    private void PickupItem()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, pickUprange))
        {
            if (hit.collider.gameObject.CompareTag("Clickable"))
            {
                fuelContainer = hit.collider.GetComponent<FuelManager>();
                Rigidbody itemRb = hit.collider.GetComponent<Rigidbody>();
                BoxCollider itemCol = hit.collider.GetComponent<BoxCollider>();
                Transform item = hit.collider.transform;
                equipped = true;
                slotFull = true;
                item.SetParent(itemContainer);
                item.localPosition = Vector3.zero;
                item.localRotation = Quaternion.Euler(Vector3.zero);

                itemRb.isKinematic = true;
                itemCol.isTrigger = true;
                currentItem = hit.collider.gameObject.transform;
            }
        }
    }

    private void Drop()
    {
        if (currentItem != null)
        {
            Rigidbody rb = currentItem.GetComponent<Rigidbody>();
            BoxCollider coll = currentItem.GetComponent<BoxCollider>();

            if (rb != null && coll != null)
            {
                equipped = false;
                slotFull = false;
                currentItem.SetParent(null);
                rb.velocity = rb.GetComponent<Rigidbody>().velocity;
                rb.AddForce(camera.forward * dropForwardforce, ForceMode.Impulse);
                rb.AddForce(camera.forward * dropUpwardforce, ForceMode.Impulse);
                float random = Random.Range(-1f, 1f);
                rb.AddTorque(new Vector3(random, random, random) * 10);
                rb.isKinematic = false;
                coll.isTrigger = false;
                currentItem = null;
            }
        }
    }

    private void Place()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, pickUprange) && Input.GetMouseButtonDown(0))
        {
            if (hit.collider.TryGetComponent<CarPart>(out CarPart carPart) && currentItem)
            {
                if (hit.collider.GetComponent<CarPart>().attachableType == currentItem.GetComponent<CarPart>().partType)
                {
                    Vector3 spawnPosition = hit.collider.transform.parent.position;
                    GameObject newItem = Instantiate(currentItem.gameObject, spawnPosition, Quaternion.identity);
                    newItem.transform.SetParent(hit.collider.transform);
                    newItem.transform.rotation = hit.collider.transform.rotation;
                    newItem.transform.localRotation = Quaternion.Euler(90, 0, 0);
                    BoxCollider newItemcol = newItem.GetComponent<BoxCollider>();
                    Rigidbody newItemRb = newItem.GetComponent<Rigidbody>();
                    newItemcol.enabled = false;
                    newItemRb.isKinematic = true;
                    newItemRb.useGravity = false;
                    if (hit.collider.name == "FrontRightAxle")
                        car.wFR = newItem.transform;
                    else if (hit.collider.name == "RearRightAxle")
                        car.wRR = newItem.transform;
                    else if (hit.collider.name == "RearLeftAxle")
                        car.wRL = newItem.transform;
                    else if (hit.collider.name == "FrontLeftAxle")
                        car.wFL = newItem.transform;

                    Destroy(currentItem.gameObject);
                    equipped = false;
                    currentItem = null;
                    slotFull = false;
                }
            }
        }
    }
}
