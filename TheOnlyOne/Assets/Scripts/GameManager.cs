using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int totalTargets;
    public int remainingTargets;

    public GameObject ammoPanel;
    public float gameTimer = 0f;
    public Text timerText;
    public Text currentAmmoText;
    public Text totalAmmoText;
    // Start is called before the first frame update
    void Start()
    {
        totalTargets = GameObject.FindGameObjectsWithTag("Target").Length;
        remainingTargets = totalTargets;
    }

    // Update is called once per frame
    void Update()
    {
        timerText.text = gameTimer.ToString("F2");

        if (remainingTargets < totalTargets && remainingTargets!=0)
        {
            gameTimer += Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SceneManager.LoadScene(0);
        }
    }
}
