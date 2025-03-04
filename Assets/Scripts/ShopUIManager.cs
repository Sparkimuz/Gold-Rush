using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class ShopUIManager : MonoBehaviour
{
    public TMP_Text warningMessageText; // Riferimento al messaggio di avviso
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

        // Ricarica dai PlayerPrefs
        character.isPurchased = PlayerPrefs.GetInt("CharacterPurchased_" + currentCharacterIndex, 0) == 1;
        int selectedCharacterIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);

        // Aggiorna il nome e il prezzo/acquisto
        characterNameText.text = character.characterName;
        characterCostText.text = character.isPurchased ? "Comprato" : character.cost.ToString();

        //Attiva Personaggi nel negozio
        AttivaPersonaggio();


        // Mostra "Selezionato" solo per il personaggio attualmente scelto
        if (character.isPurchased)
        {
            purchaseButtonText.text = (currentCharacterIndex == selectedCharacterIndex) ? "Selezionato" : "Seleziona";
        }
        else
        {
            purchaseButtonText.text = "Compra";
        }



        Debug.Log("UI aggiornata per personaggio " + currentCharacterIndex + " - Acquistato: " + character.isPurchased);
    }

    private void AttivaPersonaggio()
    {
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

        Debug.Log("🛒 Bottone Acquista/Seleziona premuto per personaggio: " + currentCharacterIndex);

        if (character.isPurchased)
        {
            characterManager.SelectCharacter(currentCharacterIndex);
            PlayerPrefs.SetInt("SelectedCharacter", currentCharacterIndex);
            PlayerPrefs.Save();
            UpdateCharacterUI();
        }
        else
        {
            if (CoinManager.SpendCoins(character.cost))
            {
                Debug.Log("💰 Acquisto effettuato per personaggio: " + currentCharacterIndex);
                characterManager.PurchaseCharacter(currentCharacterIndex);

                // Controlliamo se il personaggio è stato davvero acquistato
                if (characterManager.characters[currentCharacterIndex].isPurchased)
                {
                    Debug.Log("✅ Personaggio acquistato e aggiornato correttamente.");
                    characterManager.SelectCharacter(currentCharacterIndex);
                    PlayerPrefs.SetInt("SelectedCharacter", currentCharacterIndex);
                    PlayerPrefs.Save();
                }
                else
                {
                    Debug.LogError("❌ ERRORE: Il personaggio non è stato aggiornato correttamente.");
                }

                UpdateTotalCoinsUI();
                UpdateCharacterUI();
            }
            else
            {
                ShowWarningMessage("Monete insufficienti!");
            }
        }
    }




    private void UpdateTotalCoinsUI()
    {
        // Aggiorna il testo delle monete totali
        totalCoinsText.text = "" + CoinManager.GetTotalCoins().ToString();
    }

    public void OnBackButtonClicked()
    {
        shopPanel.SetActive(false);
    }

    void ShowWarningMessage(string message)
    {
        if (warningMessageText != null)
        {
            warningMessageText.text = message;
            warningMessageText.gameObject.SetActive(true);
            StartCoroutine(HideWarningMessage());
        }
    }

    IEnumerator HideWarningMessage()
    {
        yield return new WaitForSeconds(2); // Il messaggio resta visibile per 2 secondi
        warningMessageText.gameObject.SetActive(false);
    }

}