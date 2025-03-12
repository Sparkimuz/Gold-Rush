/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndRunSequence : MonoBehaviour
{

    public GameObject liveCoins;
    public GameObject liveDis;
    public GameObject endScreen;
    public GameObject fadeOut;

    void Update()
    {
        if (CollisioneOstacolo.lostGame)
        {
            StartCoroutine(EndSequence());
            CollisioneOstacolo.lostGame = false; // Resettiamo per non avviare la coroutine più volte
        }
    }

    IEnumerator EndSequence()
    {
        // Aggiungi le monete raccolte durante la run al totale
        if (MasterInfo.coinCount > 0)
        {
            CoinManager.AddCoins(MasterInfo.coinCount);
        }

        // Reset delle monete raccolte per la prossima run

        yield return new WaitForSeconds(1); // Aspetta prima di mostrare la schermata finale
        liveCoins.SetActive(false);
        liveDis.SetActive(false);
        endScreen.SetActive(true);
        yield return new WaitForSeconds(5);
        //fadeOut.SetActive(true);
        SceneManager.LoadScene(0);
    }
}
*/

using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndRunSequence : MonoBehaviour
{
    public GameObject liveCoins;
    public GameObject liveDis;
    public GameObject endScreen;
    public GameObject recordText;

    // Altre variabili eventualmente necessarie
    public float endScreenDelay = 1f; // Ritardo prima di mostrare la schermata finale
    public float returnToMenuDelay = 5f; // Ritardo prima di tornare al menu principale

    private bool lostGame = false; // Variabile per controllare lo stato del gioco

    public LevelDistance levelDistance;

    void Start()
    {
        if (levelDistance == null)
        {
            levelDistance = FindObjectOfType<LevelDistance>();
            if (levelDistance == null)
            {
                Debug.LogError("❌ Nessun oggetto LevelDistance trovato nella scena!");
            }
        }

        if (FirebaseController.Instance == null)
        {
            Debug.LogError("❌ FirebaseController non è inizializzato! Verifica che sia presente nella scena.");
            return;
        }

    }


    private void Update()
    {
        if (CollisioneOstacolo.lostGame && !lostGame) // Esegui solo se il gioco è perso e non è già stato processato
        {
            lostGame = true; // Impedisce che la sequenza venga ripetuta
            StartCoroutine(HandleGameEnd());
            
        }
    }

    private IEnumerator HandleGameEnd()
    {
        if (levelDistance == null)
        {
            Debug.LogError("❌ levelDistance è NULL. Assicurati di assegnarlo correttamente nell'Inspector.");
            yield break;
        }

        if (levelDistance.disDisplayEnd == null)
        {
            Debug.LogError("❌ disDisplayEnd è NULL. Assicurati che sia assegnato correttamente.");
            yield break;
        }

        string distanceText = levelDistance.disDisplayEnd.GetComponent<TextMeshProUGUI>().text;
        if (string.IsNullOrWhiteSpace(distanceText))
        {
            Debug.LogError("❌ Il testo di disDisplayEnd è vuoto. Verifica che venga aggiornato correttamente.");
            yield break;
        }

        string cleanedText = CleanNumber(distanceText);
        if (!int.TryParse(cleanedText, out int endDis))
        {
            Debug.LogError($"❌ Errore nella conversione della distanza: '{distanceText}' → '{cleanedText}'. Verifica il formato.");
            yield break;
        }

        if (FirebaseController.Instance == null)
        {
            Debug.LogError("❌ FirebaseController.Instance è NULL. Assicurati che sia presente nella scena.");
            yield break;
        }

        // 🔄 Avvia l'aggiornamento della distanza in Firebase come operazione asincrona
        yield return StartCoroutine(UpdateDistanceRecord(endDis));

        StartCoroutine(EndSequence());
    }

    private IEnumerator UpdateDistanceRecord(int endDis)
    {
        Task<int> getRecordTask = GetDisRecord();
        yield return new WaitUntil(() => getRecordTask.IsCompleted);

        if (getRecordTask.Exception != null)
        {
            Debug.LogError($"❌ Errore nel recupero del record: {getRecordTask.Exception}");
            yield break;
        }

        int recordDistance = getRecordTask.Result;
        if (endDis > recordDistance)
        {
            Task saveTask = FirebaseController.Instance.SaveDisRecord(endDis);
            yield return new WaitUntil(() => saveTask.IsCompleted);

            if (saveTask.Exception != null)
            {
                Debug.LogError($"❌ Errore nel salvataggio del record: {saveTask.Exception}");
            }

            recordText.SetActive(true);
        }
    }


    string CleanNumber(string input)
    {
        return Regex.Match(input, @"\d+").Value;
    }



    IEnumerator EndSequence()
    {
        // Aggiungi le monete raccolte durante la run al totale
        if (MasterInfo.coinCount > 0)
        {
            CoinManager.AddCoins(MasterInfo.coinCount);
        }

        // Reset delle monete raccolte per la prossima run
        //MasterInfo.coinCount = 0;

        yield return new WaitForSeconds(endScreenDelay); // Aspetta prima di mostrare la schermata finale

        // Disattiva elementi della UI durante la run e attiva la schermata finale
        liveCoins.SetActive(false);
        liveDis.SetActive(false);
        endScreen.SetActive(true);

        //yield return new WaitForSeconds(returnToMenuDelay); // Aspetta prima di tornare al menu principale

        // Torna alla schermata del menu principale (assumendo che sia nella scena con indice 0)
        //SceneManager.LoadScene(0);
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