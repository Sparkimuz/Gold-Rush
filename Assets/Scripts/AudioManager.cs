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

    void Start()
    {
        // Impostiamo i valori iniziali degli slider
        bgmSlider.value = bgmSource.volume;
        sfxSlider.value = sfxSources[0].volume;  // Prendiamo il volume del primo SFX per iniziare

        // Aggiungiamo i listener agli slider
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
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
}
