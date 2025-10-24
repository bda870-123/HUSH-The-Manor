using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyInventory : MonoBehaviour
{
    public static KeyInventory Instance;

    [Header("UI Setup")]
    public Transform keyPanel;          // Panel where key icons go
    public GameObject keyIconPrefab;    // Prefab for icons (UI Image)
    public TextMeshProUGUI pickupText;  // "Picked up key" popup text
    public float popupDuration = 2f;

    [Header("Available Keys")]
    public List<KeyData> allKeys = new List<KeyData>();

    private HashSet<string> collectedKeys = new HashSet<string>();
    private Dictionary<string, Sprite> keySprites = new Dictionary<string, Sprite>();
    private Coroutine popupRoutine;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (var key in allKeys)
        {
            if (!keySprites.ContainsKey(key.keyID))
                keySprites.Add(key.keyID, key.icon);
        }
    }

    public void AddKey(string keyID)
    {
        if (collectedKeys.Contains(keyID))
            return;

        collectedKeys.Add(keyID);
        Debug.Log("Collected key: " + keyID);

        if (keySprites.ContainsKey(keyID) && keyPanel != null)
        {
            GameObject icon = Instantiate(keyIconPrefab, keyPanel);
            icon.GetComponent<Image>().sprite = keySprites[keyID];
        }

        ShowPickupPopup(keyID);
    }

    public bool HasKey(string keyID)
    {
        return collectedKeys.Contains(keyID);
    }

    public void ShowPickupPopup(string keyName)
    {
        if (pickupText == null)
            return;

        if (popupRoutine != null)
            StopCoroutine(popupRoutine);

        popupRoutine = StartCoroutine(PopupRoutine(keyName));
    }

    private IEnumerator PopupRoutine(string keyName)
    {
        pickupText.text = "Picked up: " + keyName;

        float time = 0f;
        Color c = pickupText.color;
        Vector3 baseScale = pickupText.transform.localScale;

        // Fade in + bounce
        while (time < 0.3f)
        {
            time += Time.deltaTime;
            float t = time / 0.3f;
            c.a = Mathf.Lerp(0, 1, t);
            pickupText.color = c;

            // Scale bounce (1.0 ? 1.2 ? 1.0)
            float scale = 1f + Mathf.Sin(t * Mathf.PI) * 0.2f;
            pickupText.transform.localScale = baseScale * scale;
            yield return null;
        }

        yield return new WaitForSeconds(popupDuration);

        // Fade out
        time = 0f;
        while (time < 0.5f)
        {
            time += Time.deltaTime;
            float t = time / 0.5f;
            c.a = Mathf.Lerp(1, 0, t);
            pickupText.color = c;
            yield return null;
        }
    }
}

[System.Serializable]
public class KeyData
{
    public string keyID;
    public Sprite icon;
}
