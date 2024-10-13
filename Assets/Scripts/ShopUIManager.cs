using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{
    public CharacterManager characterManager;
    public TMP_Text characterNameText;
    public Image characterImage;
    public TMP_Text characterCostText;
    public Button purchaseButton;
    public TMP_Text purchaseButtonText;
    public GameObject shopPanel;

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
        characterImage.sprite = character.characterSprite;
        characterCostText.text = character.isPurchased ? "Acquistato" : character.cost.ToString();

        purchaseButtonText.text = character.isPurchased ? "Seleziona" : "Acquista";
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
