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
    public void PurchaseCharacter(int characterIndex)
    {
        if (!characters[characterIndex].isPurchased)
        {
            characters[characterIndex].isPurchased = true;
            Debug.Log(characters[characterIndex].characterName + " è stato acquistato!");
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