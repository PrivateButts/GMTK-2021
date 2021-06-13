using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public bool CanGrab, OverrideGrab = false;
    public float SwingForce = 500f;
    public string PlayerID;
    public GameObject OtherPlayer;

    private HingeJoint2D GrabJoint, OtherGrabJoint;
    private Rigidbody2D rb;

    private void Start () {
        GrabJoint = GetComponent<HingeJoint2D> ();
        GrabJoint.enabled = true;
        OtherGrabJoint = OtherPlayer.GetComponent<HingeJoint2D> ();

        rb = GetComponent<Rigidbody2D> ();
    }

    // Update is called once per frame
    void Update () {
        // Grab Input Code
        if(!GrabJoint.enabled){
            if (Input.GetButton ("Grab" + PlayerID) || Input.GetAxis("TriggerGrab" + PlayerID) > .5f) {
                Debug.Log ("Attempted Grab");
                if (CanGrab) {
                    Grab ();
                }
            }
        }else{
            if (!OverrideGrab && !Input.GetButton ("Grab" + PlayerID) && Input.GetAxis("TriggerGrab" + PlayerID) <= .5f) {
                GrabJoint.enabled = false;
            }else if (Input.GetButton ("Grab" + PlayerID) || Input.GetAxis("TriggerGrab" + PlayerID) > .5f){
                OverrideGrab = false;
            }
        }

        

        if (!GrabJoint.enabled && rb.velocity.sqrMagnitude > 1 && OtherPlayer != null && OtherGrabJoint.enabled) {
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