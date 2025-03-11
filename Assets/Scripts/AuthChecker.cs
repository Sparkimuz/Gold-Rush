using UnityEngine;
using Firebase.Auth;
using UnityEngine.SceneManagement;

public class AuthChecker : MonoBehaviour
{
    public static AuthChecker Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        CheckAuthentication();
    }

    public void CheckAuthentication()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        if (auth.CurrentUser == null)
        {
            Debug.Log("⛔ Nessun utente loggato, reindirizzamento alla scena di login...");
            SceneManager.LoadScene("LoginScene");
        }
    }
}
