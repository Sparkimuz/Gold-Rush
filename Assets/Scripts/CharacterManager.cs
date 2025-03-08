using UnityEngine;
using System.Collections.Generic;

public class CharacterManager : MonoBehaviour
{
    [System.Serializable]
    public class Character
    {
        public string characterName;
        public GameObject shopPrefab; // Il modello 3D del personaggio
        public GameObject gamePrefab;  // Modello/animator per il gioco (FastRun, Jump, ecc.)
        public int cost;
        public bool isPurchased;
    }
        void Awake()
    {
        // Impedisce che questo GameObject venga distrutto
        // quando carichi una nuova scena
        DontDestroyOnLoad(this.gameObject);

        // Se vuoi essere sicuro di avere solo un CharacterManager,
        // puoi aggiungere un controllo anti-duplicato
        // e distruggere eventuali duplicati:
        /*
        var existingManagers = FindObjectsOfType<CharacterManager>();
        if (existingManagers.Length > 1)
        {
            Destroy(gameObject);
        }
        */
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
        return characters[selectedCharacterIndex].shopPrefab;
    }
}