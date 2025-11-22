using UnityEngine;

public class Book : MonoBehaviour, IInteractable
{
    [Header("Book Settings")]
    public string bookID; // Unique ID for the book

    public void Interact()
    {
        // Find the player pickup script
        PickUp playerPickup = FindObjectOfType<PickUp>();
        if (playerPickup == null) return;

        // If player isn't holding anything, pick up this book
        if (playerPickup.GetHeldObject() == null)
        {
            playerPickup.PickUpObject(gameObject);
        }
        // If player is holding this book, drop it
        else if (playerPickup.GetHeldObject() == gameObject)
        {
            playerPickup.ForceDrop();
        }
    }
}
