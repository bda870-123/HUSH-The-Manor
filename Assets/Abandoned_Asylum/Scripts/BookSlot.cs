using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookSlot : MonoBehaviour
{
    public string requiredTag = "canPickUp";
    public float activationDistance = 2.0f;   // how close the player must be
    public Transform player;                  // drag Player object here
    public PickUp pickUpScript;               // drag your PickUp script object here
    public bool isFilled = false;
    public SecretBookshelf bookshelfManager;

    void Update()
    {
        if (isFilled) return;

        // 1. Check distance
        float dist = Vector3.Distance(player.position, transform.position);
        if (dist > activationDistance) return;

        // 2. Make sure the player is holding something
        GameObject heldBook = pickUpScript.GetHeldObject();
        if (heldBook == null) return;

        // 3. Make sure that object is a valid book
        if (!heldBook.CompareTag(requiredTag)) return;

        // 4. Press E to place the book
        if (Input.GetKeyDown(KeyCode.E))
        {
            PlaceBook(heldBook);
        }
    }

    void PlaceBook(GameObject book)
    {
        Rigidbody rb = book.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        // Snap into the slot
        book.transform.position = transform.position;
        book.transform.rotation = transform.rotation;
        book.transform.SetParent(transform);

        // Tell PickUp to drop the book
        pickUpScript.ForceDrop();

        isFilled = true;

        Debug.Log("Book placed in slot");

        if (bookshelfManager != null)
            bookshelfManager.CheckCompletion();
    }
}
