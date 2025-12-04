using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
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

    public void StartGame()
    {
        SceneManager.LoadScene("Show");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
