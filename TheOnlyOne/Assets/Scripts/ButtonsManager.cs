using UnityEngine;

public class ButtonsManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject MainMenuScreen;
    public GameObject OptionsMenuScreen;
    public GameObject ScoreBoardScreen;
    public GameObject LoginScreen;
    public GameObject RegisterScreen;
    public GameObject PauseMenu;

    [Header("DataBase Reference")]
    public GetUsersData updateUserData;
    public FirebaseManager firebaseManager;

    [Header("Pause Menu Reference")]
    public PauseMenu pauseMenuScript;

    public void HoverButton()
    {
        AudioManager.Instance.HoverButton();
    }
    public void PressPlayButton()
    {
        AudioManager.Instance.PressButton();
        SceneDirector.Instance.LoadScene((int)GameUtils.SceneIndex.GAME);
    }
    public void PressDropDownButton()
    {
        AudioManager.Instance.PressDropDownButton();
    }
    /////////////////Main Menu/////////////////////////
    public void PressOptionsButton(bool enter)
    {
        AudioManager.Instance.PressButton();
        MainMenuScreen.SetActive(!enter);
        OptionsMenuScreen.SetActive(enter);
    }
    public void PressScoreboardButton(bool enter)
    {
        AudioManager.Instance.PressButton();
        updateUserData.ScoreboardButton();
        MainMenuScreen.SetActive(!enter);
        ScoreBoardScreen.SetActive(enter);

    }
    public void PressSignOutButton()
    {
        AudioManager.Instance.PressButton();
        updateUserData.SignOut();
        SceneDirector.Instance.LoadScene((int)GameUtils.SceneIndex.LOGIN);
    }
    public void PressExitButton()
    {
        AudioManager.Instance.PressButton();
        SceneDirector.Instance.QuitGame();
    }

    /////////////////Login/////////////////////////
    public void PressLoginButton()
    {
        AudioManager.Instance.PressButton();
        firebaseManager.LoginButton();
    }
    public void PressRegisterButton()
    {
        AudioManager.Instance.PressButton();
        firebaseManager.RegisterButton();

    }
    public void PressRegisterScreenButton(bool enter)
    {
        AudioManager.Instance.PressButton();
        RegisterScreen.SetActive(enter);
        LoginScreen.SetActive(!enter);

    }
    /////////////////Game/////////////////////////
    public void PressExitGame()
    {
        AudioManager.Instance.PressButton();
        SceneDirector.Instance.LoadScene((int)GameUtils.SceneIndex.MAINMENU);
    }
    public void PressOptionsInGame(bool enter)
    {
        AudioManager.Instance.PressButton();
        OptionsMenuScreen.SetActive(enter);
        PauseMenu.SetActive(!enter);
    }
    public void ResumeGame()
    {
        AudioManager.Instance.PressButton();
        pauseMenuScript.Resume();
    }
}
