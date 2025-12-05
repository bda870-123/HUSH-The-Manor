using UnityEngine;
using TMPro;

public class TimeBomb : MonoBehaviour
{
    [Header("Timer Settings")]
    public float startTime = 10f;        // Time the bomb starts with
    public bool autoStart = true;        // Start countdown automatically?

    private float currentTime;
    private bool countdownActive = false;
    private bool hasExploded = false;

    [Header("UI Countdown")]
    public TextMeshProUGUI countdownText;
    public bool showMilliseconds = false;

    [Header("Door Reference")]
    public DoubleKey doorScript;         // Drag your DoubleKey door here

    [Header("Explosion Settings")]
    public GameObject explosionEffect;
    public AudioSource audioSource;
    public AudioClip explosionSound;

    void Start()
    {
        currentTime = startTime;

        if (autoStart)
            countdownActive = true;

        UpdateCountdownUI(); // Initialize UI
    }

    void Update()
    {
        if (!countdownActive || hasExploded)
            return;

        currentTime -= Time.deltaTime;

        UpdateCountdownUI();

        if (currentTime <= 0f)
        {
            CheckDoor();
        }
    }

    // Public method to manually start countdown
    public void StartCountdown()
    {
        countdownActive = true;
    }

    // Public method to reset timer
    public void ResetTimer(float newTime)
    {
        currentTime = newTime;
    }

    void UpdateCountdownUI()
    {
        if (countdownText == null)
            return;

        float displayTime = Mathf.Max(currentTime, 0f);

        if (showMilliseconds)
        {
            countdownText.text = displayTime.ToString("F2"); // 10.00, 9.99...
        }
        else
        {
            countdownText.text = Mathf.Ceil(displayTime).ToString(); // 10, 9, 8...
        }
    }

    void CheckDoor()
    {
        if (doorScript == null)
        {
            Explode();
            return;
        }

        if (IsDoorUnlocked())
        {
            Debug.Log("Bomb disarmed — door was unlocked in time.");
            Destroy(gameObject);
            return;
        }

        Explode();
    }

    bool IsDoorUnlocked()
    {
        // Access the private 'isUnlocked' variable
        var unlockedField = typeof(DoubleKey).GetField(
            "isUnlocked",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
        );

        return (bool)unlockedField.GetValue(doorScript);
    }

    void Explode()
    {
        hasExploded = true;

        Debug.Log("BOOM! The door was locked — bomb exploded!");

        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        if (audioSource != null && explosionSound != null)
            audioSource.PlayOneShot(explosionSound);

        Destroy(gameObject, 0.2f);
    }
}
