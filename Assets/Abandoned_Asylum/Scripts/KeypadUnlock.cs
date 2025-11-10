using UnityEngine;

public class KeypadSwingDoor : MonoBehaviour
{
    public Transform doorHinge;
    public float openAngle = 90f;
    public float openSpeed = 2f;
    private bool isOpen = false;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        if (doorHinge == null)
            doorHinge = transform;

        closedRotation = doorHinge.localRotation;
        openRotation = Quaternion.Euler(0f, openAngle, 0f) * closedRotation;
    }

    void Update()
    {
        Quaternion targetRotation = isOpen ? openRotation : closedRotation;
        doorHinge.localRotation = Quaternion.Slerp(doorHinge.localRotation, targetRotation, Time.deltaTime * openSpeed);
    }

    // Must be PUBLIC and take NO PARAMETERS for UnityEvents
    public void UnlockDoor()
    {
        isOpen = true;
    }

    public void LockDoor()
    {
        isOpen = false;
    }
}
