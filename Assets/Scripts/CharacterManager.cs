using UnityEngine;
using System.Collections.Generic;

public class CharacterManager : MonoBehaviour
{
    [System.Serializable]
    public class Character
    {
        public string characterName;
        public GameObject characterPrefab; // Il modello 3D del personaggio
        public int cost;
        public bool isPurchased;
    }

    public List<Character> characters = new List<Character>();
    private int selectedCharacterIndex = 0;

    // Metodo per acquistare un personaggio
    public void PurchaseCharacter(int index)
    {
        if (characters[index].isPurchased)
        {
            Debug.Log("ℹ️ Il personaggio " + index + " è già stato acquistato.");
            return;
        }

        int currentCoins = CoinManager.GetTotalCoins();

        if (characters[index].cost <= currentCoins)
        {
            Debug.Log("✅ Personaggio " + index + " acquistato con successo!");

            characters[index].isPurchased = true; // ⬅️ ASSICURA CHE VENGA SEGNATO COME ACQUISTATO
            CoinManager.SpendCoins(characters[index].cost);

            // Salva nei PlayerPrefs
            PlayerPrefs.SetInt("CharacterPurchased_" + index, 1);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogError("❌ ERRORE: Monete non sufficienti per acquistare il personaggio.");
        }
    }



    // Metodo per selezionare un personaggio
    public void SelectCharacter(int index)
    {
        if (characters[index].isPurchased)
        {
        selectedCharacterIndex = index;
        PlayerPrefs.SetInt("SelectedCharacter", index);
        PlayerPrefs.Save();
        }
    }


    // Metodo per ottenere il personaggio selezionato attualmente
    public GameObject GetSelectedCharacter()
    {
        return characters[selectedCharacterIndex].characterPrefab;
    }
}