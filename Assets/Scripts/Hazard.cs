using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D other){
        if (other.CompareTag("Player")){
            Destroy(other.gameObject);
            GameObject.Find("Game Manager").SendMessage("Damage");
        }
    }
}
