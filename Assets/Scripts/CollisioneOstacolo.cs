using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisioneOstacolo : MonoBehaviour
{
    public GameObject thePlayer;
    public GameObject charModel;
    public AudioSource thudSound;
    public GameObject mainCam;
    public GameObject levelControl;
    static public bool lostGame = false;

    void OnTriggerEnter(Collider other)
    {
        this.gameObject.GetComponent<BoxCollider>().enabled = false;
        thePlayer.GetComponent<MovimentoGiocatore>().enabled = false;
        charModel.GetComponent<Animator>().Play("Stumble Backwards");
        lostGame = true; // Imposta lostGame a true
        Debug.Log("lostGame impostato a true."); // Log di debug
        levelControl.GetComponent<LevelDistance>().enabled = false;
        thudSound.Play();
        //Codice qui sotto da modificare se voglio fare il menù con la telecamera che si muove
        mainCam.GetComponent<Animator>().enabled = true;
        levelControl.GetComponent<EndRunSequence>().enabled = true;
    }

}