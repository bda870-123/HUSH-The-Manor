using UnityEngine;

public class SecretDoor : MonoBehaviour
{
    [Header("Door Settings")]
    public BookSlot[] slots;     // All slots required to open the door
    public Transform openPos;    // Empty transform marking the open position
    public float speed = 2f;

    private bool isOpen = false;

    void Update()
    {
        if (!isOpen && AllSlotsFilled())
        {
            isOpen = true;
        }

        if (isOpen)
        {
            transform.position = Vector3.Lerp(transform.position, openPos.position, Time.deltaTime * speed);
        }
    }

    private bool AllSlotsFilled()
    {
        foreach (BookSlot slot in slots)
        {
            if (!slot.isFilled) return false;
        }
        return true;
    }
}
