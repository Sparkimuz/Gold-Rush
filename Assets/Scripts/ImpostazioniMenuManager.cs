using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ImpostazioniMenuManager : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject profileStatisticLogoutPanel;
    public GameObject authPanel;

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
        SceneManager.LoadScene(2);
    }



}
