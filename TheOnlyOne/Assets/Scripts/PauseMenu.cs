using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public static bool GameIsPaused = false;
    public AudioSource audioSource;
    public AudioClip buttonHover;
    public AudioClip buttonPressed;
    public GameObject expScreen;//evitar que el menu de opciones aparezca cuando se termina la partida
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)&&!expScreen.activeSelf)
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

    private void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        unhideCursor();
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        hideCursor();
    }
    public void LoadScene(int sceneIndex)
    {
        Time.timeScale = 1;
        SceneDirector.instance.LoadScene(sceneIndex);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    private void hideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void unhideCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void HoverButtonPlay()
    {
        audioSource.PlayOneShot(buttonHover);
    }
    public void PressButtonPlay()
    {
        audioSource.PlayOneShot(buttonPressed);
    }
}
