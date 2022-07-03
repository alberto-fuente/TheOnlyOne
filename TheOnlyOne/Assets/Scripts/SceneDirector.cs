using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDirector : MonoBehaviour
{
    //Singleton
    private static SceneDirector instance;
    [Header("Components")]
    public GameObject loadingScreen;
    public GameObject loadingWheel;
    [SerializeField] private float wheelSpeed;
    [SerializeField] private float minLoadTime = 1.7f;

    public bool isLoading;

    public static SceneDirector Instance { get => instance; set => instance = value; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        loadingScreen.SetActive(false);

    }
    public void LoadScene(int sceneIndex)
    {
        Time.timeScale = 1;
        StartCoroutine(LoadSceneCoroutine(sceneIndex));
    }
    public IEnumerator LoadSceneCoroutine(int sceneIndex)
    {
        isLoading = true;
        loadingScreen.SetActive(true);
        StartCoroutine(SpinWheel());
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        float elapsedTime = -minLoadTime;

        while (!operation.isDone)
        {
            yield return null;
        }
        while (elapsedTime < minLoadTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        isLoading = false;
        loadingScreen.SetActive(false);
    }
    //loading wheel
    private IEnumerator SpinWheel()
    {
        while (isLoading)
        {
            loadingWheel.transform.Rotate(0, 0, wheelSpeed * Time.deltaTime);
            yield return null;
        }
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
