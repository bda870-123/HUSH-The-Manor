// Written by Alan Miranda-Perez

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Ensures interactable objects are compatible with the interactor source
// Use this interface on object scripts that are interactable by player
interface IInteractable
{
    public void Interact();
}

public class InteractRaycast : MonoBehaviour
{
    [Header("Raycast Parameters")]
    public Transform interactRaycastSource;
    public float interactRange;
    [SerializeField] private LayerMask interactableMask;

    [Header("UI")]
    public Image reticleImage; // Drag your Reticle Image UI element here in the Inspector
    public Color defaultColor = Color.white;
    public Color highlightColor = Color.red;

    [Header("References")]
    [SerializeField] private PlayerInputHandler playerInputHandler;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Ray ray = new Ray(interactRaycastSource.position, interactRaycastSource.forward);
        bool isHittingInteractable = false;


        // Send out raycast
        if (Physics.Raycast(ray, out RaycastHit hitInfo, interactRange, interactableMask))
        {
            // Check if the hit object is interactable
            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
            {
                isHittingInteractable = true;

                // Handle Interaction on Key Press
                if (playerInputHandler.InteractTriggered)
                {
                    interactObj.Interact();
                }
            }
        }

        // This code sets reticle color depending if raycast is hitting an interactable
        if (reticleImage != null)
        {
            if (isHittingInteractable)
            {
                // Raycast is true, change color to highlightColor
                reticleImage.color = highlightColor;
            }
            else
            {
                // Raycast is false or hit non-interactable, revert to defaultColor
                reticleImage.color = defaultColor;
            }
        }
    }
}
