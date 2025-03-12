using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelDistance : MonoBehaviour
{
    public GameObject disDisplay;       // Oggetto per visualizzare la distanza corrente
    public GameObject disDisplayEnd;    // Oggetto per visualizzare la distanza a fine livello
    public float disRun = 0f;           // Distanza percorsa dal giocatore (in "metri")
    public bool addingDis = false;      // Flag per evitare più coroutine in contemporanea
    public float disUpdateInterval = 0.1f; // Intervallo per l'aggiornamento della distanza

    private MovimentoGiocatore movimentoGiocatore; // Riferimento allo script di movimento

    // Metodo per salvare la distanza
    public void SaveDistance()
    {
        PlayerPrefs.SetFloat("SavedDistance", disRun);
        PlayerPrefs.Save();
    }

    // Metodo per caricare la distanza
    public void LoadDistance()
    {
        disRun = PlayerPrefs.GetFloat("SavedDistance", 0);
    }

    void Start()
    {
        if (disDisplayEnd == null)
        {
            Debug.LogError("❌ disDisplayEnd non è assegnato! Assicurati di collegarlo nell'Inspector.");
        }

        // Carica la distanza salvata
        LoadDistance();

        // Trova il riferimento allo script di movimento del giocatore
        movimentoGiocatore = FindObjectOfType<MovimentoGiocatore>();
    }

    void Update()
    {
        // Aggiorna la distanza solo se il giocatore può muoversi
        if (!addingDis && MovimentoGiocatore.canMove)
        {
            addingDis = true;
            StartCoroutine(AddingDis());
        }
    }

    IEnumerator AddingDis()
    {
        // Incrementa la distanza percorsa in base alla velocità corrente del giocatore
        if (movimentoGiocatore != null)
        {
            float distanceToAdd = movimentoGiocatore.playerSpeed * disUpdateInterval;
            disRun += distanceToAdd;

            // Aggiorna le interfacce utente per mostrare la nuova distanza
            disDisplay.GetComponent<TextMeshProUGUI>().text = Mathf.FloorToInt(disRun).ToString() + " m";
            disDisplayEnd.GetComponent<TextMeshProUGUI>().text = Mathf.FloorToInt(disRun).ToString() + " m";
        }

        // Attende l'intervallo specificato prima di aggiornare nuovamente
        yield return new WaitForSeconds(disUpdateInterval);
        addingDis = false;
    }
}