using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStarter : MonoBehaviour
{
    public GameObject countDown3;
    public GameObject countDown2;
    public GameObject countDown1;
    public GameObject countDownGo;
    public AudioSource readyFX;
    public AudioSource goFX;
    // Start is called before the first frame update
    void Start()
    {
        // Assicurati che il FirebaseController esista
        if (FirebaseController.Instance == null)
        {
            GameObject firebaseObj = GameObject.Find("FirebaseController");
            if (firebaseObj == null)
            {
                firebaseObj = new GameObject("FirebaseController");
                firebaseObj.AddComponent<FirebaseController>();
            }
        }

        MasterInfo.coinCount = 0;
        MovimentoGiocatore.canMove = true;
        StartCoroutine(CountSequence());
    }

    IEnumerator CountSequence(){
        //yield return new WaitForSeconds(1.5f);
        countDown3.SetActive(true);
        readyFX.Play();
        yield return new WaitForSeconds(1);
        countDown2.SetActive(true);
        readyFX.Play();
        yield return new WaitForSeconds(1);
        countDown1.SetActive(true);
        readyFX.Play();
        yield return new WaitForSeconds(1);
        countDownGo.SetActive(true);
        goFX.Play();
        MovimentoGiocatore.canMove = true;
    }
}
