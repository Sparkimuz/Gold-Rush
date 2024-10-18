using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Net.Mail;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System;
using System.Threading.Tasks;

public class FirebaseController : MonoBehaviour


{
    public GameObject loginPanel, profilePanel, signupPanel, forgetPasswordPanel, notificationPanel;

    public TMP_InputField loginEmail, loginPassword, signupEmail, signupPassword, signupConfirmPassword, signupUsername, forgetPassEmail;

    public TMP_Text notif_Title_text, notif_Message_text, profile_UserName_text, profile_UserEmail_text;

    public Toggle rememberMe;

    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;

    bool isSignIn = false;
    bool isSigned = false;


    void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                InitializeFirebase();

                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    public void openForgetPasswordPanel()
    {
        loginPanel.SetActive(false);
        profilePanel.SetActive(false);
        signupPanel.SetActive(false);
        forgetPasswordPanel.SetActive(true);
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

        CreateUser(signupEmail.text, signupPassword.text, signupPassword.text);
    }


    public void forgetPassword()
    {
        if (string.IsNullOrEmpty(forgetPassEmail.text))
        {
            showNotificationMessage("Error", "Forget Email Empty");
            return;
        }
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
        auth.SignOut();
        profile_UserEmail_text.text = "";
        profile_UserName_text.text = "";
        openLoginPanel();
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
                return;
            }

            // Firebase user has been created.
            Firebase.Auth.AuthResult result = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);

            updateUserProfile(userName);
        });
    }

    public void signInUser(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.AuthResult result = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);

            profile_UserName_text.text = result.User.DisplayName;
            profile_UserEmail_text.text = result.User.Email;
            openProfilePanel();
            
        });

    }


    void InitializeFirebase()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null
                && auth.CurrentUser.IsValid();
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
                isSignIn = true;
            }
        }
    }

    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
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
                    Debug.LogError("UpdateUserProfileAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("User profile updated successfully.");

                showNotificationMessage("alert", "Account Created Succesfully!");
            });

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
}
