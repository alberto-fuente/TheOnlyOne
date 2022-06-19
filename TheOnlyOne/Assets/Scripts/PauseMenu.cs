using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [Header("Properties")]
    public static bool GameIsPaused = false;

    [Header("References")]
    public GameObject pauseMenu;
    public GameObject expScreen;//Avoid Pause Menu pop up when game is over
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !expScreen.activeSelf)
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        GameIsPaused = true;
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        unhideCursor();
    }

    public void Resume()
    {
        GameIsPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        hideCursor();
    }

    public void hideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void unhideCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


}
