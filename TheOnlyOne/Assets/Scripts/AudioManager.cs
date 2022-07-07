using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //Singleton
    [HideInInspector]
    private static AudioManager instance;
    [Header("Button Sounds")]
    public AudioSource audioSource;
    public AudioSource musicAudioSource;
    public AudioClip buttonHover;
    public AudioClip buttonPress;
    public AudioClip dropDownButton;
    public AudioClip music;

    public static AudioManager Instance { get => instance; private set => instance = value; }

    void Awake()
    {
        if (AudioManager.Instance == null)
        {
            AudioManager.Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void PlaySound(AudioClip _sound)
    {
        Instance.audioSource.PlayOneShot(_sound);
    }
    public void PlaySound(AudioClip _sound, float _volume)
    {
        Instance.audioSource.PlayOneShot(_sound, _volume);
    }
    public void HoverButton()
    {
        Instance.audioSource.PlayOneShot(buttonHover);
    }
    public void PressButton()
    {
        Instance.audioSource.PlayOneShot(buttonPress);
    }
    public void PressDropDownButton()
    {
        Instance.audioSource.PlayOneShot(dropDownButton);
    }
    public void PlayMusic()
    {
        Instance.musicAudioSource.UnPause();
        Instance.musicAudioSource.PlayOneShot(music);
    }
    public void PauseMusic()
    {
        Instance.musicAudioSource.Pause();
    }
}