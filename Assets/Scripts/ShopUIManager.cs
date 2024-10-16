using UnityEngine;
using TMPro;
using UnityEngine.UI; // Assicurati di avere TMP per il testo

public class ShopUIManager : MonoBehaviour
{
    public CharacterManager characterManager;
    public TMP_Text characterNameText;
    public TMP_Text characterCostText;
    public Button purchaseButton;
    public TMP_Text purchaseButtonText;
    public GameObject shopPanel;

    public Transform characterDisplayPoint; // Punto dove visualizzare i modelli 3D
    private GameObject currentCharacterModel; // Riferimento al modello attualmente mostrato

    private int currentCharacterIndex = 0;

    void Start()
    {
        UpdateCharacterUI();
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

        // Rimuovi il modello attuale, se esistente
        if (currentCharacterModel != null)
        {
            Destroy(currentCharacterModel);
        }

        // Carica il modello 3D del personaggio corrente
        currentCharacterModel = Instantiate(character.characterPrefab, characterDisplayPoint.position, Quaternion.identity);
        currentCharacterModel.transform.SetParent(characterDisplayPoint);
    }

    public void OnPurchaseButtonClicked()
    {
        if (characterManager.characters[currentCharacterIndex].isPurchased)
        {
            characterManager.SelectCharacter(currentCharacterIndex);
        }
        else
        {
            characterManager.PurchaseCharacter(currentCharacterIndex);
        }

        UpdateCharacterUI();
    }

    public void OnBackButtonClicked()
    {
        shopPanel.SetActive(false);
        // Logica per tornare al menu principale
    }
}
