using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemProximityNotifier : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // checking that its the player that is close
        {
            Debug.Log($"{gameObject.name}: Player is near object!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log($"{gameObject.name}: Player has moved away from object.");
        }
     }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
