using UnityEngine;
using UnityEngine.SceneManagement;

public class JumpscareManager : MonoBehaviour
{
    public GameObject jumpscareUI;
    public float scareDuration = 1.5f;

    public void TriggerJumpscare()
    {
        jumpscareUI.SetActive(true);
        Time.timeScale = 0f;

        Invoke(nameof(EndGame), scareDuration);
    }

    void EndGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameOverScreen");
    }
}