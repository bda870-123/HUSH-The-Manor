using System.Collections;
using UnityEngine;

public class DoorWithAudio : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    public Transform door; // Drag the door object here
    public float openAngle = 115f;
    public float closeAngle = 0f;
    public float openSpeed = 2f;

    private bool isOpen = false;
    private float currentAngle;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource = null;   // Only one AudioSource needed
    [SerializeField] private AudioClip doorOpenClip = null;    // Drag the open sound here
    [SerializeField] private AudioClip doorCloseClip = null;   // Drag the close sound here
    [SerializeField] private float openDelay = 0f;             // Optional delay
    [SerializeField] private float closeDelay = 0f;            // Optional delay

    void Start()
    {
        if (door == null)
            door = transform;

        currentAngle = closeAngle;
    }

    void Update()
    {
        float targetAngle = isOpen ? openAngle : closeAngle;

        currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * openSpeed);
        door.localRotation = Quaternion.Euler(0f, currentAngle, 0f);
    }

    public void Interact()
    {
        isOpen = !isOpen;

        if (isOpen)
            StartCoroutine(PlayClipWithDelay(doorOpenClip, openDelay));
        else
            StartCoroutine(PlayClipWithDelay(doorCloseClip, closeDelay));
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
