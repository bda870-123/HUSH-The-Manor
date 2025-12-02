using System.Collections;
using TMPro;
using UnityEngine;

public class DoubleKey : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    public Transform door;
    public Transform player;

    public string requiredKeyID = "KeyName";      // First key
    public string requiredKeyID2 = "KeyName2";    // Second key (ADDED)

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

        bool hasKey1 = KeyInventory.Instance.HasKey(requiredKeyID);
        bool hasKey2 = KeyInventory.Instance.HasKey(requiredKeyID2);

        if (!isUnlocked && (!hasKey1 || !hasKey2))
        {
            interactText.text = "Locked: Requires " + requiredKeyID + " + " + requiredKeyID2;
        }
        else
        {
            interactText.text = "Press [E] to open or close";
        }

        interactText.gameObject.SetActive(true);
    }

    void TryToggleDoor()
    {
        bool hasKey1 = KeyInventory.Instance.HasKey(requiredKeyID);
        bool hasKey2 = KeyInventory.Instance.HasKey(requiredKeyID2);

        if (isUnlocked)
        {
            isOpen = !isOpen;

            if (isOpen)
                StartCoroutine(PlayClipWithDelay(doorOpenClip, openDelay));
            else
                StartCoroutine(PlayClipWithDelay(doorCloseClip, closeDelay));

            return;
        }

        // 🔑 Must have BOTH required keys
        if (hasKey1 && hasKey2)
        {
            isUnlocked = true;
            isOpen = true;

            Debug.Log("Door unlocked with: " + requiredKeyID + " + " + requiredKeyID2);
            StartCoroutine(PlayClipWithDelay(doorOpenClip, openDelay));
        }
        else
        {
            Debug.Log("Door is locked. You need: " + requiredKeyID + " and " + requiredKeyID2);
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
