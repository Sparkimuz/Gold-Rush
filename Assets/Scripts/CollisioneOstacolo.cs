using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisioneOstacolo : MonoBehaviour
{
    public GameObject thePlayer;
    static public GameObject charModel;
    public AudioSource thudSound;
    public GameObject mainCam;
    public GameObject levelControl;
    static public bool lostGame = false;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"CollisioneOstacolo.OnTriggerEnter(): Ostacolo = {this.name}, Collider = {other.name}");

        // Per verificare se c'è un parent
        if (other.transform.parent != null)
        {
            Debug.Log($"Il parent di '{other.name}' è '{other.transform.parent.name}'");
        }

        var mg = other.GetComponentInParent<MovimentoGiocatore>();
        if (mg != null)
        {
            Debug.Log("Trovato MovimentoGiocatore su " + mg.gameObject.name);
        }

        if (MovimentoGiocatore.canMove)
        {
            this.gameObject.GetComponent<BoxCollider>().enabled = false;
            thePlayer.GetComponent<MovimentoGiocatore>().enabled = false;

            Animator animator = charModel.GetComponent<Animator>();
        
            if(animator != null)
            {
                animator.applyRootMotion = true; // 🔴 Riattiva temporaneamente il root motion
                animator.Play("Stumble Backwards");
                Debug.Log("CollisioneOstacolo: attivato ApplyRootMotion e riprodotto 'Stumble Backwards'");
            }
            Debug.Log("CollisioneOstacolo: Forzo animazione 'Stumble Backwards'");
            MovimentoGiocatore.PlayModelAnimation("Stumble Backwards");
            CollisioneOstacolo.lostGame = true; 
            Debug.Log("CollisioneOstacolo.OnTriggerEnter(): lostGame = true");
            levelControl.GetComponent<LevelDistance>().enabled = false;
            thudSound.Play();
            levelControl.GetComponent<EndRunSequence>().enabled = true;

            StartCoroutine(DisableRootMotion(animator, 3f));
        }
    }

    IEnumerator DisableRootMotion(Animator animator, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        animator.applyRootMotion = false;  // ✅ Ritorna al controllo manuale dopo la caduta
        Debug.Log("CollisioneOstacolo: ApplyRootMotion disattivato.");
    }

}