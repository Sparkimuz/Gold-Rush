using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelDistance : MonoBehaviour
{
    public GameObject disDisplay;
    public GameObject disDisplayEnd;
    public int disRun;
    public bool addingDis = false;
    public float disDelay = 0.35f;

    // Metodo per salvare la distanza
    public void SaveDistance()
    {
        PlayerPrefs.SetInt("SavedDistance", disRun);
        PlayerPrefs.Save();
    }

    // Metodo per caricare la distanza
    public void LoadDistance()
    {
        disRun = PlayerPrefs.GetInt("SavedDistance", 0);
    }

    void Start()
    {
        LoadDistance();
    }

    void Update()
    {
        if (!addingDis)
        {
            addingDis = true;
            StartCoroutine(AddingDis());
        }
    }

    IEnumerator AddingDis()
    {
        disRun += 1;
        disDisplay.GetComponent<TextMeshProUGUI>().text = disRun.ToString();
        disDisplayEnd.GetComponent<TextMeshProUGUI>().text = disRun.ToString();
        yield return new WaitForSeconds(disDelay);
        addingDis = false;
    }
}
