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
}
