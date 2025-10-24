using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LockedDoor : MonoBehaviour
{
    public Transform door;
    public Transform player;
    public string requiredKeyID = "KeyName";
    public float activationDistance = 3f;
    public float openAngle = 115f;
    public float closeAngle = 0f;
    public float openSpeed = 2f;
    public TextMeshProUGUI interactText;

    private bool isOpen = false;
    private float currentAngle;
    private bool isUnlocked = false;

    void Start()
    {
        if (door == null)
            door = transform;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (interactText != null)
            interactText.gameObject.SetActive(false);

        currentAngle = closeAngle;
    }

    void Update()
    {
        if (player == null || door == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= activationDistance)
        {
            HandleInteractionText();

            if (Input.GetKeyDown(KeyCode.E))
            {
                TryToggleDoor();
            }
        }
        else
        {
            if (interactText != null)
                interactText.gameObject.SetActive(false);
        }

        // Smoothly rotate door
        float targetAngle = isOpen ? openAngle : closeAngle;
        currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * openSpeed);
        door.localRotation = Quaternion.Euler(0f, currentAngle, 0f);
    }

    void HandleInteractionText()
    {
        if (interactText == null)
            return;

        if (!isUnlocked && !KeyInventory.Instance.HasKey(requiredKeyID))
        {
            interactText.text = "Locked: Requires " + requiredKeyID;
        }
        else
        {
            interactText.text = "Press [E] to open or close";
        }

        interactText.gameObject.SetActive(true);
    }

    void TryToggleDoor()
    {
        // If already unlocked, toggle open/close
        if (isUnlocked)
        {
            isOpen = !isOpen;
            return;
        }

        // If not unlocked, check inventory
        if (KeyInventory.Instance != null && KeyInventory.Instance.HasKey(requiredKeyID))
        {
            isUnlocked = true;
            Debug.Log("Door unlocked with key: " + requiredKeyID);
            isOpen = true; // Automatically open after unlocking
        }
        else
        {
            Debug.Log("Door is locked. You need the " + requiredKeyID);
        }
    }
}