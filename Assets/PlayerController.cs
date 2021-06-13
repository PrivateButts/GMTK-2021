using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public bool CanGrab, OverrideGrab = false;
    public float SwingForce, MaxAngularVelo = 500f;
    public string PlayerID;
    public GameObject OtherPlayer;

    private HingeJoint2D GrabJoint, OtherGrabJoint;
    private Rigidbody2D rb;
    private GameObject GrabbedSprite, SwingSprite;

    private void Start () {
        // Hinges
        GrabJoint = GetComponent<HingeJoint2D> ();
        GrabJoint.enabled = true;
        OtherGrabJoint = OtherPlayer.GetComponent<HingeJoint2D> ();

        // Varb searching
        rb = GetComponent<Rigidbody2D> ();
        GrabbedSprite = transform.Find("Grabbed").gameObject;
        GrabbedSprite.SetActive(true);
        SwingSprite = transform.Find("Swing").gameObject;
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
                GrabbedSprite.SetActive(false);
                SwingSprite.SetActive(true);
            }else if (Input.GetButton ("Grab" + PlayerID) || Input.GetAxis("TriggerGrab" + PlayerID) > .5f){
                OverrideGrab = false;
            }
        }

        if (!GrabJoint.enabled && rb.velocity.sqrMagnitude > 1 && OtherPlayer != null && OtherGrabJoint.enabled) {
            if (Input.GetAxis("Horizontal") < 0){
                transform.right = (OtherPlayer.transform.position - transform.position) * -1;
                SwingSprite.GetComponent<SpriteRenderer>().flipY = true;
            } else if (Input.GetAxis("Horizontal") > 0){
                transform.right = (OtherPlayer.transform.position - transform.position);
                SwingSprite.GetComponent<SpriteRenderer>().flipY = false;
            }
        }

        rb.angularVelocity = Mathf.Clamp(rb.angularVelocity, -MaxAngularVelo, MaxAngularVelo);
    }

    void FixedUpdate () {
        if (!GrabJoint.enabled) {
            rb.AddRelativeForce (Vector2.down * Mathf.Abs(Input.GetAxis ("Horizontal")) * SwingForce * Time.deltaTime);
            // rb.AddForce (Vector2.up * Input.GetAxis ("Vertical") * SwingForce * Time.deltaTime);
        }
    }

    void Grab () {
        Debug.Log ("Grabbed");
        GrabJoint.connectedAnchor = transform.position;
        GrabJoint.enabled = true;
        GrabbedSprite.SetActive(true);
        SwingSprite.SetActive(false);
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