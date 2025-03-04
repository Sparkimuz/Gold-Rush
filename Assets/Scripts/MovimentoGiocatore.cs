using System.Collections;
using UnityEngine;

public class MovimentoGiocatore : MonoBehaviour
{
    private GameObject currentCharacter;  // Il modello attuale del personaggio
    public float initialSpeed = 2; // Velocità iniziale del giocatore
    public float growthRate = 0.5f; // Tasso di crescita logaritmica
    public float horizontalSpeed = 3;
    public float rightLimit = 3.2f;
    public float leftLimit = -3.2f;
    public static bool canMove = false;
    public bool isJumping = false;
    public float jumpHeight = 3; // Altezza del salto
    public float timeInAir = 1; // Tempo totale in aria
    private Animator animator;
    public GameObject playerObject;

    private float originalY; // Posizione Y iniziale del personaggio
    private float epsilon = 0.01f; // Tolleranza per piccoli errori di calcolo
    private float distanceCovered = 0; // Distanza percorsa dal giocatore

    public float playerSpeed; // Velocità attuale del giocatore

    // Variabili per il rilevamento dello swipe
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private float swipeThreshold = 50f; // Distanza minima dello swipe per essere considerato valido

    void Start()
    {
        animator = GetComponent<Animator>(); //Assicura che il personaggio abbia un animator
        originalY = transform.position.y;

        int selectedCharacterIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        GameObject selectedCharacterPrefab = FindObjectOfType<CharacterManager>().characters[selectedCharacterIndex].characterPrefab;

        if (selectedCharacterPrefab != null)
        {
            Debug.Log("✅ Personaggio selezionato: " + selectedCharacterPrefab.name);

            if (playerObject != null)
            {
                Destroy(playerObject); // Elimina il modello precedente se esiste
            }

            currentCharacter = Instantiate(selectedCharacterPrefab, transform.position, transform.rotation);
            currentCharacter.transform.parent = transform;
            playerObject = currentCharacter;

            Debug.Log("✅ Nuovo personaggio attivato: " + currentCharacter.name);
        }
        else
        {
            Debug.LogError("❌ ERRORE: Il prefab del personaggio selezionato è nullo!");
        }
    }


    void Update()
    {

            // Controlla se il personaggio sta correndo
        animator.SetBool("IsJumping", isJumping);

        // Se il giocatore perde, attiva l'animazione di caduta
        if (CollisioneOstacolo.lostGame)
        {
            animator.SetBool("HasLost", true);
        }
        if (canMove)
        {
            // Aggiorna la distanza percorsa
            distanceCovered += Time.deltaTime * initialSpeed;

            // Calcola la velocità logaritmica
            playerSpeed = initialSpeed + Mathf.Log(1 + distanceCovered) * growthRate;

            // Movimento in avanti con la velocità logaritmica
            transform.Translate(Vector3.forward * Time.deltaTime * playerSpeed, Space.World);

            // Gestisci input per PC e mobile
            HandleInput();
        }
    }

    void HandleInput()
    {
        // Controllo degli input da tastiera per PC
        if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
        {
            HandleKeyboardInput();
        }
        // Controllo degli input touch per mobile
        else
        {
            HandleTouchInput();
        }
    }

    void HandleKeyboardInput()
    {
        // Movimento laterale a sinistra
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            if (transform.position.x > leftLimit)
            {
                transform.Translate(Vector3.left * Time.deltaTime * horizontalSpeed);
            }
        }

        // Movimento laterale a destra
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            if (transform.position.x < rightLimit)
            {
                transform.Translate(Vector3.right * Time.deltaTime * horizontalSpeed);
            }
        }

        // Salto
        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            isJumping = true;
            playerObject.GetComponent<Animator>().Play("Jump"); // Esegui animazione di salto
            StartCoroutine(JumpSequence());
        }
    }

    void HandleTouchInput()
    {
        // Controlla se ci sono tocchi sullo schermo
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Gestisci il movimento laterale
            if (touch.phase == TouchPhase.Moved)
            {
                float move = touch.deltaPosition.x * horizontalSpeed * Time.deltaTime;

                // Movimento a sinistra
                if (move < 0 && transform.position.x > leftLimit)
                {
                    transform.Translate(Vector3.left * Mathf.Abs(move));
                }
                // Movimento a destra
                else if (move > 0 && transform.position.x < rightLimit)
                {
                    transform.Translate(Vector3.right * Mathf.Abs(move));
                }
            }

            // Gestione dello swipe per il salto
            if (touch.phase == TouchPhase.Began)
            {
                // Memorizza la posizione iniziale del tocco
                startTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                // Memorizza la posizione finale del tocco
                endTouchPosition = touch.position;

                // Calcola la distanza dello swipe
                float swipeDistance = endTouchPosition.y - startTouchPosition.y;

                // Controlla se è uno swipe verso l'alto
                if (swipeDistance > swipeThreshold && !isJumping)
                {
                    isJumping = true;
                    playerObject.GetComponent<Animator>().Play("Jump"); // Esegui animazione di salto
                    StartCoroutine(JumpSequence());
                }
            }
        }
    }

    IEnumerator JumpSequence()
    {
        // Fase di salto (salita)
        float jumpSpeed = jumpHeight / (timeInAir / 2); // Velocità per salire fino all'altezza di salto
        float elapsedTime = 0;

        // Salita
        while (elapsedTime < timeInAir / 2)
        {
            transform.Translate(Vector3.up * jumpSpeed * Time.deltaTime, Space.World);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Fase di discesa (discesa)
        elapsedTime = 0;
        while (elapsedTime < timeInAir / 2)
        {
            // Controllo se il personaggio è sceso al di sotto della sua altezza originale
            if (transform.position.y <= originalY + epsilon)
            {
                transform.position = new Vector3(transform.position.x, originalY, transform.position.z);
                break;
            }
            transform.Translate(Vector3.down * jumpSpeed * Time.deltaTime, Space.World);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Forza la posizione Y alla fine del salto
        transform.position = new Vector3(transform.position.x, originalY, transform.position.z);

        // Fine del salto, reset variabili e animazione
        isJumping = false;
        playerObject.GetComponent<Animator>().Play("Fast Run 1"); // Ritorno all'animazione di corsa
    }
}