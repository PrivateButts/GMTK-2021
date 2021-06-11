using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public GameObject PlayerPrefab;
    public Transform Spawn1, Spawn2;
    public GameObject RopeSegment;
    private float DistPerSegment = .2f;

    void Start () {
        GameObject p1 = Instantiate (PlayerPrefab, Spawn1.position, Spawn1.rotation);
        GameObject p2 = Instantiate (PlayerPrefab, Spawn2.position, Spawn2.rotation);
        p1.GetComponent<PlayerController> ().PlayerID = "1";
        p2.GetComponent<PlayerController> ().PlayerID = "2";

        int SegmentCount = Mathf.CeilToInt (Vector2.Distance (p1.transform.position, p2.transform.position) / DistPerSegment);
        CreateRope (p1, p2, SegmentCount);
        AddDistanceJoint (p1, p2, SegmentCount * DistPerSegment);
    }

    void CreateRope (GameObject t1, GameObject t2, int segments) {
        GameObject container = new GameObject ("Spawned Rope");
        container.transform.position = Vector2.Lerp (t2.transform.position, t1.transform.position, 0.5f);
        List<GameObject> rsegs = new List<GameObject> ();
        for (int i = 0; i < segments; i++) {
            GameObject rseg = Instantiate (RopeSegment, container.transform);
            HingeJoint2D end = rseg.GetComponent<HingeJoint2D> ();
            if (i == 0) {
                end.connectedBody = t1.GetComponent<Rigidbody2D> ();
                end.connectedAnchor = Vector2.zero;
            } else {
                end.connectedBody = rsegs[i - 1].GetComponent<Rigidbody2D> ();
                AddDistanceJoint (rseg, t1, (i * DistPerSegment) + 0.1f);
                AddDistanceJoint (rseg, t2, ((segments - i) * DistPerSegment) + 0.1f);
            }

            if (i == segments - 1) {
                HingeJoint2D otherEnd = rseg.AddComponent<HingeJoint2D> ();
                otherEnd.enableCollision = false;
                otherEnd.autoConfigureConnectedAnchor = false;
                otherEnd.anchor = new Vector2 (0.1f, 0);
                otherEnd.connectedAnchor = Vector2.zero;
                otherEnd.connectedBody = t2.GetComponent<Rigidbody2D> ();
            }

            rsegs.Add (rseg);
        }
    }

    void AddDistanceJoint (GameObject Parent, GameObject Target, float Distance) {
        DistanceJoint2D distJoint = Parent.AddComponent<DistanceJoint2D> ();
        distJoint.autoConfigureDistance = false;
        distJoint.distance = Distance;
        distJoint.maxDistanceOnly = true;
        distJoint.anchor = Vector2.zero;
        distJoint.autoConfigureConnectedAnchor = false;
        distJoint.connectedBody = Target.GetComponent<Rigidbody2D> ();
        distJoint.connectedAnchor = Vector2.zero;
    }
}