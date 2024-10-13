using System.Collections;
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
        yield return new WaitForSeconds(1); // Aspetta prima di mostrare la schermata finale
        liveCoins.SetActive(false);
        liveDis.SetActive(false);
        endScreen.SetActive(true);
        //yield return new WaitForSeconds(5);
        //fadeOut.SetActive(true);
        //SceneManager.LoadScene(0);
    }
}
