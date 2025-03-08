using System;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    private const string TotalCoinsKey = "TotalCoins";

    // Ottieni il totale delle monete salvate
    public static void GetTotalCoins(Action<int> callback)
    {
        if (FirebaseController.Instance == null)
        {
            Debug.LogError("❌ FirebaseController non è pronto!");
            callback?.Invoke(0);
            return;
        }

        FirebaseController.Instance.LoadCoins(coins =>
        {
            Debug.Log("🎮 Monete caricate da Firebase: " + coins);
            callback?.Invoke(coins); // 🔥 Ora restituisce il valore quando è pronto
        });
    }






    // Aggiungi monete al totale e salva
    public static void AddCoins(int amount)
    {
        GetTotalCoins(currentCoins =>
        {
            int newTotal = currentCoins + amount;
            FirebaseController.Instance.SaveCoins(newTotal);
            Debug.Log("💰 Monete aggiunte! Nuovo totale: " + newTotal);
        });
    }

    // Riduci le monete quando si compra qualcosa
    public static void SpendCoins(int amount, Action<bool> callback)
    {
        GetTotalCoins(currentCoins =>
        {
            if (currentCoins >= amount)
            {
                int newTotal = currentCoins - amount;
                FirebaseController.Instance.SaveCoins(newTotal);
                Debug.Log("💸 Monete spese! Nuovo totale: " + newTotal);
                callback?.Invoke(true);
            }
            else
            {
                Debug.Log("❌ Monete insufficienti!");
                callback?.Invoke(false);
            }
        });
    }

}