using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratoreSegmento : MonoBehaviour
{
    public GameObject[] bioma1Segments;
    public GameObject[] bioma2Segments;
    public GameObject[] bioma3Segments;  // Non utilizzato al momento
    public GameObject[] bioma4Segments;  // Non utilizzato al momento

    private List<GameObject[]> biomiSegments = new List<GameObject[]>();

    [SerializeField] int zPos = 50;
    [SerializeField] int zIncrement = 50;
    [SerializeField] bool creatingSegment = false;

    private int biomaCount = 0;
    public int biomaDuration = 5;
    private int biomaIndex = 0;

    public int maxSegments = 20; // Massimo numero di segmenti da mantenere

    private List<GameObject> segments = new List<GameObject>(); // Lista per tenere traccia dei segmenti creati

    private MovimentoGiocatore movimentoGiocatore; // Riferimento allo script di movimento del giocatore

    void Start()
    {
        // Aggiunge gli array di segmenti alla lista
        biomiSegments.Add(bioma1Segments);
        biomiSegments.Add(bioma2Segments);
        biomiSegments.Add(bioma3Segments);
        biomiSegments.Add(bioma4Segments);

        // Trova il riferimento allo script MovimentoGiocatore
        movimentoGiocatore = FindObjectOfType<MovimentoGiocatore>();
    }

    void Update()
    {
        if (!creatingSegment)
        {
            creatingSegment = true;
            StartCoroutine(SegmentGen());
        }
    }

    IEnumerator SegmentGen()
    {
        // Se il bioma corrente non ha segmenti disponibili
        while (biomiSegments[biomaIndex].Length == 0)
        {
            Debug.LogWarning("Non ci sono segmenti definiti per il bioma: " + biomaIndex);
            biomaIndex = (biomaIndex + 1) % biomiSegments.Count; // Passa al bioma successivo
        }

        // Scegli un segmento dal bioma corrente
        GameObject[] currentBiomaSegments = biomiSegments[biomaIndex];
        int segmentNum = Random.Range(0, currentBiomaSegments.Length);
        GameObject newSegment = Instantiate(currentBiomaSegments[segmentNum], new Vector3(0, 0, zPos), Quaternion.identity);
        
        // Aggiungi il nuovo segmento alla lista
        segments.Add(newSegment);

        // Rimuovi il segmento più vecchio se abbiamo superato il massimo
        if (segments.Count > maxSegments)
        {
            GameObject oldSegment = segments[0];
            segments.RemoveAt(0); // Rimuove il segmento più vecchio dalla lista
            Destroy(oldSegment); // Distrugge il segmento dal gioco
        }

        zPos += zIncrement;
        biomaCount++;

        // Cambia bioma se è stato raggiunto il numero di segmenti richiesti
        if (biomaCount >= biomaDuration)
        {
            biomaCount = 0;
            biomaIndex = (biomaIndex + 1) % biomiSegments.Count; // Passa al bioma successivo
        }

        // Calcola l'intervallo per la generazione del segmento in base alla velocità del giocatore
        float playerSpeed = movimentoGiocatore != null ? movimentoGiocatore.playerSpeed : 10f; // Default a 10 se movimentoGiocatore è nullo
        float segmentGenerationInterval = Mathf.Clamp(zIncrement / playerSpeed, 0.1f, 3f);

        // Aspetta per un periodo di tempo calcolato dinamicamente prima di creare il prossimo segmento
        yield return new WaitForSeconds(segmentGenerationInterval);
        creatingSegment = false;
    }
}
