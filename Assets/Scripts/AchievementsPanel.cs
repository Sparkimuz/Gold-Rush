using UnityEngine;
using TMPro;
using System.Collections;

public class AchievementsPanel : MonoBehaviour
{
    [Header("Distance UI")]
    public TMP_Text distance5000Text;
    public TMP_Text distance10000Text;
    public TMP_Text distance50000Text;
    public TMP_Text distanceTotalText; // "TOT: 6781"

    [Header("Coins UI")]
    public TMP_Text coins500Text;
    public TMP_Text coins1000Text;
    public TMP_Text coins5000Text;
    public TMP_Text coinsTotalText; // "TOT: 137"

    // Soglie di esempio (modificabili)
    private int distanceThreshold1 = 5000;
    private int distanceThreshold2 = 10000;
    private int distanceThreshold3 = 50000;

    private int coinThreshold1 = 500;
    private int coinThreshold2 = 1000;
    private int coinThreshold3 = 5000;

    // Colori che useremo per "non sbloccato" e "sbloccato"
    public Color notAchievedColor = Color.white;  // O giallo
    public Color achievedColor = Color.green;     // O un altro colore

    // Quando il pannello si attiva, ricarichiamo i dati
    private void OnEnable()
    {
        StartCoroutine(RefreshAchievements());
    }

    IEnumerator RefreshAchievements()
    {
        // 1) Carica la distanza totale
        int totalDistance = -1;
        FirebaseController.Instance.LoadDistanceTotal((dist) => {
            totalDistance = dist;
        });
        yield return new WaitUntil(() => totalDistance >= 0);

        // 2) Carica le monete totali (all-time)
        int totalCoins = -1;
        FirebaseController.Instance.LoadCoinsAllTime((coins) => {
            totalCoins = coins;
        });
        yield return new WaitUntil(() => totalCoins >= 0);

        // Aggiorna TOT
        distanceTotalText.text = "TOT: " + totalDistance;
        coinsTotalText.text    = "TOT: " + totalCoins;

        // Aggiorna i testi delle soglie DISTANZA
        UpdateThresholdText(distance5000Text, totalDistance, distanceThreshold1);
        UpdateThresholdText(distance10000Text, totalDistance, distanceThreshold2);
        UpdateThresholdText(distance50000Text, totalDistance, distanceThreshold3);

        // Aggiorna i testi delle soglie MONETE
        UpdateThresholdText(coins500Text,   totalCoins, coinThreshold1);
        UpdateThresholdText(coins1000Text,  totalCoins, coinThreshold2);
        UpdateThresholdText(coins5000Text,  totalCoins, coinThreshold3);
    }

    // Metodo di utilità per mostrare "X/soglia"
    // e cambiare colore se raggiunto
    private void UpdateThresholdText(TMP_Text textField, int currentValue, int threshold)
    {
        // Se currentValue supera la soglia, mostriamo la soglia piena
        // Esempio: 6781/5000 => "5000/5000" (raggiunto)
        int displayedValue = Mathf.Min(currentValue, threshold);

        textField.text = displayedValue + "/" + threshold;

        // Se total >= soglia, metti in "achievedColor", altrimenti "notAchievedColor"
        if (currentValue >= threshold)
            textField.color = achievedColor;
        else
            textField.color = notAchievedColor;
    }
}
