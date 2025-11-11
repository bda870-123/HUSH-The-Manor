using System.Collections;
using TMPro;
using UnityEngine;

public class LockedDoorWithAudio : MonoBehaviour
{
    [Header("Door Settings")]
    public Transform door;
    public Transform player;
    public string requiredKeyID = "KeyName";
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
    [SerializeField] private AudioClip lockedClip = null;  // Sound when trying to open a locked door
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

            if (Input.GetKeyDown(KeyCode.E))
            {
                TryToggleDoor();
            }
        }
        else if (interactText != null)
        {
            interactText.gameObject.SetActive(false);
        }

        // Smoothly rotate the door
        float targetAngle = isOpen ? openAngle : closeAngle;
        currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * openSpeed);
        door.localRotation = Quaternion.Euler(0f, currentAngle, 0f);
    }

    void HandleInteractionText()
    {
        if (interactText == null)
            return;

        if (!isUnlocked && !KeyInventory.Instance.HasKey(requiredKeyID))
            interactText.text = "Locked: Requires " + requiredKeyID;
        else
            interactText.text = "Press [E] to open or close";

        interactText.gameObject.SetActive(true);
    }

    void TryToggleDoor()
    {
        if (isUnlocked)
        {
            // Toggle door state and play sound
            isOpen = !isOpen;
            if (isOpen)
                StartCoroutine(PlayClipWithDelay(doorOpenClip, openDelay));
            else
                StartCoroutine(PlayClipWithDelay(doorCloseClip, closeDelay));
            return;
        }

        // Check inventory for key
        if (KeyInventory.Instance != null && KeyInventory.Instance.HasKey(requiredKeyID))
        {
            isUnlocked = true;
            isOpen = true;
            Debug.Log("Door unlocked with key: " + requiredKeyID);
            StartCoroutine(PlayClipWithDelay(doorOpenClip, openDelay));
        }
        else
        {
            Debug.Log("Door is locked. You need the " + requiredKeyID);
            if (lockedClip != null)
                audioSource.PlayOneShot(lockedClip);
        }
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
