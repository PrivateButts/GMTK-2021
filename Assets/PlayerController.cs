using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public bool CanGrab = false;
    public float SwingForce = 500f;
    public string PlayerID;
    public GameObject OtherPlayer;

    private HingeJoint2D GrabJoint;
    private Rigidbody2D rb;

    private void Start () {
        GrabJoint = GetComponent<HingeJoint2D> ();
        GrabJoint.enabled = true;

        rb = GetComponent<Rigidbody2D> ();
    }

    // Update is called once per frame
    void Update () {
        // Grab Input Code
        if (Input.GetButtonDown ("Grab" + PlayerID)) {
            Debug.Log ("Attempted Grab");
            if (CanGrab && !GrabJoint.enabled) {
                Grab ();
            }
        }

        if (Input.GetButtonUp ("Grab" + PlayerID)) {
            GrabJoint.enabled = false;
        }

        if (!GrabJoint.enabled) {
            transform.right = OtherPlayer.transform.position - transform.position;
        }
    }

    void FixedUpdate () {
        if (!GrabJoint.enabled) {
            rb.AddRelativeForce (Vector2.down * Input.GetAxis ("Horizontal") * SwingForce * Time.deltaTime);
            // rb.AddForce (Vector2.up * Input.GetAxis ("Vertical") * SwingForce * Time.deltaTime);
        }
    }

    void Grab () {
        Debug.Log ("Grabbed");
        GrabJoint.connectedAnchor = transform.position;
        GrabJoint.enabled = true;
    }

    void OnTriggerEnter2D (Collider2D other) {
        Debug.Log ("Entered");
        if (other.CompareTag ("Grabbable")) {
            CanGrab = true;
        }
    }
    void OnTriggerExit2D (Collider2D other) {
        Debug.Log ("Exited");
        if (other.CompareTag ("Grabbable")) {
            CanGrab = false;
        }
    }
}