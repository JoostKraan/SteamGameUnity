using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
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

    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    public Transform currentUsingItem;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
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
}
