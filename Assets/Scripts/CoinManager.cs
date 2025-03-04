using UnityEngine;

public class CoinManager : MonoBehaviour
{
    private const string TotalCoinsKey = "TotalCoins";

    // Ottieni il totale delle monete salvate
    public static int GetTotalCoins()
    {
        return PlayerPrefs.GetInt(TotalCoinsKey, 0);
    }

    // Aggiungi monete al totale e salva
    public static void AddCoins(int amount)
    {
        int currentCoins = GetTotalCoins();
        currentCoins += amount;
        PlayerPrefs.SetInt(TotalCoinsKey, currentCoins);
        PlayerPrefs.Save();
    }

    // Riduci le monete quando si compra qualcosa
    public static bool SpendCoins(int amount)
    {
        int currentCoins = GetTotalCoins();
        Debug.Log("Tentativo di acquisto - Monete attuali: " + currentCoins + " - Costo: " + amount);

        if (currentCoins >= amount)
        {
            currentCoins -= amount;
            PlayerPrefs.SetInt(TotalCoinsKey, currentCoins);
            PlayerPrefs.Save();
            Debug.Log("Acquisto riuscito! Monete rimanenti: " + currentCoins);
            return true;
        }
        else
        {
            Debug.Log("Acquisto fallito: monete insufficienti!");
            return false;
        }
    }

}