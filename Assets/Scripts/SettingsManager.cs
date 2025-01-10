using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public GameObject settingsPanel;  // Il pannello delle impostazioni
    private bool isSettingsOpen = false; // Controlla se la schermata delle impostazioni è aperta

    // Metodo di inizializzazione
    void Start()
    {
        // All'avvio, disattiva il pannello delle impostazioni
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    // Metodo per aprire/chiudere le impostazioni
    public void ToggleSettings()
    {
        if (isSettingsOpen)
        {
            // Se le impostazioni sono aperte, chiudi le impostazioni e riprendi il gioco
            CloseSettings();
        }
        else
        {
            // Se le impostazioni sono chiuse, aprile
            OpenSettings();
        }
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);   // Mostra il pannello delle impostazioni
        isSettingsOpen = true;
        PauseGame(); // Mette in pausa il gioco
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);   // Nascondi il pannello delle impostazioni
        isSettingsOpen = false;
        ResumeGame(); // Riprende il gioco
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