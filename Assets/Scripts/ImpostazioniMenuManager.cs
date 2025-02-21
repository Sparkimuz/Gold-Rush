using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ImpostazioniMenuManager : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject profileStatisticLogoutPanel;
    public GameObject authPanel;
    public TMP_Text profile_UserName_text, profile_UserEmail_text;

    void Start()
    {

        FirebaseController.OnUserUpdated += UpdateProfileUI;

        // 🔥 Se Firebase è già pronto, aggiorna subito la UI
        if (FirebaseController.Instance != null && FirebaseController.Instance.user != null)
        {
            Debug.Log("🟢 Utente già caricato, aggiorno UI.");
            UpdateProfileUI();
        }
        else
        {
            Debug.Log("⏳ Aspetto che Firebase carichi l'utente...");
        }


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
        DestroyFirebaseController();
        SceneManager.LoadScene(2);
    }

    void OnDestroy()
    {
        // Evita memory leaks rimuovendo il listener quando l'oggetto viene distrutto
        FirebaseController.OnUserUpdated -= UpdateProfileUI;
    }


    void DestroyFirebaseController()
    {
        if (FirebaseController.Instance != null)
        {
            Debug.Log("Tentativo di distruggere FirebaseController, ma la distruzione è stata annullata.");
            // Commenta la linea seguente per evitare la distruzione
            // Destroy(FirebaseController.Instance.gameObject);
            // FirebaseController.Instance = null;
        }
    }

    void UpdateProfileUI()
    {
        Firebase.Auth.FirebaseUser user = FirebaseController.Instance.user;

        if (user != null)
        {
            Debug.Log("🔄 Aggiorno UI con Nome: " + user.DisplayName + " | Email: " + user.Email);
            profile_UserName_text.text = string.IsNullOrEmpty(user.DisplayName) ? "Nessun Nome" : user.DisplayName;
            profile_UserEmail_text.text = string.IsNullOrEmpty(user.Email) ? "Nessuna Email" : user.Email;
        }
        else
        {
            Debug.LogError("❌ ERRORE: Utente nullo in FirebaseController!");
            profile_UserName_text.text = "ERRORE";
            profile_UserEmail_text.text = "ERRORE";
        }
    }



}
