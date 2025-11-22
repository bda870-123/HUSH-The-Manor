using UnityEngine;

public class BookSlot : MonoBehaviour, IInteractable
{
    public string requiredBookID; // ID of the correct book
    public bool isFilled = false;
    public Book currentBook;

    public void Interact()
    {
        PickUp playerPickup = FindObjectOfType<PickUp>();
        if (playerPickup == null) return;

        GameObject heldObj = playerPickup.GetHeldObject();
        if (heldObj == null) return;

        Book book = heldObj.GetComponent<Book>();
        if (book == null) return;

        // Handle insertion entirely inside BookSlot
        if (!isFilled && book.bookID == requiredBookID)
        {
            currentBook = book;
            isFilled = true;

            // Snap the book to the slot
            book.transform.position = transform.position;
            book.transform.rotation = transform.rotation;
            book.transform.SetParent(transform);

            Rigidbody rb = book.GetComponent<Rigidbody>();
            if (rb) rb.isKinematic = true;

            Collider col = book.GetComponent<Collider>();
            if (col) col.enabled = false;

            // Drop book from player hold
            playerPickup.ForceDrop();
        }
    }
}
