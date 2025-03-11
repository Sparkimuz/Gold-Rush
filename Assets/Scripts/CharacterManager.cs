using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks; // Per supportare Task

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
        public static CharacterManager Instance;

    }

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public List<Character> characters = new List<Character>();
    private int selectedCharacterIndex = 0;




    // Metodo per acquistare un personaggio
    public async void PurchaseCharacter(int index)
    {
        if (characters[index].isPurchased)
        {
            Debug.Log("ℹ️ Il personaggio " + index + " è già stato acquistato.");
            return;
        }

        if (index == 0)
        {
            characters[index].isPurchased = true;
            PlayerPrefs.SetInt("CharacterPurchased_" + index, 1);
            PlayerPrefs.Save();
            return;
        }

        // Attende il valore delle monete da Firebase
        FirebaseController.Instance.LoadCoins(async (currentCoins) =>
        {
        
            if (characters[index].cost <= currentCoins)
            {
                Debug.Log("✅ Personaggio " + index + " acquistato con successo!");
                characters[index].isPurchased = true;

                int newCoinBalance = currentCoins - characters[index].cost;
                await FirebaseController.Instance.SaveCoins(newCoinBalance);

                PlayerPrefs.SetInt("CharacterPurchased_" + index, 1);
                PlayerPrefs.Save();
            }
            else
            {
                Debug.LogError("❌ ERRORE: Monete non sufficienti per acquistare il personaggio.");
            }
        });
    }

    // Metodo per selezionare un personaggio
    public async Task SelectCharacter(int index)
    {
        if (characters[index].isPurchased)
        {
            selectedCharacterIndex = index;

            // Salva il personaggio selezionato su Firebase invece di PlayerPrefs
            if (FirebaseController.Instance.user != null)
            {
                string userId = FirebaseController.Instance.user.UserId;
                await FirebaseController.Instance.dbReference
                    .Child("users").Child(userId).Child("selectedCharacter")
                    .SetValueAsync(index);
            }

            PlayerPrefs.SetInt("SelectedCharacter", index); // Per backup locale
            PlayerPrefs.Save();
        }
    }

    // Metodo asincrono per caricare il personaggio selezionato da Firebase
    public async Task LoadSelectedCharacter()
    {
        if (FirebaseController.Instance.user != null)
        {
            string userId = FirebaseController.Instance.user.UserId;
            var characterTask = FirebaseController.Instance.dbReference
                .Child("users").Child(userId).Child("selectedCharacter").GetValueAsync();

            await characterTask;

            if (characterTask.Result.Exists)
            {
                selectedCharacterIndex = int.Parse(characterTask.Result.Value.ToString());
                PlayerPrefs.SetInt("SelectedCharacter", selectedCharacterIndex); // Backup locale
                PlayerPrefs.Save();
            }
            else
            {
                Debug.Log("⚠️ Nessun personaggio selezionato trovato, uso quello di default.");
            }
        }
    }

    // Metodo corretto per restituire il personaggio selezionato in gioco
    public GameObject GetSelectedCharacter()
    {
        //return characters[selectedCharacterIndex].gamePrefab;  forse è piu corretta, da verificare
        return characters[selectedCharacterIndex].shopPrefab; // Usa il modello corretto per il gameplay

    }

}
