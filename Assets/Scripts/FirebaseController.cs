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
        if (string.IsNullOrEmpty(loginEmail.text) && string.IsNullOrEmpty(loginPassword.text))
        {
            showNotificationMessage("Error", "Fields empty!, please input details in all fields");
            return;
        }

        //do login
    }

    public void signupUser()
    {
        if (string.IsNullOrEmpty(signupEmail.text) && string.IsNullOrEmpty(signupPassword.text) &&
            string.IsNullOrEmpty(signupConfirmPassword.text) && string.IsNullOrEmpty(signupUsername.text))
        {
            showNotificationMessage("Error", "Fields empty!, please input details in all fields");
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
