using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public GameObject settingsPanel;  // Il pannello delle impostazioni
    public Slider volumeSlider;       // Slider per il volume (se decidi di aggiungerlo)
    public AudioSource gameAudioSource; // L'audio del gioco
    private bool isPaused = false;    // Stato del gioco, per sapere se è in pausa o meno

    void Start()
    {
        // Assicurati che il pannello delle impostazioni sia inizialmente nascosto
        settingsPanel.SetActive(false);
    }

    // Metodo per aprire il pannello delle impostazioni e mettere il gioco in pausa
public void OpenSettings()
{
    if (isPaused) return;  // Evita di riaprire il pannello se il gioco è già in pausa

    settingsPanel.SetActive(true);
    Time.timeScale = 0f;
    isPaused = true;
}

public void CloseSettings()
{
    if (!isPaused) return;  // Evita di chiudere le impostazioni se il gioco non è in pausa

    settingsPanel.SetActive(false);
    Time.timeScale = 1f;
    isPaused = false;
}

    // Metodo per cambiare la lingua (usato dalle bandiere)
    public void ChangeLanguage(int languageIndex)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[languageIndex];
        Debug.Log("Lingua cambiata a: " + LocalizationSettings.SelectedLocale.LocaleName);
    }

    // Metodo per modificare il volume tramite uno slider (opzionale)
    public void AdjustVolume(float volume)
    {
        gameAudioSource.volume = volume;
        Debug.Log("Volume regolato a: " + volume);
    }

    // Metodo per mutare/smutare il volume (alternativa allo slider)
    public void ToggleMute()
    {
        gameAudioSource.mute = !gameAudioSource.mute;
        Debug.Log("Audio muto: " + gameAudioSource.mute);
    }

    // Metodo per il pulsante "Riprendi", che chiude le impostazioni e continua il gioco
    public void ResumeGame()
    {
        CloseSettings();  // Chiude il pannello e riprende il gioco
        Debug.Log("Gioco ripreso dal pulsante 'Riprendi'.");
    }
}