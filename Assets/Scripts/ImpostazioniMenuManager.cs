using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    public TMP_Text profile_UserName_text, profile_UserEmail_text, profile_distanceRecord_text;

    void Start()
    {


        // Assicurati che il FirebaseController esista
        /*if (FirebaseController.Instance == null)
        {
            SceneManager.LoadScene(2);
            return;
        }*/
        





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
        FirebaseController.Instance.logOut();
    }


    void OnDestroy()
    {
        // Evita memory leaks rimuovendo il listener quando l'oggetto viene distrutto
        FirebaseController.OnUserUpdated -= UpdateProfileUI;
    }


    /*void DestroyFirebaseController()
    {
        if (FirebaseController.Instance != null)
        {
            Debug.Log("🚪 Logout effettuato, distruggo FirebaseController.");
            Destroy(FirebaseController.Instance.gameObject);
            FirebaseController.Instance = null; // Rimuovi il riferimento statico
        }
        else
        {
            Debug.Log("⚠️ FirebaseController non trovato, impossibile distruggere.");
        }
    }*/


    private void Update()
    {
        if (FirebaseController.Instance == null)
        {
            Debug.LogWarning("⚠️ FirebaseController non trovato, provo a ricaricarlo...");
            SceneManager.LoadScene(2);
        }
    }



    async void UpdateProfileUI()
    {
        if (FirebaseController.Instance == null)
            return;

        Firebase.Auth.FirebaseUser user = FirebaseController.Instance.user;

        if (user != null)
        {
            Debug.Log("🔄 Aggiorno UI con Nome: " + user.DisplayName + " | Email: " + user.Email);
            profile_UserName_text.text = string.IsNullOrEmpty(user.DisplayName) ? "Nessun Nome" : user.DisplayName;
            profile_UserEmail_text.text = string.IsNullOrEmpty(user.Email) ? "Nessuna Email" : user.Email;

            // ⚠️ ATTENDI il valore prima di assegnarlo
            int distanceRecord = await GetDisRecord();
            profile_distanceRecord_text.text = distanceRecord.ToString() + " m";  // Aggiunto " m" per chiarezza
        }
        else
        {
            Debug.Log("❌ ERRORE: Utente nullo in FirebaseController!");
            SceneManager.LoadScene(2);
        }
    }




    private async Task<int> GetDisRecord()
    {
        TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();

        FirebaseController.Instance.LoadDisRecord((distanceRecord) =>
        {
            tcs.SetResult(distanceRecord);
        });

        return await tcs.Task;
    }



}
