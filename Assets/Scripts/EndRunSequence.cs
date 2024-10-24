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

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndRunSequence : MonoBehaviour
{
    public GameObject liveCoins;
    public GameObject liveDis;
    public GameObject endScreen;
    // Altre variabili eventualmente necessarie
    public float endScreenDelay = 1f; // Ritardo prima di mostrare la schermata finale
    public float returnToMenuDelay = 5f; // Ritardo prima di tornare al menu principale

    private bool lostGame = false; // Variabile per controllare lo stato del gioco

    private void Update()
    {
        if (CollisioneOstacolo.lostGame) // Controlla se la variabile è impostata su true
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
}