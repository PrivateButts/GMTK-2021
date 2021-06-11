using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public bool CanGrab = false;
    public string PlayerID;
    private HingeJoint2D GrabJoint;

    private void Start () {
        GrabJoint = GetComponent<HingeJoint2D> ();
        GrabJoint.enabled = false;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetButtonDown ("Grab" + PlayerID)) {
            Debug.Log ("Attempted Grab");
            if (CanGrab && !GrabJoint.enabled) {
                Grab ();
            }
        }

        if (Input.GetButtonUp ("Grab" + PlayerID)) {
            GrabJoint.enabled = false;
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