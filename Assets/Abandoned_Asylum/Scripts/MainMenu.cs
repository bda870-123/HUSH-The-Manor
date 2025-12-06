using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        Screen.fullScreen = true;
    }

    public Button defaultButton;
    private void Update()
    {
        if (Gamepad.current != null)
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(defaultButton.gameObject);
            }
        }
    }
    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Show");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
