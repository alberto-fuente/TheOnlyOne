using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{

    public AudioSource audioSource;
    public AudioClip buttonHover;
    public AudioClip buttonPressed;
    public AudioClip MenuMusic;

    /*
   public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    public void OptionsMenu()
    {

    }*/
    private void OnEnable()
    {
        SceneDirector.instance.isLoading = false;
        SceneDirector.instance.loadingScreen.SetActive(false);
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
    public void HoverButtonPlay()
    {
        audioSource.PlayOneShot(buttonHover);
    }
    public void PressButtonPlay()
    {
        audioSource.PlayOneShot(buttonPressed);
    }
    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}

