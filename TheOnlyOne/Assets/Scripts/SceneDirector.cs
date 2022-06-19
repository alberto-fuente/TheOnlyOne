using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDirector : MonoBehaviour
{
    //Singleton
    public static SceneDirector instance;
    [Header("Components")]
    public GameObject loadingScreen;
    public GameObject loadingWheel;
    [SerializeField] private float wheelSpeed;
    [SerializeField] private float minLoadTime = 2;

    public bool isLoading;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
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
