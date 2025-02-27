using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMovement : MonoBehaviour
{
    public float speed = 5f; // Velocità di movimento della nuvola
    public Vector3 startPoint; // Punto di partenza fuori dallo schermo
    public Vector3 endPoint; // Punto finale fuori dallo schermo

    void Update()
    {
        // Muove la nuvola da startPoint a endPoint
        transform.position = Vector3.MoveTowards(transform.position, endPoint, speed * Time.deltaTime);

        // Riporta la nuvola a startPoint quando raggiunge endPoint
        if (Vector3.Distance(transform.position, endPoint) < 0.1f)
        {
            transform.position = startPoint;
        }
    }
}
