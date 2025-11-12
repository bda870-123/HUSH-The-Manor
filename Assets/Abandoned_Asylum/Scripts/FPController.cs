// Written by Alan Miranda-Perez
// 11/10/2025

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintMultiplier = 2.0f;

    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float gravityMulitplier = 1.0f;

    [Header("Look Parameters")]
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float gamepadSensitivity = 150.0f;
    [SerializeField] private float upDownLookRange = 80.0f;

    [Header("Crouch Parameters")]
    [SerializeField] private float standingHeight = 2.0f;
    [SerializeField] private float crouchingHeight = 1.0f;
    [SerializeField] private float standingHeadY = 1.8f;
    [SerializeField] private float crouchingHeadY = 0.9f;
    [SerializeField] private float crouchMultiplier = 0.5f;
    [SerializeField] private float crouchTransitionSpeed = 10f;
    [SerializeField] private LayerMask whatIsObstacle;

    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform headTransform;
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private PlayerInputHandler playerInputHandler;

    private Vector3 currentMovement;
    private float verticalRotation;
    private bool isCrouching = false;
    private Vector3 standingCenter;
    private Vector3 crouchingCenter;
    private Vector3 standingHeadPosition;
    private Vector3 crouchingHeadPosition;
    private Vector3 standingBodyPosition;
    private Vector3 crouchingBodyPosition;
    private float currentSpeed
    {
        get
        {
            if(isCrouching)
            {
                return walkSpeed * crouchMultiplier;
            }

            if (playerInputHandler.SprintTriggered && !playerInputHandler.CrouchTriggered)
            {
                return walkSpeed * sprintMultiplier;
            }
            return walkSpeed;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        characterController.height = standingHeight;

        standingCenter = new Vector3(0, standingHeight / 2.0f, 0);
        crouchingCenter = new Vector3(0, crouchingHeight / 2.0f, 0);
        characterController.center = standingCenter;

        standingHeadPosition = new Vector3(0, standingHeadY, 0);
        crouchingHeadPosition = new Vector3(0, crouchingHeadY, 0);
        headTransform.localPosition = standingHeadPosition;

        standingBodyPosition = standingCenter;
        crouchingBodyPosition = crouchingCenter;
        bodyTransform.localPosition = standingBodyPosition;
    }

    // Update is called once per frame
    void Update()
    {
        HandleCrouching();
        HandleMovement();
        HandleRotation();
    }

    private Vector3 CalculateWorldDirection()
    {
        Vector3 inputDirection = new Vector3(playerInputHandler.MovementInput.x, 0f, playerInputHandler.MovementInput.y);
        Vector3 worldDirectoin = transform.TransformDirection(inputDirection);

        return worldDirectoin.normalized;
    }

    private void HandleJumping()
    {
        if (characterController.isGrounded)
        {
            currentMovement.y = -0.5f;

            if (playerInputHandler.JumpTriggered)
            {
                currentMovement.y = jumpForce;
            }
        }
        else
        {
            currentMovement.y += Physics.gravity.y * gravityMulitplier * Time.deltaTime;
        }
    }

    private void HandleMovement()
    {
        Vector3 worldDirection = CalculateWorldDirection();
        currentMovement.x = worldDirection.x * currentSpeed;
        currentMovement.z = worldDirection.z * currentSpeed;

        HandleJumping();
        characterController.Move(currentMovement * Time.deltaTime);

    }

    private void ApplyHorizontalRotation(float rotationAmount)
    {
        transform.Rotate(0, rotationAmount, 0);
    }

    private void ApplyVerticlaRotation(float rotationAmount)
    {
        verticalRotation = Mathf.Clamp(verticalRotation - rotationAmount, -upDownLookRange, upDownLookRange);
        headTransform.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    private void HandleRotation()
    {
        float inputX = playerInputHandler.RotationInput.x;
        float inputY = playerInputHandler.RotationInput.y;

        float rotationX;
        float rotationY;

        if (playerInputHandler.IsUsingGamepad)
        {
            // For gamepads, input is a *rate* (degrees per second)
            // So we MUST multiply by Time.deltaTime.
            rotationX = inputX * gamepadSensitivity * Time.deltaTime;
            rotationY = inputY * gamepadSensitivity * Time.deltaTime;
        }
        else
        {
            // For mouse, input is a *delta* (pixels per frame)
            // So we must NOT multiply by Time.deltaTime.
            rotationX = inputX * mouseSensitivity;
            rotationY = inputY * mouseSensitivity;
        }

        ApplyHorizontalRotation(rotationX);
        ApplyVerticlaRotation(rotationY);
    }

    private void HandleCrouching()
    {
        bool wantsToCrouch = playerInputHandler.CrouchTriggered;

        if (wantsToCrouch)
        {
            isCrouching = true;
        }
        else
        {
            // Only stand if the state is crouched AND there's room
            if (isCrouching && CanStand())
            {
                isCrouching = false;
            }
            // If we can't stand, "isCrouching" remains true
        }

        // Apply the state smoothly
        float targetHeight = isCrouching ? crouchingHeight : standingHeight;
        Vector3 targetCenter = isCrouching ? crouchingCenter : standingCenter;
        Vector3 targetHeadPos = isCrouching ? crouchingHeadPosition : standingHeadPosition;
        Vector3 targetBodyPos = isCrouching ? crouchingBodyPosition : standingBodyPosition;

        // Lerp
        characterController.height = Mathf.Lerp(characterController.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);
        characterController.center = Vector3.Lerp(characterController.center, targetCenter, Time.deltaTime * crouchTransitionSpeed);
        headTransform.localPosition = Vector3.Lerp(headTransform.localPosition, targetHeadPos, Time.deltaTime * crouchTransitionSpeed);
        bodyTransform.localPosition = Vector3.Lerp(bodyTransform.localPosition, targetBodyPos, Time.deltaTime * crouchTransitionSpeed);
    }

    private bool CanStand()
    {
        // Get the standing capsule's properties
        Vector3 currentPos = transform.position;
        float radius = characterController.radius;

        // Calculate the top/bottom points of the *standing* capsule
        // These are the centers of the spheres at the capsule's ends
        Vector3 point1 = currentPos + standingCenter + (Vector3.down * (standingHeight / 2 - radius));
        Vector3 point2 = currentPos + standingCenter + (Vector3.up * (standingHeight / 2 - radius));

        // Check if this new standing capsule would collide with anything
        // We check against the "whatIsObstacle" layermask
        return !Physics.CheckCapsule(point1, point2, radius, whatIsObstacle);
    }
}
