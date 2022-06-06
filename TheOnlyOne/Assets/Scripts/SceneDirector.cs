using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneDirector : MonoBehaviour
{
    public static SceneDirector instance;
    public GameObject loadingScreen;
    public GameObject loadingWheel;
    public float wheelSpeed;
    public bool isLoading;
    public float minLoadTime = 2;


    private void Awake()
    {//Singleton
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
        float elapsedTime = 0;
        
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
   
    private IEnumerator SpinWheel()
    {
        while (isLoading)
        {
            loadingWheel.transform.Rotate(0, 0, -wheelSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
