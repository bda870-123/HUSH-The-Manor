using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LockedDoorWithAudio : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    public Transform door;
    public Transform player;

    [Tooltip("You must have ALL keys in this list to unlock the door.")]
    public List<string> requiredKeyIDs = new List<string>();

    public float activationDistance = 3f;
    public float openAngle = 115f;
    public float closeAngle = 0f;
    public float openSpeed = 2f;
    public TextMeshProUGUI interactText;

    private bool isOpen = false;
    private float currentAngle;
    private bool isUnlocked = false;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource = null;
    [SerializeField] private AudioClip doorOpenClip = null;
    [SerializeField] private AudioClip doorCloseClip = null;
    [SerializeField] private AudioClip lockedClip = null;
    [SerializeField] private float openDelay = 0f;
    [SerializeField] private float closeDelay = 0f;

    void Start()
    {
        if (door == null)
            door = transform;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (interactText != null)
            interactText.gameObject.SetActive(false);

        currentAngle = closeAngle;
    }

    void Update()
    {
        if (player == null || door == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= activationDistance)
        {
            HandleInteractionText();
        }
        else if (interactText != null)
        {
            interactText.gameObject.SetActive(false);
        }

        float targetAngle = isOpen ? openAngle : closeAngle;
        currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * openSpeed);
        door.localRotation = Quaternion.Euler(0f, currentAngle, 0f);
    }

    public void Interact()
    {
        TryToggleDoor();
    }

    void HandleInteractionText()
    {
        if (interactText == null)
            return;

        if (!isUnlocked && !PlayerHasAllKeys())
        {
            interactText.text = "Locked: Requires multiple keys";
        }
        else
        {
            interactText.text = "Press [E] to open or close";
        }

        interactText.gameObject.SetActive(true);
    }

    void TryToggleDoor()
    {
        if (isUnlocked)
        {
            isOpen = !isOpen;

            if (isOpen)
                StartCoroutine(PlayClipWithDelay(doorOpenClip, openDelay));
            else
                StartCoroutine(PlayClipWithDelay(doorCloseClip, closeDelay));

            return;
        }

        // REQUIRES ALL KEYS
        if (PlayerHasAllKeys())
        {
            isUnlocked = true;
            isOpen = true;
            Debug.Log("Door unlocked! Player has ALL required keys.");
            StartCoroutine(PlayClipWithDelay(doorOpenClip, openDelay));
        }
        else
        {
            Debug.Log("Door is locked. Missing one or more required keys.");
            if (lockedClip != null)
                audioSource.PlayOneShot(lockedClip);
        }
    }

    // MUST HAVE ALL KEYS TO UNLOCK
    private bool PlayerHasAllKeys()
    {
        if (KeyInventory.Instance == null)
            return false;

        foreach (string keyID in requiredKeyIDs)
        {
            if (!KeyInventory.Instance.HasKey(keyID))
                return false; // missing a key cannot unlock
        }

        return true; // has all keys
    }

    private IEnumerator PlayClipWithDelay(AudioClip clip, float delay)
    {
        if (audioSource == null || clip == null)
            yield break;

        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        audioSource.PlayOneShot(clip);
    }
}
