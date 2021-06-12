using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalHazard : MonoBehaviour {
    public float StartDelay = 1f;
    public float MovementSpeed = 1f;

    private void Start () {
        StartCoroutine ("startMoving");
    }

    IEnumerator startMoving () {
        yield return new WaitForSeconds (StartDelay);
        while (true) {
            transform.Translate (Vector3.up * MovementSpeed);
            yield return new WaitForEndOfFrame ();
        }
    }

    private void OnTriggerEnter2D (Collider2D other) {
        if (other.CompareTag ("Player")) {
            Debug.Log ("Ded");
            StopCoroutine ("startMoving");
        }
    }
}