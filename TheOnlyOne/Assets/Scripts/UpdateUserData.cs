using Firebase.Database;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class UpdateUserData : MonoBehaviour
{
    [Header("User data")]
    public TMP_Text usernameField;
    public TMP_Text xpField;

    [Header("Scoreboard data")]
    public GameObject scoreboardUI;
    public GameObject scoreElement;
    public Transform scoreboardContent;
    private void OnEnable()
    {
        StartCoroutine(UpdateVisualUserData());
    }
    private IEnumerator UpdateVisualUserData()
    {
        var user = FirebaseManager.User;
        var DBTask = FirebaseManager.DBReference.Child("players").Child(user.UserId).Child("xp").GetValueAsync();//get player's stored xp
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Tarea fallida con {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            // Data has not been found
            usernameField.text = "";
            xpField.text = "0";
        }
        else
        {
            // Data has been found
            DataSnapshot snapshot = DBTask.Result;
            usernameField.text = "   hola, " + user.DisplayName;//player's stored username
            xpField.text = snapshot.Value.ToString();
        }
    }
    public void ScoreboardButton()
    {
        StartCoroutine(LoadScoreboardData());
    }
    private IEnumerator LoadScoreboardData()
    {
        //Get all the users data ordered by kills amount
        var DBTask = FirebaseManager.DBReference.Child("players").OrderByChild("xp").GetValueAsync();//order data from low to high

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Tarea fallida con {DBTask.Exception}");
        }
        else
        {
            //Data has been found
            DataSnapshot snapshot = DBTask.Result;

            //Destroy any existing scoreboard elements
            foreach (Transform child in scoreboardContent.transform)
            {
                Destroy(child.gameObject);
            }
            int pLayersLeft = 10;
            //Loop through every user's UID
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())//from high to low
            {
                if (pLayersLeft <= 0) break;//max 10 players shown

                string username = childSnapshot.Child("username").Value.ToString();
                int xp = int.Parse(childSnapshot.Child("xp").Value.ToString());

                //Instantiate new scoreboard elements
                GameObject scoreboardElement = Instantiate(scoreElement, scoreboardContent);
                scoreboardElement.GetComponent<ScoreboardElement>().NewScoreboardElement(username, xp);
                pLayersLeft--;
            }
            //show scoreboard screen
            scoreboardUI.SetActive(true);
        }
    }
    //sign out the current account
    public void SignOut()
    {
        var auth = FirebaseManager.Auth;
        auth.SignOut();
        SceneDirector.instance.LoadScene(0);
    }
}
