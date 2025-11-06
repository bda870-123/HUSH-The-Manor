using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeyV2 : MonoBehaviour, IInteractable
{
    public string keyID = "KeyName"; // The unique ID of this key
    public TextMeshProUGUI pickupText; // Optional: assign in inspector for "Press E to pick up" text


    void Start()
    {
        if (pickupText != null)
            pickupText.gameObject.SetActive(false);
    }

    public void Interact()
    {
        PickupKey();
    }

    void Update()
    {

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
