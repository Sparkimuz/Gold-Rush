using System;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public GameObject settingsPanel;  // Il pannello delle impostazioni

    // Metodo di inizializzazione
    void Start()
    {
        
        // Assicurati che il FirebaseController esista
        if (FirebaseController.Instance == null)
        {
            GameObject firebaseObj = GameObject.Find("FirebaseController");
            if (firebaseObj == null)
            {
                firebaseObj = new GameObject("FirebaseController");
                firebaseObj.AddComponent<FirebaseController>();
            }
        }


        // All'avvio, disattiva il pannello delle impostazioni
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);   // Mostra il pannello delle impostazioni
        PauseGame(); // Mette in pausa il gioco
    }

    public void CloseSettings()
    {
        ResumeGame(); // Riprende il gioco
        settingsPanel.SetActive(false);   // Nascondi il pannello delle impostazioni
        
    }

    // Metodo per mettere in pausa il gioco
    private void PauseGame()
    {
        Time.timeScale = 0f; // Ferma il tempo di gioco
    }

    // Metodo per riprendere il gioco
    public void ResumeGame()
    {
        Time.timeScale = 1f; // Riprendi il tempo di gioco
    }

    // Metodo per il pulsante "Continua"
    public void OnContinueButtonClicked()
    {
        // Chiude il pannello delle impostazioni e riprende il gioco
        CloseSettings();
    }

}