using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuFunction : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayGame(){
        SceneManager.LoadScene(1);

    }

    public void Restart()
    {
        // Ricarica la scena attuale
        SceneManager.LoadScene(1/*SceneManager.GetActiveScene().buildIndex*/);
    }

    // Questa funzione verrà chiamata quando il pulsante di menu viene cliccato
    public void GoToMenu()
    {
        // Ricarica la scena del menu principale (assicurati che sia nella posizione corretta nell'index)
        SceneManager.LoadScene(0); // Sostituisci 0 con l'index della tua scena principale se necessario
    }
}
