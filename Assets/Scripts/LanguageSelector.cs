using UnityEngine;
using UnityEngine.Localization.Settings;

public class LanguageSelector : MonoBehaviour
{
    // Metodo per cambiare la lingua in base all'indice passato
    public void ChangeLanguage(int languageIndex)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[languageIndex];
        Debug.Log("Lingua cambiata a: " + LocalizationSettings.SelectedLocale.LocaleName);
    }
}