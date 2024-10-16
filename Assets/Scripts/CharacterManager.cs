using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Character
{
    public string characterName;
    public GameObject characterPrefab; // Prefab 3D del personaggio
    public int cost;
    public bool isPurchased;
}

public class CharacterManager : MonoBehaviour
{
    public List<Character> characters;
    public Transform characterDisplayPoint; // Punto in cui mostrare i modelli 3D
    private GameObject currentCharacterInstance;

    private int selectedCharacterIndex = 0;

    void Start()
    {
        DisplayCharacter(selectedCharacterIndex);
    }

    public void DisplayCharacter(int characterIndex)
    {
        // Rimuovi il personaggio attualmente visualizzato
        if (currentCharacterInstance != null)
        {
            Destroy(currentCharacterInstance);
        }

        // Crea una nuova istanza del personaggio selezionato
        Character character = characters[characterIndex];
        currentCharacterInstance = Instantiate(character.characterPrefab, characterDisplayPoint.position, characterDisplayPoint.rotation);
        currentCharacterInstance.transform.SetParent(characterDisplayPoint, false);
    }

    public void SelectCharacter(int characterIndex)
    {
        selectedCharacterIndex = characterIndex;
        // Aggiorna la visualizzazione del personaggio
        DisplayCharacter(selectedCharacterIndex);
    }

    public void PurchaseCharacter(int characterIndex)
    {
        if (!characters[characterIndex].isPurchased)
        {
            characters[characterIndex].isPurchased = true;
            Debug.Log(characters[characterIndex].characterName + " è stato acquistato!");
        }
    }
}