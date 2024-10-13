using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [System.Serializable]
    public class Character
    {
        public string characterName;
        public int cost;
        public Sprite characterSprite;
        public bool isPurchased;
    }

    public List<Character> characters = new List<Character>(); // Lista di personaggi disponibili
    public int playerCoins = 10000; // Monete del giocatore (puoi caricarle da un sistema di salvataggio)

    private int selectedCharacterIndex = 0; // Indice del personaggio selezionato

    void Start()
    {
        LoadPlayerData();
    }

    public void SelectCharacter(int index)
    {
        if (characters[index].isPurchased)
        {
            selectedCharacterIndex = index;
            Debug.Log("Personaggio selezionato: " + characters[index].characterName);
            SavePlayerData();
        }
        else
        {
            Debug.Log("Personaggio non acquistato. Acquistalo prima di selezionarlo.");
        }
    }

    public void PurchaseCharacter(int index)
    {
        if (!characters[index].isPurchased && playerCoins >= characters[index].cost)
        {
            playerCoins -= characters[index].cost;
            characters[index].isPurchased = true;
            Debug.Log("Personaggio acquistato: " + characters[index].characterName);
            SavePlayerData();
        }
        else if (characters[index].isPurchased)
        {
            Debug.Log("Personaggio già acquistato.");
        }
        else
        {
            Debug.Log("Monete insufficienti.");
        }
    }

    private void LoadPlayerData()
    {
        // Aggiungi il caricamento dei dati del giocatore (ad esempio PlayerPrefs)
        // playerCoins = PlayerPrefs.GetInt("PlayerCoins", 10000);
        // selectedCharacterIndex = PlayerPrefs.GetInt("SelectedCharacterIndex", 0);
    }

    private void SavePlayerData()
    {
        // Aggiungi il salvataggio dei dati del giocatore (ad esempio PlayerPrefs)
        // PlayerPrefs.SetInt("PlayerCoins", playerCoins);
        // PlayerPrefs.SetInt("SelectedCharacterIndex", selectedCharacterIndex);
    }

    public Character GetSelectedCharacter()
    {
        return characters[selectedCharacterIndex];
    }
}
