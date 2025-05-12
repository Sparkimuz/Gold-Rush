using Firebase.Database;
using System.Collections;
using System.Threading.Tasks;
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

        originalY = transform.position.y;

        StartCoroutine(LoadSelectedCharacter());
    }

    IEnumerator LoadSelectedCharacter()
    {
        if (FirebaseController.Instance.user == null)
        {
            Debug.LogError("❌ Nessun utente loggato, impossibile caricare il personaggio!");
            yield break;
        }

        string userId = FirebaseController.Instance.user.UserId;
        var dbRef = FirebaseController.Instance.dbReference;

        Task<DataSnapshot> task = dbRef.Child("users").Child(userId).Child("selectedCharacter").GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Result.Exists)
        {
            int selectedIndex = int.Parse(task.Result.Value.ToString());
            Debug.Log("✅ Personaggio selezionato recuperato da Firebase: " + selectedIndex);

            // Dopo aver ottenuto l'indice, carica il personaggio
            LoadCharacterModel(selectedIndex);
        }
        else
        {
            Debug.LogWarning("⚠️ Nessun personaggio selezionato trovato, imposto default (0).");
            LoadCharacterModel(0);
        }
    }


    void LoadCharacterModel(int selectedIndex)
    {
        CharacterManager cm = FindObjectOfType<CharacterManager>();
        if (cm == null)
        {
            Debug.LogError("❌ CharacterManager non trovato in scena!");
            return;
        }

        if (selectedIndex < 0 || selectedIndex >= cm.characters.Count)
        {
            Debug.LogError("❌ Indice personaggio non valido: " + selectedIndex);
            return;
        }

        GameObject selectedPrefab = cm.characters[selectedIndex].gamePrefab;
        if (selectedPrefab == null)
        {
            Debug.LogError("❌ Il prefab del personaggio selezionato è nullo!");
            return;
        }

        Debug.Log("🎭 Prefab selezionato: " + selectedPrefab.name);

        // Rimuove eventuali modelli esistenti prima di istanziare il nuovo
        foreach (Transform child in transform)
        {
            if (child.CompareTag("PlayerModel"))
            {
                Debug.Log($"🚮 Rimuovo modello precedente: {child.name}");
                Destroy(child.gameObject);
            }
        }

        // Istanzia il nuovo modello
        modelObject = Instantiate(selectedPrefab, transform.position, transform.rotation);
        modelObject.transform.SetParent(this.transform, false);
        modelObject.transform.localPosition = new Vector3(0f, -0.69f, 0f);
        modelObject.transform.localRotation = Quaternion.identity;

        modelObject.tag = "PlayerModel";
        CollisioneOstacolo.charModel = modelObject;

        Debug.Log("✅ Nuovo personaggio attivato: " + modelObject.name);
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

    /*void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // Ottieni la posizione del tocco in pixel
                float touchX = touch.position.x;

                // Calcola la metà dello schermo
                float screenHalf = Screen.width / 2f;

                if (touchX < screenHalf && transform.position.x > leftLimit)
                {
                    // Tocca nella parte sinistra dello schermo → vai a sinistra
                    transform.Translate(Vector3.left * horizontalSpeed * Time.deltaTime);
                }
                else if (touchX >= screenHalf && transform.position.x < rightLimit)
                {
                    // Tocca nella parte destra dello schermo → vai a destra
                    transform.Translate(Vector3.right * horizontalSpeed * Time.deltaTime);
                }
                else if (touch.phase == TouchPhase.Ended){
                    endTouchPosition = touch.position;
                    float swipeDistance = endTouchPosition.y - startTouchPosition.y;

                    // SWIPE verso l'alto per salto
                    if (swipeDistance > swipeThreshold && !isJumping)
                    {
                        isJumping = true;
                        PlayModelAnimation("Jump");
                        StartCoroutine(JumpSequence());
                    }
                }
            }
        }
    }*/
    void HandleTouchInput()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);

        // Calcola le 3 zone a runtime
        float halfWidth   = Screen.width * 0.5f;          // 50 % sinistra / 50 % destra
        float jumpHeight  = Screen.height * 0.2f;        // 15 % basso  → salto

        bool inLeftHalf   = touch.position.x < halfWidth;
        bool inRightJump  = (touch.position.x >= halfWidth) && (touch.position.y <  jumpHeight);
        bool inRightMove  = (touch.position.x >= halfWidth) && (touch.position.y >= jumpHeight);

        /* ---------- 1.  SALTO (tap nella zona in basso a destra) ---------- */
        if (inRightJump && (touch.phase == TouchPhase.Began && !isJumping))
        {
            isJumping = true;
            PlayModelAnimation("Jump");
            StartCoroutine(JumpSequence());                 // evita di analizzare swipe in questo frame
        }

        /* ---------- 2.  MOVIMENTO LATERALE ---------- */
            // Dal lato sinistro: accetti solo spostamenti verso sinistra
            if (inLeftHalf && transform.position.x > leftLimit)
                transform.Translate(Vector3.left * horizontalSpeed * Time.deltaTime);

            // Dal lato destro‑alto: accetti solo spostamenti verso destra
            else if (inRightMove && transform.position.x < rightLimit)
                transform.Translate(Vector3.right * horizontalSpeed * Time.deltaTime);
    }
/*public void StartJump()              // reso pubblico per eventuale tasto UI
{
    isJumping = true;
    playerObject.GetComponent<Animator>().Play("Jump");
    StartCoroutine(JumpSequence());
}*/


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
