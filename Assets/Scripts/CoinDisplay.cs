using UnityEngine;
using TMPro; // Assicurati di avere TextMeshPro nel progetto

public class CoinDisplay : MonoBehaviour
{
    public TextMeshProUGUI coinText; // Riferimento all'elemento UI di TextMeshPro

    void Start()
    {
        UpdateCoinDisplay(); // Aggiorna l'UI all'avvio
    }

    // Metodo per aggiornare il testo con il totale delle monete
    public void UpdateCoinDisplay()
    {
        int totalCoins = CoinManager.GetTotalCoins();
        coinText.text = "" + totalCoins.ToString(); // Mostra il totale delle monete
    }

    // Metodo per chiamare l'aggiornamento dell'interfaccia utente da altri script
    public void RefreshUI()
    {
        UpdateCoinDisplay();
    }
}