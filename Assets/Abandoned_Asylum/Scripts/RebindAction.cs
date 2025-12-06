using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RebindAction : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private InputActionReference actionReference; // Drag the specific action here (e.g., Jump)
    [SerializeField] private int selectedBindingIndex = 0; // 0 for Keyboard, usually 1 for Gamepad (depending on your setup)
    [SerializeField] private InputBinding.DisplayStringOptions displayStringOptions;

    [Header("UI Components")]
    [SerializeField] private TMP_Text bindingText;
    [SerializeField] private Button rebindButton;
    [SerializeField] private GameObject waitingForInputOverlay; // Optional: Text saying "Press any key..."


    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    private void Start()
    {
        // Update the UI text when the game starts
        UpdateBindingDisplay();

        // Add listener to button
        rebindButton.onClick.AddListener(StartRebinding);
    }

    public void StartRebinding()
    {
        rebindButton.interactable = false;
        if (waitingForInputOverlay) waitingForInputOverlay.SetActive(true);

        // We must disable the action before rebinding it
        actionReference.action.Disable();

        // Start the interactive rebind process
        rebindingOperation = actionReference.action.PerformInteractiveRebinding(selectedBindingIndex)
            .WithControlsExcluding("Mouse") // Prevent binding mouse movement
            .OnMatchWaitForAnother(0.1f) // waiting time to avoid double clicks
            .OnComplete(operation => RebindComplete())
            .OnCancel(operation => RebindComplete())
            .Start();
    }

    private void RebindComplete()
    {
        // Clean up
        rebindingOperation.Dispose();

        if (waitingForInputOverlay) waitingForInputOverlay.SetActive(false);
        rebindButton.interactable = true;

        // Re-enable the action so gameplay works again
        actionReference.action.Enable();

        // Update UI
        UpdateBindingDisplay();

        // SAVE THE BINDINGS (Call the saver system here)
        InputSaver.Save(actionReference.action.actionMap.asset);

        EventSystem.current.SetSelectedGameObject(rebindButton.gameObject);
    }

    private void UpdateBindingDisplay()
    {
        if (actionReference != null && bindingText != null)
        {
            // Gets the readable string (e.g., "Space", "A", "Left Mouse")
            bindingText.text = actionReference.action.GetBindingDisplayString(selectedBindingIndex, displayStringOptions);
        }
    }
}
