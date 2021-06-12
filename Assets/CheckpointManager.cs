using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour {
    public bool Used = false;

    void OnTriggerEnter2D(Collider2D other){
        if(other.CompareTag("Player") && !Used){
            GameObject.Find("Game Manager").SendMessage("Checkpoint", transform);
        }
    }
}
