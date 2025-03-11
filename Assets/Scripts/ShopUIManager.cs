using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks; // Per Task

public class ShopUIManager : MonoBehaviour
{
    public TMP_Text warningMessageText;
    public CharacterManager characterManager;
    public TMP_Text characterNameText;
    public TMP_Text characterCostText;
    public TMP_Text totalCoinsText;
    public Button purchaseButton;
    public TMP_Text purchaseButtonText;
    public GameObject shopPanel;

    private int currentCharacterIndex = 0;

    async void Start()
    {
        currentCharacterIndex = await GetSelectedCharacterIndex(); // Inizializza con il personaggio dell'utente
        await UpdateTotalCoinsUI();
        await UpdateCharacterUI();

        if (!characterManager.characters[0].isPurchased)
        {
            await SaveCharacterPurchase(0);
            await characterManager.SelectCharacter(0);
            await UpdateCharacterUI();
        }
        
    }


    public void NextCharacter()
    {
        currentCharacterIndex = (currentCharacterIndex + 1) % characterManager.characters.Count;
        _ = UpdateCharacterUI(); // Ignoriamo il Task per evitare errori
    }

    public void PreviousCharacter()
    {
        currentCharacterIndex = (currentCharacterIndex - 1 + characterManager.characters.Count) % characterManager.characters.Count;
        _ = UpdateCharacterUI();
    }

    private async Task UpdateCharacterUI()
    {
        var character = characterManager.characters[currentCharacterIndex];

        character.isPurchased = await IsCharacterPurchased(currentCharacterIndex);
        int selectedCharacterIndex = await GetSelectedCharacterIndex();

        characterNameText.text = character.characterName;
        characterCostText.text = character.isPurchased ? "Comprato" : character.cost.ToString();

        AttivaPersonaggio();

        purchaseButtonText.text = character.isPurchased
            ? (currentCharacterIndex == selectedCharacterIndex ? "Selezionato" : "Seleziona")
            : "Compra";

        Debug.Log($"UI aggiornata per personaggio {currentCharacterIndex} - Acquistato: {character.isPurchased}");
    }

    private void AttivaPersonaggio()
    {
        for (int i = 0; i < characterManager.characters.Count; i++)
        {
            var c = characterManager.characters[i];
            if (c.shopPrefab != null)
            {
                c.shopPrefab.SetActive(i == currentCharacterIndex);
            }
        }
    }

    public async void OnPurchaseButtonClicked()
    {
        var character = characterManager.characters[currentCharacterIndex];

        Debug.Log($"🛒 Bottone Acquista/Seleziona premuto per personaggio: {currentCharacterIndex}");

        if (character.isPurchased)
        {
            await characterManager.SelectCharacter(currentCharacterIndex);
            await UpdateTotalCoinsUI();
            await UpdateCharacterUI();
        }
        else
        {
            int currentCoins = await GetTotalCoins();
            if (character.cost <= currentCoins)
            {
                Debug.Log($"💰 Acquisto effettuato per personaggio: {currentCharacterIndex}");

                character.isPurchased = true;
                await SaveCharacterPurchase(currentCharacterIndex);
                await FirebaseController.Instance.SaveCoins(currentCoins - character.cost);

                await characterManager.SelectCharacter(currentCharacterIndex);
                await UpdateTotalCoinsUI();
                await UpdateCharacterUI();
            }
            else
            {
                ShowWarningMessage("Monete insufficienti!");
            }
        }
    }

    private async Task UpdateTotalCoinsUI()
    {
        int currentCoins = await GetTotalCoins();
        totalCoinsText.text = currentCoins.ToString();
    }

    private async Task<int> GetTotalCoins()
    {
        TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();

        FirebaseController.Instance.LoadCoins((currentCoins) =>
        {
            tcs.SetResult(currentCoins);
        });

        return await tcs.Task;
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
        yield return new WaitForSeconds(2);
        warningMessageText.gameObject.SetActive(false);
    }

    private async Task SaveCharacterPurchase(int characterIndex)
    {
        if (FirebaseController.Instance.user != null)
        {
            string userId = FirebaseController.Instance.user.UserId;
            await FirebaseController.Instance.dbReference
                .Child("users").Child(userId).Child("purchasedCharacters")
                .Child(characterIndex.ToString()).SetValueAsync(true);
        }
    }

    private async Task<bool> IsCharacterPurchased(int characterIndex)
    {
        if (FirebaseController.Instance.user != null)
        {
            string userId = FirebaseController.Instance.user.UserId;
            var task = await FirebaseController.Instance.dbReference
                .Child("users").Child(userId).Child("purchasedCharacters")
                .Child(characterIndex.ToString()).GetValueAsync();

            return task.Exists && bool.Parse(task.Value.ToString());
        }
        return false;
    }

    private async Task<int> GetSelectedCharacterIndex()
    {
        if (FirebaseController.Instance.user != null)
        {
            string userId = FirebaseController.Instance.user.UserId;
            var task = await FirebaseController.Instance.dbReference
                .Child("users").Child(userId).Child("selectedCharacter").GetValueAsync();

            return task.Exists ? int.Parse(task.Value.ToString()) : 0;
        }
        return 0;
    }
}
