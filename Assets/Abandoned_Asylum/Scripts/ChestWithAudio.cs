using System.Collections;
using UnityEngine;

public class ChestWithAudio : MonoBehaviour, IInteractable
{
    [Header("Chest Settings")]
    public Transform lid; // Drag the lid object here
    public float openAngle = -70f;   // Lid rotates upward on X-axis
    public float closeAngle = 0f;
    public float openSpeed = 2f;

    private bool isOpen = false;
    private float currentAngle;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource = null;
    [SerializeField] private AudioClip chestOpenClip = null;
    [SerializeField] private AudioClip chestCloseClip = null;
    [SerializeField] private float openDelay = 0f;
    [SerializeField] private float closeDelay = 0f;

    void Start()
    {
        if (lid == null)
            lid = transform;

        currentAngle = closeAngle;
    }

    void Update()
    {
        float targetAngle = isOpen ? openAngle : closeAngle;

        currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * openSpeed);

        // Rotate around X-axis to open the lid upward
        lid.localRotation = Quaternion.Euler(currentAngle, 0f, 0f);
    }

    public void Interact()
    {
        isOpen = !isOpen;

        if (isOpen)
            StartCoroutine(PlayClipWithDelay(chestOpenClip, openDelay));
        else
            StartCoroutine(PlayClipWithDelay(chestCloseClip, closeDelay));
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
