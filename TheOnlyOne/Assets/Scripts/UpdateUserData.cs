using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
public class UpdateUserData : MonoBehaviour
{
    public TMP_Text usernameField;
    public TMP_Text xpField;
    private void OnEnable()
    {
        Debug.Log("Updating player's xp...");
        StartCoroutine(UpdateVisualUserData());

    }
    private IEnumerator UpdateVisualUserData()
    {
        var user = FirebaseManager.User;
        var DBTask = FirebaseManager.DBReference.Child("players").Child(user.UserId).Child("xp").GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with{DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            // No data exists yet
            usernameField.text = "";
            xpField.text = "0";
        }
        else
        {
            // Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;
            usernameField.text = user.DisplayName;

            Debug.Log(snapshot.Value.ToString());
            xpField.text = snapshot.Value.ToString();
        }
    }
}
