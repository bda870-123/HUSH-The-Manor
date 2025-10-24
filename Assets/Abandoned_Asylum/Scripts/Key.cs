using UnityEngine;
using TMPro;

public class KeyPickup : MonoBehaviour
{
    public string keyID = "KeyName"; // The unique ID of this key
    public TextMeshProUGUI pickupText; // Optional: assign in inspector for "Press E to pick up" text
    public float pickupDistance = 3f;

    private Transform player;
    private bool canPickUp = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (pickupText != null)
            pickupText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (player == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= pickupDistance)
        {
            if (pickupText != null)
            {
                pickupText.text = "Press [E] to pick up key";
                pickupText.gameObject.SetActive(true);
            }

            canPickUp = true;

            if (Input.GetKeyDown(KeyCode.E))
            {
                PickupKey();
            }
        }
        else
        {
            if (pickupText != null)
                pickupText.gameObject.SetActive(false);

            canPickUp = false;
        }
    }

    void PickupKey()
    {
        KeyInventory.Instance.AddKey(keyID); // Add to player inventory
        Debug.Log("Picked up key: " + keyID);

        if (pickupText != null)
            pickupText.gameObject.SetActive(false);

        Destroy(gameObject); // Remove key from the world
    }
}
