﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuFunction : MonoBehaviour
{
    public GameObject shopPanel, settingsMenu, creditPanel, achievementsPanel; // Riferimento al pannello dello shop
    

    void Start()
    {

        /*if (FirebaseController.Instance == null)
        {
            SceneManager.LoadScene(2);
            return;
        }

        // Assicurati che il FirebaseController esista
        if (FirebaseController.Instance == null)
        {
            GameObject firebaseObj = GameObject.Find("FirebaseController");
            if (firebaseObj == null)
            {
                firebaseObj = new GameObject("FirebaseController");
                firebaseObj.AddComponent<FirebaseController>();
            }
        }*/

       





        // Assicurati che il pannello dello shop sia disattivato all'inizio
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
        if (settingsMenu != null)
        {
            settingsMenu.SetActive(false);
        }
    }

    void Update()
    {
        // Eventuali aggiornamenti o logiche aggiuntive
        if (FirebaseController.Instance == null)
        {
            SceneManager.LoadScene(2);
            return;
        }
    }

    public void PlayGame()
    {
        if (FirebaseController.Instance == null)
        {
            Debug.LogError("FirebaseController non trovato! Ricreazione in corso...");
            GameObject firebaseObj = new GameObject("FirebaseController");
            firebaseObj.AddComponent<FirebaseController>();
        }

        SceneManager.LoadScene(1 /*SceneManager.GetActiveScene().buildIndex*/);

        //SceneManager.LoadScene(1, LoadSceneMode.Single);
    }


    public void Restart()
    {
        if (settingsMenu != null)
        {
            settingsMenu.SetActive(false);
        }

        Time.timeScale = 1f; // Assicurati che il tempo riparta

        SceneManager.LoadScene(1, LoadSceneMode.Single); // 🔥 Forza il reset totale della scena
    }



    public void GoToMenu()
    {
        // Ricarica la scena del menu principale (assicurati che sia nella posizione corretta nell'index)
        if (settingsMenu != null)
        {
            settingsMenu.SetActive(false);
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene(0); // Sostituisci 0 con l'index della tua scena principale se necessario
    }

    // Metodo per aprire il pannello dello shop
    public void OpenShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
        }
    }

    // Metodo per chiudere il pannello dello shop
    public void CloseShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
    }


    public void OpenCredits()
    {
        if(creditPanel != null)
            creditPanel.SetActive(true);
    }

    public void CloseCredits()
    {
        if(creditPanel != null)
            creditPanel.SetActive(false);
    }

    public void QuitGame()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
    public void OpenAchievements()
    {
        if(achievementsPanel != null)
            achievementsPanel.SetActive(true);
    }

    public void CloseAchievements()
    {
        if(achievementsPanel != null)
            achievementsPanel.SetActive(false);
    }
}