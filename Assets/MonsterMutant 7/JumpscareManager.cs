using UnityEngine;
using UnityEngine.SceneManagement;

public class JumpscareManager : MonoBehaviour
{
    public GameObject jumpscareUI;
    public float scareDuration = 1.5f;
    public CameraShake camShake;
    

    public void TriggerJumpscare()
    {
        jumpscareUI.SetActive(true);

        // Play jumpscare sound
        GetComponent<AudioSource>().Play();

        // Shake camera
        if (camShake != null)
            StartCoroutine(camShake.Shake(1f, 0.3f));

        // Freeze the game
        Time.timeScale = 0f;

        // After delay, load Game Over
        Invoke(nameof(EndGame), scareDuration);
    }

    void EndGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameOverScreen");
    }
}