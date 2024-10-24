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
        if (characters[index].cost <= CoinManager.GetTotalCoins()) // Supponiamo esista un metodo GetCurrency()
        {
            characters[index].isPurchased = true;
            CoinManager.SpendCoins(characters[index].cost);
        }
    }

    // Metodo per selezionare un personaggio
    public void SelectCharacter(int index)
    {
        selectedCharacterIndex = index;
        // Assicurati che il personaggio selezionato sia visualizzato nella scena o nel gioco
    }

    // Metodo per ottenere il personaggio selezionato attualmente
    public GameObject GetSelectedCharacter()
    {
        return characters[selectedCharacterIndex].characterPrefab;
    }
}