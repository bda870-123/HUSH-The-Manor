using System.Collections;
using UnityEngine;

public class RightDoorsWithAudio : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    public Transform door; // Drag your door object here

    [Tooltip("Rotation (in degrees) when the door is closed")]
    public Vector3 closedRotation = new Vector3(180f, 0f, 0f);

    [Tooltip("Rotation (in degrees) when the door is open")]
    public Vector3 openRotation = new Vector3(180f, 115f, 0f);

    [Tooltip("How fast the door rotates open/closed")]
    public float openSpeed = 3.5f;

    private bool isOpen = false;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource = null;   // Single AudioSource for both sounds
    [SerializeField] private AudioClip doorOpenClip = null;    // Drag open sound here
    [SerializeField] private AudioClip doorCloseClip = null;   // Drag close sound here
    [SerializeField] private float openDelay = 0.5f;           // Optional delay before playing open sound
    [SerializeField] private float closeDelay = 0.5f;          // Optional delay before playing close sound

    void Start()
    {
        if (door == null)
            door = transform;

        // Set initial rotation to closed
        door.localRotation = Quaternion.Euler(closedRotation);
    }

    void Update()
    {
        Vector3 targetRotation = isOpen ? openRotation : closedRotation;

        // Smoothly rotate towards target
        door.localRotation = Quaternion.Lerp(
            door.localRotation,
            Quaternion.Euler(targetRotation),
            Time.deltaTime * openSpeed
        );
    }

    public void Interact()
    {
        isOpen = !isOpen;

        // Play appropriate sound
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