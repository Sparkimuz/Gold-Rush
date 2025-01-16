using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ImpostazioniMenuManager : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject profileStatisticLogoutPanel;
    public GameObject authPanel;
    public TMP_Text profile_UserName_text, profile_UserEmail_text;

    void Start()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        if (profileStatisticLogoutPanel != null)
        {
            profileStatisticLogoutPanel.SetActive(false);
        }


        if (FirebaseController.Instance.profile_UserName_text.text==null)
            profile_UserName_text.text = "ERRORE";
        else
            profile_UserName_text.text = FirebaseController.Instance.profile_UserName_text.text;



        if (FirebaseController.Instance.profile_UserEmail_text.text == null)
            profile_UserEmail_text.text = "ERRORE";
        else
            profile_UserEmail_text.text = FirebaseController.Instance.profile_UserEmail_text.text;


        //profile_UserName_text.text = FirebaseController.Instance.profile_UserName_text.text;
        //profile_UserEmail_text.text = FirebaseController.Instance.profile_UserEmail_text.text;
        //profile_UserEmail_text.text = "a";
        //profile_UserName_text.text = "b";

    }


    public void OpenSettings()
    {
        settingsPanel.SetActive(true);   // Mostra il pannello delle impostazioni

    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);   // Nascondi il pannello delle impostazioni
    }


    public void OpenProfile()
    {
        CloseSettings();
        profileStatisticLogoutPanel.SetActive(true);
    }

    public void CloseProfile()
    {   

        profileStatisticLogoutPanel.SetActive(false);
        OpenSettings();
    }


    public void logOut()
    {
        profileStatisticLogoutPanel?.SetActive(false);
        mainMenuPanel.SetActive(false);
        DestroyFirebaseController();
        SceneManager.LoadScene(2);
    }

    void DestroyFirebaseController()
    {
        if (FirebaseController.Instance != null)
        {
            Destroy(FirebaseController.Instance.gameObject);
            FirebaseController.Instance = null; // Assicurati di rimuovere il riferimento statico
        }
    }



}
