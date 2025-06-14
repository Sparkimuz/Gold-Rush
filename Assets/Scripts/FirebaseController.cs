﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Net.Mail;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;
using System;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;


public class FirebaseController : MonoBehaviour


{
    public static event Action OnUserUpdated;

    public static FirebaseController Instance;

    public GameObject mainMenu, settingsMenu;

    public GameObject loginPanel, profilePanel, signupPanel, forgetPasswordPanel, notificationPanel;

    public TMP_InputField loginEmail, loginPassword, signupEmail, signupPassword, signupConfirmPassword, signupUsername, forgetPassEmail;

    public TMP_Text notif_Title_text, notif_Message_text, profile_UserName_text, profile_UserEmail_text;

    public Toggle rememberMe;

    public Firebase.Auth.FirebaseAuth auth;
    public Firebase.Auth.FirebaseUser user;
    public DatabaseReference dbReference;

    bool isSignIn = false;
    bool isSigned = false;


    void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                InitializeFirebase();

                // 🔥 Se siamo nella scena di autenticazione, riattiviamo la UI
                if (SceneManager.GetActiveScene().buildIndex == 2)
                {
                    Debug.Log("🎯 Siamo nella scena di autenticazione, mostro il login.");
                    openLoginPanel();
                }
            }
            else
            {
                Debug.LogError($"Could not resolve Firebase dependencies: {dependencyStatus}");
            }
        });
    }




    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("🔥 FirebaseController creato e mantenuto.");
        }
        else
        {
            if (SceneManager.GetActiveScene().buildIndex != 2) // NON distruggere in LoginScene
            {
                Debug.Log("FirebaseController già esistente, distruggo il duplicato.");
                Destroy(gameObject);
            }
        }
    }



    public void openForgetPasswordPanel()
    {
        loginPanel.SetActive(false);
        profilePanel.SetActive(false);
        signupPanel.SetActive(false);
        forgetPasswordPanel.SetActive(true);
    }

    public void openMainMenu()
    {
        loginPanel.SetActive(false);
        profilePanel.SetActive(false);
        mainMenu.SetActive(true);
    }
    public void openLoginPanel()
    {
        forgetPasswordPanel.SetActive(false);
        profilePanel.SetActive(false);
        signupPanel.SetActive(false);
        loginPanel.SetActive(true);
    }



    public void openProfilePanel()
    {
        forgetPasswordPanel.SetActive(false);
        loginPanel.SetActive(false);
        signupPanel.SetActive(false);
        profilePanel.SetActive(true);
    }

    public void closeProfilePanel()
    {
        profilePanel.SetActive(false);
    }

    public void openSignupPanel()
    {
        forgetPasswordPanel.SetActive(false);
        loginPanel.SetActive(false);
        profilePanel.SetActive(false);
        signupPanel.SetActive(true);
    }

    public void loginUser()
    {
        if (string.IsNullOrEmpty(loginEmail.text))
        {
            showNotificationMessage("Email Error", "Please enter an Email!");
            return;
        }
        else if (string.IsNullOrEmpty(loginPassword.text))
        {
            showNotificationMessage("Password Error", "Please enter a password!");
            return;
        }

        //do login

        signInUser(loginEmail.text, loginPassword.text);
        //SceneManager.LoadScene(0);
        //loginPanel.SetActive(false);
        //profilePanel.SetActive(true);
    }

    public void signupUser()
    {
        if (string.IsNullOrEmpty(signupUsername.text))
        {
            showNotificationMessage("Username Error", "Please enter an Username!");
            return;
        }
        else if (string.IsNullOrEmpty(signupEmail.text))
        {
            showNotificationMessage("Email Error", "Please enter an Email!");
            return;
        }
        if (string.IsNullOrEmpty(signupPassword.text))
        {
            showNotificationMessage("Password Error", "Please enter a Password!");
            return;
        }
        if (string.IsNullOrEmpty(signupConfirmPassword.text))
        {
            showNotificationMessage("Password Error", "Please confirm the Password");
            return;
        }

        //do signup

        CreateUser(signupEmail.text, signupPassword.text, signupUsername.text);

    }


    public void forgetPassword()
    {
        if (string.IsNullOrEmpty(forgetPassEmail.text))
        {
            showNotificationMessage("Error", "Forget Email Empty");
            return;
        }

        forgetPasswordSubmit(forgetPassEmail.text);
    }


    private void showNotificationMessage(string title, string message)
    {
        notif_Title_text.text = "" + title;
        notif_Message_text.text = "" + message;

        notificationPanel.SetActive(true);
    }


    public void closeNotificationPanel()
    {
        notif_Title_text.text = "";
        notif_Message_text.text = "";

        notificationPanel.SetActive(false);
    }


    public void logOut()
    {
        if (auth != null)
        {
            auth.SignOut();
            Debug.Log("✅ Utente disconnesso correttamente.");
        }

        user = null; // Resetta l'utente
        PlayerPrefs.DeleteKey("selectedCharacterIndex"); // Cancella i dati locali salvati

        // 🔥 Disattiva tutti i pannelli prima di caricare la scena di login
        if (loginPanel) loginPanel.SetActive(false);
        if (profilePanel) profilePanel.SetActive(false);
        if (signupPanel) signupPanel.SetActive(false);
        if (forgetPasswordPanel) forgetPasswordPanel.SetActive(false);
        if (notificationPanel) notificationPanel.SetActive(false);

        // 🔄 Ricarica Firebase nella scena di autenticazione
        SceneManager.LoadScene(2);
    }







    void CreateUser(string email, string password, string userName)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);

                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                {
                    Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                    if (firebaseEx != null)
                    {
                        var errorCode = (AuthError)firebaseEx.ErrorCode;
                        showNotificationMessage("Error", GetErrorMessage(errorCode));
                    }
                }

                return;
            }

            // Firebase user has been created.
            Firebase.Auth.AuthResult result = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);

            updateUserProfile(userName);
            signupPanel.SetActive(false);
            loginPanel.SetActive(true);
        });
    }

   public void signInUser(string email, string password)
{
    auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
    {
        if (task.IsCanceled)
        {
            Debug.LogError("❌ Login annullato.");
            showNotificationMessage("Login Failed", "Login was canceled.");
            return;
        }
        if (task.IsFaulted)
        {
            Debug.LogError("❌ Errore nel login: " + task.Exception);

            foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
            {
                Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                if (firebaseEx != null)
                {
                    var errorCode = (AuthError)firebaseEx.ErrorCode;
                    string errorMessage = GetErrorMessage(errorCode);
                    showNotificationMessage("Login Failed", errorMessage);
                    return;
                }
            }

            // Se non si riesce a estrarre il codice di errore, mostra un messaggio generico
            showNotificationMessage("Login Failed", "An unknown error occurred.");
            return;
        }

        Firebase.Auth.AuthResult result = task.Result;
        user = result.User;

        Debug.Log("✅ Login riuscito: " + user.DisplayName + " | " + user.Email);

        // Notifica immediatamente gli altri script che l'utente è stato caricato
        OnUserUpdated?.Invoke();

        SceneManager.LoadScene(0); // Torna al menu principale
    });
}



    private static string GetErrorMessage(AuthError errorCode)
    {
        var message = "";
        switch (errorCode)
        {
            case AuthError.AccountExistsWithDifferentCredentials:
                message = "Account not exist";
                break;
            case AuthError.MissingPassword:
                message = "Missing password";
                break;
            case AuthError.WeakPassword:
                message = "Password too weak";
                break;
            case AuthError.WrongPassword:
                message = "Wrong Password";
                break;
            case AuthError.EmailAlreadyInUse:
                message = "Your Email is already in use";
                break;
            case AuthError.InvalidEmail:
                message = "Your email is invalid";
                break;
            case AuthError.MissingEmail:
                message = "Email missing";
                break;
            default:
                message = "Invalid error";
                break;
        }
        return message;
    }




    void InitializeFirebase()
    {
        Debug.Log("🟢 Inizializzando Firebase...");

        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

        // URL manuale del database Firebase
        string databaseUrl = "https://fir-goldrush-default-rtdb.europe-west1.firebasedatabase.app/";

        FirebaseApp app = FirebaseApp.DefaultInstance;
        dbReference = FirebaseDatabase.GetInstance(app, databaseUrl).RootReference;

        if (dbReference != null)
        {
            Debug.Log("✅ Firebase Database inizializzato correttamente con URL: " + databaseUrl);
        }
        else
        {
            Debug.LogError("❌ Errore nell'inizializzazione del Database Firebase.");
        }

        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);

        Debug.Log("🔥 Firebase completamente inizializzato!");
    }






    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        FirebaseUser newUser = auth.CurrentUser;

        if (newUser != user)
        {
            bool signedIn = newUser != null && newUser.IsValid();
            if (!signedIn && user != null)
            {
                Debug.Log("🚪 Utente disconnesso, ricarico Firebase.");
                FirebaseController.Instance = null; // Rimuovi la reference globale
            }
            user = newUser;
        }

        // 🔥 Avvisa gli altri script che Firebase è cambiato
        OnUserUpdated?.Invoke();
    }






    void OnDestroy()
    {
        if (auth != null)
        {
            auth.StateChanged -= AuthStateChanged;
            auth = null;
        }
    }


    public void updateUserProfile(string userName)
    {
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            Firebase.Auth.UserProfile profile = new Firebase.Auth.UserProfile
            {
                DisplayName = userName,
                PhotoUrl = new System.Uri("https://example.com/jane-q-user/profile.jpg"),
            };
            user.UpdateUserProfileAsync(profile).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("UpdateUserProfileAsync cancellato.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("UpdateUserProfileAsync ha riscontrato un errore: " + task.Exception);
                    return;
                }

                Debug.Log("User profile aggiornato correttamente.");

                showNotificationMessage("alert", "Account Creato correttamente!");
            });

        }
    }

    void forgetPasswordSubmit(string forgetPasswordEmail)
    {
        auth.SendPasswordResetEmailAsync(forgetPasswordEmail).ContinueWithOnMainThread(task =>
        {

            if (task.IsCanceled)
            {
                Debug.LogError("SendPasswordResetEmailAsync was canceled");
            }

            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);

                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                {
                    Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                    if (firebaseEx != null)
                    {
                        var errorCode = (AuthError)firebaseEx.ErrorCode;
                        showNotificationMessage("Error", GetErrorMessage(errorCode));
                    }
                }
            }

            showNotificationMessage("Alert", "Succesfully send Email for reset Password!");
        });
    }



    public void LoadCoins(Action<int> callback)
    {
        if (dbReference == null)
        {
            Debug.LogError("❌ dbReference è NULL! Provo a reinizializzare Firebase...");
            InitializeFirebase();
            return;
        }

        if (user == null)
        {
            Debug.LogError("❌ Nessun utente autenticato! Assicurati di essere loggato prima di caricare le monete.");
            return;
        }

        string userId = user.UserId;
        dbReference.Child("users").Child(userId).Child("coins").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                int coins = int.Parse(task.Result.Value.ToString());
                Debug.Log("💰 Monete caricate da Firebase: " + coins);
                callback?.Invoke(coins);
            }
            else
            {
                Debug.Log("⚠️ Nessun valore trovato, lascio inalterato.");
                callback?.Invoke(0);
            }
        });
    }



    public async Task SaveCoins(int coinAmount)
    {
        if (user != null)
        {
            string userId = user.UserId;

            try
            {
                // Controlla il valore attuale prima di aggiornare
                DataSnapshot snapshot = await dbReference.Child("users").Child(userId).Child("coins").GetValueAsync();
                if (snapshot.Exists)
                {
                    int currentCoins = int.Parse(snapshot.Value.ToString());
                    if (currentCoins != coinAmount) // Solo se il valore è cambiato
                    {
                        await dbReference.Child("users").Child(userId).Child("coins").SetValueAsync(coinAmount);
                        Debug.Log("✅ Monete aggiornate: " + coinAmount);
                    }
                    else
                    {
                        Debug.Log("🔹 Monete non cambiate, nessun aggiornamento.");
                    }
                }
                else
                {
                    // Se il valore non esiste, lo inizializziamo
                    await dbReference.Child("users").Child(userId).Child("coins").SetValueAsync(coinAmount);
                    Debug.Log("✅ Monete inizializzate: " + coinAmount);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("❌ Errore nel salvataggio delle monete: " + e.Message);
            }
        }
    }



    public void LoadDisRecord(Action<int> callback)
    {
        if (dbReference == null)
        {
            Debug.LogError("❌ dbReference è NULL! Provo a reinizializzare Firebase...");
            InitializeFirebase();
            return;
        }

        if (user == null)
        {
            Debug.LogError("❌ Nessun utente autenticato! Assicurati di essere loggato prima di caricare la distanza.");
            return;
        }

        string userId = user.UserId;
        dbReference.Child("users").Child(userId).Child("distanceRecord").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                int distanceRecord = int.Parse(task.Result.Value.ToString());
                Debug.Log("Distanza caricata da Firebase: " + distanceRecord);
                callback?.Invoke(distanceRecord);
            }
            else
            {
                Debug.Log("⚠️ Nessun valore trovato, lascio inalterato.");
                callback?.Invoke(0);
            }
        });
    }


    public async Task SaveDisRecord(int endDistance)
    {
        if (user != null)
        {
            string userId = user.UserId;

            try
            {
                // Controlla il valore attuale prima di aggiornare
                DataSnapshot snapshot = await dbReference.Child("users").Child(userId).Child("distanceRecord").GetValueAsync();
                if (snapshot.Exists)
                {
                    int distanceRecord = int.Parse(snapshot.Value.ToString());
                    if (distanceRecord < endDistance) // Solo se il valore è cambiato
                    {
                        await dbReference.Child("users").Child(userId).Child("distanceRecord").SetValueAsync(endDistance);
                        Debug.Log("✅ distanza aggiornata: " + endDistance);
                    }
     
                }
                else
                {
                    // Se il valore non esiste, lo inizializziamo
                    await dbReference.Child("users").Child(userId).Child("distanceRecord").SetValueAsync(endDistance);
                    Debug.Log("✅ distanza inizializzata: " + endDistance);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("❌ Errore nel salvataggio della distanza: " + e.Message);
            }
        }
    }




    void Update()
    {
        if (isSignIn)
        {
            if(!isSigned)
            {
                isSigned = true;
                profile_UserName_text.text = user.DisplayName;
                profile_UserEmail_text.text = user.Email;
                openProfilePanel();
            }
        }
    }

        public void LoadDistanceTotal(Action<int> callback)
    {
        if (dbReference == null || user == null)
        {
            Debug.LogError("❌ dbReference o user null, impossibile caricare distanceTotal.");
            callback?.Invoke(0);
            return;
        }

        string userId = user.UserId;
        dbReference.Child("users").Child(userId).Child("distanceTotal").GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && task.Result.Exists)
                {
                    int distTotal = int.Parse(task.Result.Value.ToString());
                    callback?.Invoke(distTotal);
                }
                else
                {
                    // Se non esiste, consideriamo 0
                    callback?.Invoke(0);
                }
            });
    }

    public async Task AddToTotalDistance(int distanceToAdd)
    {
        if (user == null || dbReference == null)
        {
            Debug.LogError("❌ user o dbReference null, impossibile aggiungere distanza!");
            return;
        }

        string userId = user.UserId;
        // Leggi il valore attuale
        DataSnapshot snapshot = await dbReference.Child("users").Child(userId).Child("distanceTotal").GetValueAsync();
        int currentTotal = 0;
        if (snapshot.Exists)
        {
            currentTotal = int.Parse(snapshot.Value.ToString());
        }

        int newTotal = currentTotal + distanceToAdd;
        await dbReference.Child("users").Child(userId).Child("distanceTotal").SetValueAsync(newTotal);
        Debug.Log($"✅ distanceTotal aggiornata: da {currentTotal} a {newTotal}");
    }

    public void LoadCoinsAllTime(Action<int> callback)
    {
        if (dbReference == null || user == null)
        {
            Debug.LogError("❌ dbReference o user null, impossibile caricare coinsAllTime.");
            callback?.Invoke(0);
            return;
        }

        string userId = user.UserId;
        dbReference.Child("users").Child(userId).Child("coinsAllTime").GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && task.Result.Exists)
                {
                    int c = int.Parse(task.Result.Value.ToString());
                    callback?.Invoke(c);
                }
                else
                {
                    callback?.Invoke(0);
                }
            });
    }

    public async Task AddToAllTimeCoins(int coinsToAdd)
    {
        if (user == null || dbReference == null)
        {
            Debug.LogError("❌ user o dbReference null, impossibile aggiungere monete storiche!");
            return;
        }

        string userId = user.UserId;
        // Leggi il valore attuale
        DataSnapshot snapshot = await dbReference.Child("users").Child(userId).Child("coinsAllTime").GetValueAsync();
        int currentAllTime = 0;
        if (snapshot.Exists)
        {
            currentAllTime = int.Parse(snapshot.Value.ToString());
        }

        int newTotal = currentAllTime + coinsToAdd;
        await dbReference.Child("users").Child(userId).Child("coinsAllTime").SetValueAsync(newTotal);
        Debug.Log($"✅ coinsAllTime aggiornata: da {currentAllTime} a {newTotal}");
    }

}
