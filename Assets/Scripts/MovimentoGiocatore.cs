using System.Collections;
using UnityEngine;

public class MovimentoGiocatore : MonoBehaviour
{
    // Riferimento al modello effettivo istanziato
    static private GameObject modelObject;

    // Parametri di movimento
    public float initialSpeed = 2f;
    public float growthRate = 0.5f;
    public float horizontalSpeed = 3f;
    public float rightLimit = 3.2f;
    public float leftLimit = -3.2f;
    public static bool canMove = false;

    // Parametri di salto
    public bool isJumping = false;
    public float jumpHeight = 3f;
    public float timeInAir = 1f;

    private float originalY;
    private float epsilon = 0.01f;
    private float distanceCovered = 0f;
    public float playerSpeed;

    // Variabili per lo swipe su mobile
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private float swipeThreshold = 50f;

    void Start()
    {
        CollisioneOstacolo.lostGame = false;
        Debug.Log("MovimentoGiocatore.Start(): lostGame = " + CollisioneOstacolo.lostGame);

        // Memorizza l’altezza iniziale
        originalY = transform.position.y;

        // Recupera l’indice del personaggio selezionato
        int selectedIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        Debug.Log("Personaggio selezionato: " + selectedIndex);

        // Trova il CharacterManager
        CharacterManager cm = FindObjectOfType<CharacterManager>();
        if (cm == null)
        {
            Debug.LogError("CharacterManager non trovato in scena!");
            return;
        }

        // Prendi il prefab del personaggio
        if (selectedIndex < 0 || selectedIndex >= cm.characters.Count)
        {
            Debug.LogError("Indice personaggio non valido: " + selectedIndex);
            return;
        }
        GameObject selectedPrefab = cm.characters[selectedIndex].gamePrefab;

        if (selectedPrefab == null)
        {
            Debug.LogError("Prefab del personaggio selezionato è nullo!");
            return;
        }
        Debug.Log("Prefab selezionato: " + selectedPrefab.name);

        // LOG per vedere i figli esistenti PRIMA di distruggerli
        Debug.Log($"Prima di distruggere i figli, transform ha {transform.childCount} child.");

        // Se avevi già un modello, lo distruggi prima (in caso di reload della scena)
        foreach (Transform child in transform)
        {
            // Aggiungiamo un log per capire cosa c'è come figlio
            Debug.Log($"Trovo child '{child.name}' con tag '{child.tag}'.");
            if (child.CompareTag("PlayerModel"))
            {
                Debug.Log($"-- Distruggo '{child.name}' perché ha il tag 'PlayerModel'.");
                Destroy(child.gameObject);
            }
        }

        // LOG per vedere quanti figli rimangono
        Debug.Log($"Dopo il ciclo di distruzione, transform ha {transform.childCount} child.");

        // Istanzia il nuovo modello come figlio di questo "PlayerRoot"
        modelObject = Instantiate(selectedPrefab, transform.position, transform.rotation);
        modelObject.transform.SetParent(this.transform, false);

        // Azzera la trasformazione locale per farlo combaciare col pivot del Player
        // Ad esempio, posizione locale a (0, -0.69, 0)
        modelObject.transform.localPosition = new Vector3(0f, -0.69f, 0f);
        modelObject.transform.localRotation = Quaternion.identity;

        // Assegna il tag "PlayerModel" così la prossima volta potrai distruggerlo
        modelObject.tag = "PlayerModel";
        CollisioneOstacolo.charModel = modelObject;

        Debug.Log("Nuovo personaggio attivato: " + modelObject.name);


        /* Animator anim = modelObject.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetBool("HasLost", false);
            Debug.Log("MovimentoGiocatore.Start(): SetBool('HasLost', false)");

            anim.Play("Fast Run");
            Debug.Log("MovimentoGiocatore.Start(): Forzo animazione 'Fast Run'");
        } */
    }

    void Update()
    {
        // Se il giocatore ha perso
        if (CollisioneOstacolo.lostGame)
        {
            Debug.Log("MovimentoGiocatore.Update(): lostGame == true; imposta 'HasLost' a true");
            Animator modelAnimator = modelObject ? modelObject.GetComponent<Animator>() : null;
            if (modelAnimator != null)
            {
                modelAnimator.SetBool("HasLost", true);
            }
        }

        // Se possiamo muoverci
        if (canMove)
        {
            // Aumenta la distanza percorsa
            distanceCovered += Time.deltaTime * initialSpeed;
            // Calcola velocità logaritmica
            playerSpeed = initialSpeed + Mathf.Log(1 + distanceCovered) * growthRate;

            // Movimento in avanti
            transform.Translate(Vector3.forward * Time.deltaTime * playerSpeed, Space.World);

            HandleInput();
        }
    }

    void HandleInput()
    {
        // Differenzia input PC da input mobile
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            HandleTouchInput();
        }
        else
        {
            HandleKeyboardInput();
        }
    }

    void HandleKeyboardInput()
    {
        // Movimento laterale a sinistra
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            if (transform.position.x > leftLimit)
            {
                transform.Translate(Vector3.left * horizontalSpeed * Time.deltaTime);
            }
        }

        // Movimento laterale a destra
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            if (transform.position.x < rightLimit)
            {
                transform.Translate(Vector3.right * horizontalSpeed * Time.deltaTime);
            }
        }

        // Salto
        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            isJumping = true;
            PlayModelAnimation("Jump");
            StartCoroutine(JumpSequence());
        }
    }

    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Movimento laterale durante il touch
            if (touch.phase == TouchPhase.Moved)
            {
                float move = touch.deltaPosition.x * horizontalSpeed * Time.deltaTime;
                float newX = Mathf.Clamp(transform.position.x + move, leftLimit, rightLimit);
                transform.position = new Vector3(newX, transform.position.y, transform.position.z);
            }

            // Swipe per salto
            if (touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                endTouchPosition = touch.position;
                float swipeDistance = endTouchPosition.y - startTouchPosition.y;

                if (swipeDistance > swipeThreshold && !isJumping)
                {
                    isJumping = true;
                    PlayModelAnimation("Jump");
                    StartCoroutine(JumpSequence());
                }
            }
        }
    }

    IEnumerator JumpSequence()
    {
        float jumpSpeed = jumpHeight / (timeInAir / 2f);
        float elapsed = 0f;

        // Fase di salita
        while (elapsed < timeInAir / 2f)
        {
            transform.Translate(Vector3.up * jumpSpeed * Time.deltaTime, Space.World);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Fase di discesa
        elapsed = 0f;
        while (elapsed < timeInAir / 2f)
        {
            if (transform.position.y <= originalY + epsilon)
            {
                transform.position = new Vector3(transform.position.x, originalY, transform.position.z);
                break;
            }
            transform.Translate(Vector3.down * jumpSpeed * Time.deltaTime, Space.World);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Rimetti il giocatore a terra in modo pulito
        transform.position = new Vector3(transform.position.x, originalY, transform.position.z);

        isJumping = false;
        // Torna all'animazione di corsa
        PlayModelAnimation("Fast Run");
    }

    // Metodo di utilità per avviare animazioni sul modello
    public static void PlayModelAnimation(string animationName)
    {
        if (modelObject != null)
        {
            Animator anim = modelObject.GetComponent<Animator>();
            if (anim != null)
            {
                anim.Play(animationName);
            }
        }
    }
}
