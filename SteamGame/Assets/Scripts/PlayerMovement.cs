using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform itemContainer, camera;
    public Transform player;
    public float pickUprange;
    private float dropForwardforce, dropUpwardforce;
    public bool equipped;
    public static bool slotFull;
    public Transform currentItem;
    public GameObject wheelMesh;
    private float walkingSpeed = 7.5f;
    private float runningSpeed = 11.5f;
    private float jumpSpeed = 8f;
    private float CrouchJumpSpeed = 5f;
    private float crouchSpeed = 3.5f;
    private float lookSpeed = 2f;

    private float gravity = 20f;
    private float lookXLimit = 75f;
    private float crouchHeight = 0.5f;
    private float rotationX = 0f;

    private bool IsCrouching = false;
    public bool canMove = true;

    private CarAttachments car;
    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        characterController = GetComponent<CharacterController>();
        car = GameObject.FindAnyObjectByType<CarAttachments>();


    }
    void Update()
    {
        Place();
        Vector3 distanceToplayer = player.position - transform.position;
        if (!equipped && distanceToplayer.magnitude <= pickUprange && Input.GetKeyDown(KeyCode.E) && !slotFull) PickupItem();

        if (equipped && Input.GetKeyDown(KeyCode.Q)) Drop();

        if (!IsCrouching) //Is currenly NOT crouching
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
            float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
            float movementDirectionY = moveDirection.y;
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);

            if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
            {
                moveDirection.y = jumpSpeed;
            }
            else
            {
                moveDirection.y = movementDirectionY;
            }
            this.GetComponentInChildren<CapsuleCollider>().height = 1;
            characterController.height = 2f;
            this.GetComponentInChildren<Transform>().localScale = new Vector3(1, 1, 1);
        }
        else   //IS currenly crouching
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            float curSpeedX = crouchSpeed * Input.GetAxis("Vertical");
            float curSpeedY = crouchSpeed * Input.GetAxis("Horizontal");
            float movementDirectionY = moveDirection.y;
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);

            if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
            {
                moveDirection.y = CrouchJumpSpeed;
            }
            else
            {
                moveDirection.y = movementDirectionY;
            }
            this.GetComponentInChildren<CapsuleCollider>().height = crouchHeight;
            characterController.height = crouchHeight * 2;
            this.GetComponent<Transform>().localScale = new Vector3(1, crouchHeight, 1);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            IsCrouching = true;
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            IsCrouching = false;
        }
        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            Camera.main.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

            transform.Rotate(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }
    private void PickupItem()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, pickUprange))
        {
            if (hit.collider.gameObject.CompareTag("Clickable"))
            {
                Rigidbody itemRb = hit.collider.GetComponent<Rigidbody>();
                BoxCollider itemCol = hit.collider.GetComponent<BoxCollider>();


                Transform item = hit.collider.transform;

                equipped = true;
                slotFull = true;

                item.SetParent(itemContainer);
                item.localPosition = Vector3.zero;
                item.localRotation = Quaternion.Euler(Vector3.zero);
                item.localScale = Vector3.one;

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

        if (Physics.Raycast(ray, out hit, pickUprange) && Input.GetMouseButtonDown(0) && hit.collider.CompareTag("Placeable"))
        {
            Vector3 spawnPosition = hit.collider.transform.GetChild(0).position;
            GameObject newItem = Instantiate(wheelMesh, spawnPosition, Quaternion.identity);
            newItem.transform.SetParent(hit.collider.transform);
            newItem.transform.rotation = hit.collider.transform.rotation;
            BoxCollider newItemcol = newItem.GetComponent<BoxCollider>();
            Rigidbody newItemRb = newItem.GetComponent<Rigidbody>();
            if (newItemRb != null)
            {
                newItemcol.enabled = false;
                newItemRb.isKinematic = true;
                newItemRb.useGravity = false;
            }
            if (hit.collider.name == "FrontRightAxel")
            {
                car.wFR = newItem.transform;
            }
            if (hit.collider.name == "RearRightAxel")
            {
                car.wRR = newItem.transform;
            }
            if (hit.collider.name == "RearLeftAxel")
            {
                car.wRL = newItem.transform;
            }
            if (hit.collider.name == "FrontLeftAxel")
            {
                car.wFL = newItem.transform;
            }
            Destroy(currentItem.gameObject);
            equipped = false;
            currentItem = null;
            slotFull = false;
        }
    }
}