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

        if (camShake != null)
            StartCoroutine(camShake.Shake(1f, 0.2f));  // <-- shake

        Time.timeScale = 0f;
        Invoke(nameof(EndGame), scareDuration);
    }

    void EndGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameOverScreen");
    }
}
