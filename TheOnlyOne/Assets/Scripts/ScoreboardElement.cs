using TMPro;
using UnityEngine;

public class ScoreboardElement : MonoBehaviour
{
    public TMP_Text usernameText;
    public TMP_Text xpText;

    public void NewScoreboardElement(string _username, int _xp)
    {
        usernameText.text = _username;
        xpText.text = _xp.ToString();
    }
}
