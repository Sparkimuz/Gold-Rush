using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FirebaseController : MonoBehaviour


{
    public GameObject loginPanel, profilePanel, signupPanel, forgetPasswordPanel, notificationPanel;

    public TMP_InputField loginEmail, loginPassword, signupEmail, signupPassword, signupConfirmPassword, signupUsername, forgetPassEmail;

    public TMP_Text notif_Title_text, notif_Message_text, profile_UserName_text, profile_UserEmail_text;

    public Toggle rememberMe;




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
    }


    public void forgetPassword()
    {
        if(string.IsNullOrEmpty(forgetPassEmail.text)) {
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
        profile_UserEmail_text.text = "";
        profile_UserName_text.text = "";
        openLoginPanel();
    }
}
