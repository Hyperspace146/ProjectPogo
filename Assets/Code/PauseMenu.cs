using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject PauseMenuUI;

    [Tooltip("Aiming will be disabled while paused.")]
    public CameraController PlayerCamera;

    [Tooltip("Player input will be disabled while paused.")]
    public PlayerController PlayerController;

    [Tooltip("Objects, such as HUD elements, to set inactive while the game is paused and the pause menu is open.")]
    public GameObject[] ToDisable;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Unpause();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        Cursor.lockState = CursorLockMode.Confined;
        PlayerCamera.AimDisabled = true;
        PlayerController.InputDisabled = true;
        PauseMenuUI.SetActive(true);
        foreach (GameObject o in ToDisable)
        {
            o.SetActive(false);
        }
        GameIsPaused = true;
    }

    public void Unpause()
    {
        Cursor.lockState = CursorLockMode.Locked;
        PlayerCamera.AimDisabled = false;
        PlayerController.InputDisabled = false;
        PauseMenuUI.SetActive(false);
        foreach (GameObject o in ToDisable)
        {
            o.SetActive(true);
        }
        GameIsPaused = false;
    }

    public void TitleScreen()
    {
        SceneManager.LoadScene(0);
    }
}
