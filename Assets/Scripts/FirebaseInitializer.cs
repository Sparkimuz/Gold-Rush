using UnityEngine;

public class FirebaseInitializer : MonoBehaviour
{
    void Awake()
    {
        if (FirebaseController.Instance == null)
        {
            GameObject firebaseObj = GameObject.Find("FirebaseController");
            if (firebaseObj == null)
            {
                firebaseObj = new GameObject("FirebaseController");
                firebaseObj.AddComponent<FirebaseController>();
            }
        }
    }
}