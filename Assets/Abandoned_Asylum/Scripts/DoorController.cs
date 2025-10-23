using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Transform door; // Drag the door object here
    public Transform player; // Drag the player here (or use tag)
    public float activationDistance = 3f;
    public float openAngle = 115f;
    public float closeAngle = 0f;
    public float openSpeed = 2f;

    private bool isOpen = false; // Tracks if door is open
    private float currentAngle;  // Internal state for smooth lerp

    void Start()
    {
        if (door == null)
            door = transform;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        currentAngle = closeAngle;
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        // Toggle when E is pressed within range
        if (distance <= activationDistance && Input.GetKeyDown(KeyCode.E))
        {
            isOpen = !isOpen;
        }

        // Set target angle depending on door state
        float targetAngle = isOpen ? openAngle : closeAngle;

        // Smoothly rotate door
        currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * openSpeed);
        door.localRotation = Quaternion.Euler(0f, currentAngle, 0f);
    }
}

