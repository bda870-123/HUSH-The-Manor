using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SecretBookshelf : MonoBehaviour
{
    public BookSlot[] bookSlots;
    public Animator secretDoorAnimator;
    private bool isUnlocked = false;

    public void CheckCompletion()
    {
        if (isUnlocked) return;

        foreach (BookSlot slot in bookSlots)
        {
            if (!slot.isFilled)
                return;
        }

        isUnlocked = true;
        Debug.Log("All books placed! The door opens...");

        if (secretDoorAnimator != null)
            secretDoorAnimator.SetTrigger("Open");
    }
}
