using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System.Collections;
using TMPro;
using UnityEngine;
public class FirebaseManager : MonoBehaviour
{
    // Firebase variables
    [Header("Firebase References")]
    public DependencyStatus dependencyStatus;
    public static FirebaseAuth Auth;
    public static FirebaseUser User;
    public static DatabaseReference DBReference;

    // Login variables
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;

    // Register variables
    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;

    //Screens
    [Header("Canvas")]
    public GameObject loginCanvas;
    public GameObject registerCanvas;

    private void Start()
    {
        // Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // If they are avalible Initialize Firebase
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("No se pudieron resolver todas las dependencias de Firebase: " + dependencyStatus);

            }
        });
    }
    //Initialize DataBase
    private void InitializeFirebase()
    {
        // Set the authentication instance object
        Auth = FirebaseAuth.DefaultInstance;
        DBReference = FirebaseDatabase.DefaultInstance.RootReference;
    }
    // Logic for the register button
    public void RegisterButton()
    {
        // Call the register coroutine passing the email,password,and username
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }
    // Logic for the login button
    public void LoginButton()
    {
        // Call the login coroutine passing the email and password
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }
    public void ClearRegisterFeilds()
    {
        usernameRegisterField.text = "";
        emailRegisterField.text = "";
        passwordRegisterField.text = "";
        passwordRegisterVerifyField.text = "";
    }
    public void ClearLoginFeilds()
    {
        emailLoginField.text = "";
        passwordLoginField.text = "";
    }
    //Creates row in database with two fields
    public void CreatePlayerRow(string userID, string username, int xp)
    {
        DBReference.Child("players").Child(userID).Child("username").SetValueAsync(username);
        DBReference.Child("players").Child(userID).Child("xp").SetValueAsync(xp);
    }
    //Register Corroutine
    private IEnumerator Register(string _email, string _password, string _username)
    {
        if (_username == "")
        {
            warningRegisterText.text = "Introduce un nombre de usuario";
        }
        else if (passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            warningRegisterText.text = "Las contraseñas no coinciden";
        }
        else
        {
            //Call the Firebase auth signin function passing the email and password
            var RegisterTask = Auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            //Wait until the task completes
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                Debug.LogWarning(message: $"Tarea de registro fallida con {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                warningLoginText.text = "";
                string message = "Registro fallido";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Introduce un correo";
                        break;
                    case AuthError.MissingPassword:
                        message = "Introduce una contraseña";
                        break;
                    case AuthError.WeakPassword:
                        message = "Contraseña demasiado débil";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "El correo ya está en uso";
                        break;
                }
                warningRegisterText.text = message;
            }
            else
            {
                //User registered successfully!
                User = RegisterTask.Result;
                if (User != null)
                {
                    //Create a user profile and set the username
                    UserProfile profile = new UserProfile { DisplayName = _username };
                    //Call the Firebase auth update user profile function passing the profile with the username
                    var ProfileTask = User.UpdateUserProfileAsync(profile);
                    //Wait until the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        warningRegisterText.text = "Registro fallido";
                    }
                    else
                    {
                        // User has been registered in successfully!
                        CreatePlayerRow(User.UserId, User.DisplayName, 0);
                        registerCanvas.SetActive(false);
                        loginCanvas.SetActive(true);
                        warningRegisterText.text = "";
                    }
                }
            }
        }
    }
    //Login Corroutine
    private IEnumerator Login(string _email, string _password)
    {
        // Call the Firebase auth signin function passing the email and password.
        var LoginTask = Auth.SignInWithEmailAndPasswordAsync(_email, _password);
        // Wait until the task completes
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);
        if (LoginTask.Exception != null)
        {
            Debug.LogWarning(message: $"Tarea de inicio de sesión fallida con {LoginTask.Exception}");
            FirebaseException firebaseExcetion = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseExcetion.ErrorCode;
            warningLoginText.text = "";
            string message = "Inicio de sesión fallido";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Introduce el correo";
                    break;
                case AuthError.MissingPassword:
                    message = "Introduce la contraseña";
                    break;
                case AuthError.WrongPassword:
                    message = "Contraseña incorrecta";
                    break;
                case AuthError.InvalidEmail:
                    message = "Correo no válido";
                    break;
                case AuthError.UserNotFound:
                    message = "La cuenta no existe";
                    break;
            }
            warningLoginText.text = message;
        }
        else
        {
            // User has logged in successfully!
            User = LoginTask.Result;
            warningLoginText.text = "";
            confirmLoginText.text = "Bienvenido/a, " + User.DisplayName;

            yield return new WaitForSeconds(1.5f);
            ClearRegisterFeilds();
            ClearLoginFeilds();
            confirmLoginText.text = "";
            //Go to Main Menu Screen 
            SceneDirector.Instance.LoadScene(1);

        }
    }
}
