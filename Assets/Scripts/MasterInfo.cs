using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterInfo : MonoBehaviour
{
    public static int coinCount = 0;
    [SerializeField] GameObject coinDisplay;
    [SerializeField] GameObject coinDisplayEnd;

    // Update is called once per frame
    void Update()
    {
        coinDisplay.GetComponent<TMPro.TMP_Text>().text= "" + coinCount;
        coinDisplayEnd.GetComponent<TMPro.TMP_Text>().text= "" + coinCount;
    }
}
