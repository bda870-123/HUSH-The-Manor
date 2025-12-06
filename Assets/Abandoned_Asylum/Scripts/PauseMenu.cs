using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject controlsMenu;
    [SerializeField] private Button defaultButton;

    private bool pauseActive;

    // Start is called before the first frame update
    void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);

    }

    // Update is called once per frame
    void Update()
    {
        if (playerInputHandler.PauseTriggered && pauseActive != true)
        {
            Pause();
            if (Gamepad.current != null)
            {
                if (EventSystem.current.currentSelectedGameObject == null)
                {
                    EventSystem.current.SetSelectedGameObject(defaultButton.gameObject);
                }
            }
        }
        else if (playerInputHandler.PauseTriggered && pauseActive == true)
        {
            Continue();
        }
    }

    public void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        pauseActive = true;
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void Continue()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(false);
        Time.timeScale = 1;
        pauseActive = false;
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Main Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SelectEventCurrentSelected(GameObject select)
    {
        if (Gamepad.current != null)
        {
            EventSystem.current.SetSelectedGameObject(select);
        }
    }

    public void OpenSettings()
    {
        pauseMenu.SetActive(false);
        controlsMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void OpenControls()
    {
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(true);
    }

    public void OpenPauseMenu()
    {
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }
}
