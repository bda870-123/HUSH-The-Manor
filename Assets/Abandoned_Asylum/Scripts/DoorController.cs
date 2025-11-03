using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour, IInteractable
{
    public Transform door; // Drag the door object here
    public float openAngle = 115f;
    public float closeAngle = 0f;
    public float openSpeed = 2f;


    private bool isOpen = false; // Tracks if door is open
    private float currentAngle;  // Internal state for smooth lerp

    void Start()
    {
        if (door == null)
            door = transform;

        currentAngle = closeAngle;


    }

    void Update()
    {

        // Set target angle depending on door state
        float targetAngle = isOpen ? openAngle : closeAngle;

        // Smoothly rotate door
        currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * openSpeed);
        door.localRotation = Quaternion.Euler(0f, currentAngle, 0f);


    }



    public void Interact()
    {
        isOpen = !isOpen;
    }
}
