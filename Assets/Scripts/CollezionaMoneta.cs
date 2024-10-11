using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollezionaMoneta : MonoBehaviour
{
    [SerializeField] AudioSource coinFX;

    void OnTriggerEnter(Collider other)
    {
        coinFX.Play();
        MasterInfo.coinCount +=1;
        this.gameObject.SetActive(false);
    }

}