using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{
    public CharacterManager characterManager;
    public TMP_Text characterNameText;
    public TMP_Text characterCostText;
    public Button purchaseButton;
    public TMP_Text purchaseButtonText;
    public GameObject shopPanel;

    public Transform characterDisplayPoint;
    private GameObject currentCharacterModel;
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
        characterNameText.text = character.characterName;
        characterCostText.text = character.isPurchased ? "Acquistato" : character.cost.ToString();
        purchaseButtonText.text = character.isPurchased ? "Seleziona" : "Acquista";

        if (currentCharacterModel != null)
        {
            Destroy(currentCharacterModel);
        }

        currentCharacterModel = Instantiate(character.characterPrefab, characterDisplayPoint.position, Quaternion.identity);
        currentCharacterModel.transform.SetParent(characterDisplayPoint);
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