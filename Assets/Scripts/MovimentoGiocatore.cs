/*using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class MovimentoGiocatore : MonoBehaviour
{
    public float playerSpeed = 2;
    public float horizontalSpeed = 3;
    public float rightLimit = 3.2f;
    public float leftLimit = -3.2f;
    static public bool canMove = false;
    public bool isJumping = false;
    public bool comingDown = false;
    public GameObject playerObject;
    public float jumpingSpace = 3;
    public float timeInAir = 1;
    private float 
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * playerSpeed, Space.World);
        if (canMove == true)
        {

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                if (this.gameObject.transform.position.x > leftLimit)
                {
                    transform.Translate(Vector3.left * Time.deltaTime * horizontalSpeed);
                }

            }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                if (this.gameObject.transform.position.x < rightLimit)
                {
                    transform.Translate(Vector3.left * Time.deltaTime * horizontalSpeed * -1);
                }
            }
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Space)){
                if(isJumping == false){
                    isJumping = true;
                    playerObject.GetComponent<Animator>().Play("Jump");
                    StartCoroutine(JumpSequence());
                }
            }
        }
        if(isJumping == true){
            if(comingDown == false){
                transform.Translate(Vector3.up * Time.deltaTime * jumpingSpace, Space.World);
            }
            if(comingDown == true){
                transform.Translate(Vector3.up * Time.deltaTime * -jumpingSpace, Space.World);
            }
        }
    }

    IEnumerator JumpSequence(){
        yield return new WaitForSeconds(timeInAir/2);
        comingDown = true;
        yield return new WaitForSeconds(timeInAir/2);
        isJumping = false;
        comingDown = false;
        playerObject.GetComponent<Animator>().Play("Fast Run 1");
    }
}
*/
using System.Collections;
using UnityEngine;

public class MovimentoGiocatore : MonoBehaviour
{
    public float playerSpeed = 2;
    public float horizontalSpeed = 3;
    public float rightLimit;
    public float leftLimit;
    public static bool canMove = false;
    public bool isJumping = false;
    public float jumpHeight = 3; // Altezza del salto
    public float timeInAir = 1; // Tempo totale in aria
    public GameObject playerObject;

    private float originalY; // Posizione Y iniziale del personaggio
    private float epsilon = 0.01f; // Tolleranza per piccoli errori di calcolo

    void Start()
    {
        // Memorizza la posizione Y originale del personaggio
        originalY = transform.position.y;
        leftLimit += transform.position.x;
        rightLimit += transform.position.x;
    }

    void Update()
    {
        // Movimento in avanti costante
        transform.Translate(Vector3.forward * Time.deltaTime * playerSpeed, Space.World);

        if (canMove)
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
            if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Space)) && !isJumping)
            {
                isJumping = true;
                playerObject.GetComponent<Animator>().Play("Jump"); // Esegui animazione di salto
                StartCoroutine(JumpSequence());
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