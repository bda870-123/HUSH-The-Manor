using System.Collections;
using UnityEngine;

public class DrawerWithAudio : MonoBehaviour, IInteractable
{
    [Header("Drawer Settings")]
    public Transform drawer;            // Drag the drawer object here
    public float openDistance = 0.5f;   // How far the drawer slides out
    public float openSpeed = 2f;        // Smooth speed

    private bool isOpen = false;
    private Vector3 closedPosition;
    private Vector3 openPosition;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource = null;
    [SerializeField] private AudioClip drawerOpenClip = null;
    [SerializeField] private AudioClip drawerCloseClip = null;
    [SerializeField] private float openDelay = 0f;
    [SerializeField] private float closeDelay = 0f;

    void Start()
    {
        if (drawer == null)
            drawer = transform;

        closedPosition = drawer.localPosition;
        openPosition = closedPosition + new Vector3(0f, 0f, openDistance); // Slides along local Z-axis
    }

    void Update()
    {
        Vector3 targetPosition = isOpen ? openPosition : closedPosition;
        drawer.localPosition = Vector3.Lerp(drawer.localPosition, targetPosition, Time.deltaTime * openSpeed);
    }

    public void Interact()
    {
        isOpen = !isOpen;

        if (isOpen)
            StartCoroutine(PlayClipWithDelay(drawerOpenClip, openDelay));
        else
            StartCoroutine(PlayClipWithDelay(drawerCloseClip, closeDelay));
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
