using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuFunction : MonoBehaviour
{
    public GameObject shopPanel; // Riferimento al pannello dello shop

    void Start()
    {
        // Assicurati che il pannello dello shop sia disattivato all'inizio
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
    }

    void Update()
    {
        // Eventuali aggiornamenti o logiche aggiuntive
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Restart()
    {
        // Ricarica la scena attuale
        SceneManager.LoadScene(1 /*SceneManager.GetActiveScene().buildIndex*/);
    }

    public void GoToMenu()
    {
        // Ricarica la scena del menu principale (assicurati che sia nella posizione corretta nell'index)
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
}