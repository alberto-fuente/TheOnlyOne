using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int totalTargets;
    public int remainingTargets;
    public WeaponHolder weaponHolder;
    public GameObject ammoPanel;
    public float gameTimer = 0f;
    public Text timerText;
    public Text currentAmmoText;
    public Text totalAmmoText;

    public bool isSafeToReload;
    // Start is called before the first frame update
    void Start()
    {
        isSafeToReload = true;
        weaponHolder = FindObjectOfType<WeaponHolder>();
        totalTargets = GameObject.FindGameObjectsWithTag("Target").Length;
        remainingTargets = totalTargets;
        Physics.IgnoreLayerCollision(10, 11);//player and bullet
        Physics.IgnoreLayerCollision(7, 11);
    }

    // Update is called once per frame
    void Update()
    {
        timerText.text = gameTimer.ToString("F2");

        if (remainingTargets < totalTargets && remainingTargets!=0)
        {
            gameTimer += Time.deltaTime;
        }

        if (!weaponHolder.isEmpty()&&weaponHolder.GetCurrentItem().typeOfItem.Equals(GameUtils.TypeOfItem.GUN))
        {
            ammoPanel.SetActive(true);
            currentAmmoText.text = weaponHolder.GetCurrentItem().GetComponent<Weapon>().currentAmmo.ToString();
            totalAmmoText.text = weaponHolder.GetCurrentItem().GetComponent<Weapon>().totalAmmo.ToString();
        }
        else
        {
            ammoPanel.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SceneManager.LoadScene(0);
        }
    }
}
