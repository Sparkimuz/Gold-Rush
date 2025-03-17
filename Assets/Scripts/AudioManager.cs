using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource bgmSource;  // AudioSource per la musica di sottofondo
    public AudioSource[] sfxSources;  // Array di AudioSource per gli effetti sonori
    
    [Header("UI Elements")]
    public Slider bgmSlider;  // Slider per controllare il volume della musica
    public Slider sfxSlider;  // Slider per controllare il volume degli effetti sonori

    [Header("SFX Clips")]
    public AudioClip buttonClickClip;  // ← Dragga qui la clip del click
    public AudioClip gameOverClip;  // ← Dragga qui la clip di game over

    void Start()
    {
        // Impostiamo i valori iniziali degli slider
        if (bgmSource != null && bgmSlider != null)
        {
            bgmSlider.value = bgmSource.volume;
            bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        }
        else
        {
            Debug.LogWarning("BGM Source o Slider non assegnato nell'Inspector.");
        }

        if (sfxSources != null && sfxSources.Length > 0 && sfxSlider != null)
        {
            sfxSlider.value = sfxSources[0].volume;
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }
        else
        {
            Debug.LogWarning("SFX Sources o Slider non assegnato correttamente nell'Inspector.");
        }
    }

    // Metodo per controllare il volume della musica
    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = volume;
    }

    // Metodo per controllare il volume degli effetti sonori
    public void SetSFXVolume(float volume)
    {
        foreach (AudioSource sfx in sfxSources)
        {
            sfx.volume = volume;
        }
    }

    // ----------------------------------------------------------
    // 1) Metodo per riprodurre il suono di "click" di un pulsante
    // ----------------------------------------------------------
    public void PlayButtonClick()
    {
        if (buttonClickClip == null)
        {
            Debug.LogWarning("AudioManager: nessun buttonClickClip assegnato!");
            return;
        }
        // Esempio: uso il primo sfxSources[0], ma puoi scegliere o randomizzare
        sfxSources[0].PlayOneShot(buttonClickClip);
    }

    public void StopBackgroundMusic()
    {
        if (bgmSource != null)
        {
            bgmSource.Stop();
        }
    }

    public void PlayGameOverSFX()
    {
        if (gameOverClip == null)
        {
            Debug.LogWarning("AudioManager: GameOverClip non assegnato!");
            return;
        }
        // Per comodità, lo facciamo riprodurre dal quarto sfxSource, 
        // ma puoi sceglierne un altro o un array in base alle tue necessità
        sfxSources[4].PlayOneShot(gameOverClip);
    }
}


