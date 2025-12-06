// Written by Alan Miranda-Perez
// 11/10/2025

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Action Map Name Reference")]
    [SerializeField] private string actionMapName = "Player";

    [Header("Action Name References")]
    [SerializeField] private string move = "Move";
    [SerializeField] private string look = "Look";
    [SerializeField] private string jump = "Jump";
    [SerializeField] private string sprint = "Sprint";
    [SerializeField] private string interact = "Interact";
    [SerializeField] private string crouch = "Crouch";
    [SerializeField] private string pause = "Pause";

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction interactAction;
    private InputAction crouchAction;
    private InputAction pauseAction;

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool JumpTriggered { get; private set; }
    public bool SprintTriggered { get; private set; }
    public bool InteractTriggered { get; private set; }
    public bool CrouchTriggered { get; private set; }
    public bool PauseTriggered { get; private set; }
    public bool IsUsingGamepad { get; private set; }


    private void Awake()
    {
        InputSaver.Load(playerControls);

        InputActionMap mapReferences = playerControls.FindActionMap(actionMapName);

        moveAction = mapReferences.FindAction(move);
        lookAction = mapReferences.FindAction(look);
        jumpAction = mapReferences.FindAction(jump);
        sprintAction = mapReferences.FindAction(sprint);
        interactAction = mapReferences.FindAction(interact);
        crouchAction = mapReferences.FindAction(crouch);
        pauseAction = mapReferences.FindAction(pause);

        SubscribeActionValuesToInputEvents();
    }

    private void SubscribeActionValuesToInputEvents()
    {
        moveAction.performed += inputInfo => MoveInput = inputInfo.ReadValue<Vector2>();
        moveAction.canceled += inputInfo => MoveInput = Vector2.zero;

        lookAction.performed += inputInfo =>
        {
            // Check if the control that triggered this action is a gamepad
            IsUsingGamepad = inputInfo.control.device is Gamepad;

            LookInput = inputInfo.ReadValue<Vector2>();
        };
        lookAction.canceled += inputInfo => LookInput = Vector2.zero;

        jumpAction.performed += inputInfo => JumpTriggered = true;

        sprintAction.performed += inputInfo => SprintTriggered = true;
        sprintAction.canceled += inputInfo => SprintTriggered = false;

        interactAction.performed += inputInfo => InteractTriggered = true;

        crouchAction.performed += inputInfo => CrouchTriggered = true;
        crouchAction.canceled += inputInfo => CrouchTriggered = false;

        pauseAction.performed += inputInfo => PauseTriggered = true;
    }

    private void LateUpdate()
    {
        InteractTriggered = false;
        PauseTriggered = false;
        JumpTriggered = false;

    }

    private void OnEnable()
    {
        playerControls.FindActionMap(actionMapName).Enable();
    }

    private void OnDisable()
    {
        playerControls.FindActionMap(actionMapName).Disable();
    }
}
