using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalHazard : MonoBehaviour {
    public float StartDelay = 1f;
    public float MovementSpeed = 1f;
    public float RubberBandDistance = 30f;

    private GameManager gameManager;

    private void Start () {
        StopAllCoroutines ();
        StartCoroutine ("startMoving");

        gameManager = GameObject.Find ("Game Manager").GetComponent<GameManager> ();
    }

    private void Update () {
        if (gameManager.Score - RubberBandDistance > transform.position.y) {
            transform.position = Vector3.up * (gameManager.Score - RubberBandDistance);
        }
    }

    IEnumerator startMoving () {
        yield return new WaitForSeconds (StartDelay);
        while (true) {
            transform.Translate (Vector3.up * MovementSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame ();
        }
    }

    private void OnTriggerEnter2D (Collider2D other) {
        if (other.CompareTag ("Player")) {
            Destroy (other.gameObject);
            GameObject.Find ("Game Manager").SendMessage ("Damage");
        }
    }
}