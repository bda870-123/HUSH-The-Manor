using UnityEngine;
using TMPro;

public class UIMessageManager : MonoBehaviour
{
    public static UIMessageManager Instance;

    public GameObject messagePanel;
    public TextMeshProUGUI messageText;

    public float defaultDisplayTime = 2f;
    private float timer = 0f;

    void Awake()
    {
        Instance = this;
        messagePanel.SetActive(false);
    }

    void Update()
    {
        if (messagePanel.activeSelf)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
                messagePanel.SetActive(false);
        }
    }

    public void ShowMessage(string msg, float duration = -1)
    {
        if (duration <= 0)
            duration = defaultDisplayTime;

        timer = duration;

        messageText.text = msg;
        messagePanel.SetActive(true);
    }
}
