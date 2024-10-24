using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{
    public CharacterManager characterManager;
    public TMP_Text characterNameText;
    public TMP_Text characterCostText;
    public TMP_Text totalCoinsText; // Riferimento al testo delle monete totali
    public Button purchaseButton;
    public TMP_Text purchaseButtonText;
    public GameObject shopPanel;

    private int currentCharacterIndex = 0;

    void Start()
    {
        UpdateCharacterUI();
        UpdateTotalCoinsUI(); // Aggiorna il totale delle monete quando si avvia lo shop
    }

    public void NextCharacter()
    {
        currentCharacterIndex = (currentCharacterIndex + 1) % characterManager.characters.Count;
        UpdateCharacterUI();
    }

    public void PreviousCharacter()
    {
        currentCharacterIndex = (currentCharacterIndex - 1 + characterManager.characters.Count) % characterManager.characters.Count;
        UpdateCharacterUI();
    }

    private void UpdateCharacterUI()
    {
        var character = characterManager.characters[currentCharacterIndex];

        // Aggiorna i dettagli del personaggio
        characterNameText.text = character.characterName;
        characterCostText.text = character.isPurchased ? "Acquistato" : character.cost.ToString();
        purchaseButtonText.text = character.isPurchased ? "Seleziona" : "Acquista";

        // Attiva il modello del personaggio selezionato e disattiva gli altri
        for (int i = 0; i < characterManager.characters.Count; i++)
        {
            var characterModel = characterManager.characters[i].characterPrefab;
            characterModel.SetActive(i == currentCharacterIndex);
        }
    }

    public void OnPurchaseButtonClicked()
    {
        var character = characterManager.characters[currentCharacterIndex];
        if (character.isPurchased)
        {
            characterManager.SelectCharacter(currentCharacterIndex);
        }
        else
        {
            if (CoinManager.SpendCoins(character.cost))
            {
                characterManager.PurchaseCharacter(currentCharacterIndex);
                UpdateTotalCoinsUI(); // Aggiorna il totale delle monete dopo l'acquisto
            }
            else
            {
                Debug.Log("Monete insufficienti per acquistare il personaggio!");
            }
        }

        UpdateCharacterUI();
    }

    private void UpdateTotalCoinsUI()
    {
        // Aggiorna il testo delle monete totali
        totalCoinsText.text = "Monete: " + CoinManager.GetTotalCoins().ToString();
    }

    public void OnBackButtonClicked()
    {
        shopPanel.SetActive(false);
    }
}