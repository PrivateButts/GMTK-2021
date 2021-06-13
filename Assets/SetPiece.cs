using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPiece : MonoBehaviour {
    public GameObject Player1, Player2, RopeHolder, RopeSegment;
    private float DistPerSegment = .2f;

    void Start () {
        // Invoke the rope
        int SegmentCount = Mathf.CeilToInt (Vector2.Distance (Player1.transform.position, Player2.transform.position) / DistPerSegment);
        CreateRope (Player1, Player2, SegmentCount);
    }

    void CreateRope (GameObject t1, GameObject t2, int segments) {
        RopeHolder.tag = "Rope";
        RopeHolder.transform.position = Vector2.Lerp (t2.transform.position, t1.transform.position, 0.5f);
        List<GameObject> rsegs = new List<GameObject> ();
        for (int i = 0; i < segments; i++) {
            GameObject rseg = Instantiate (RopeSegment, RopeHolder.transform);
            HingeJoint2D end = rseg.GetComponent<HingeJoint2D> ();
            if (i == 0) {
                end.connectedBody = t1.GetComponent<Rigidbody2D> ();
                end.connectedAnchor = Vector2.zero;
            } else {
                end.connectedBody = rsegs[i - 1].GetComponent<Rigidbody2D> ();
                // AddDistanceJoint (rseg, t1, (i * DistPerSegment) + .5f);
                // AddDistanceJoint (rseg, t2, ((segments - i) * DistPerSegment) + .5f);
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
        RecalcDistanceJoints ();
    }

    void RecalcDistanceJoints () {
        int segmentCnt = RopeHolder.transform.childCount;

        // // Players
        // Component[] p1joints = Player1.GetComponents<DistanceJoint2D> () as Component[];
        // foreach (Component joint in p1joints) {
        //     Destroy (joint as DistanceJoint2D);
        // }
        // Component[] p2joints = Player2.GetComponents<DistanceJoint2D> () as Component[];
        // foreach (Component joint in p2joints) {
        //     Destroy (joint as DistanceJoint2D);
        // }
        // AddDistanceJoint (Player1, Player2, segmentCnt * DistPerSegment + 1.5f);

        // Ropes
        for (int i = 0; i < segmentCnt; i++) {
            GameObject Segment = RopeHolder.transform.GetChild (i).gameObject;
            Component[] joints = Segment.GetComponents<DistanceJoint2D> () as Component[];
            foreach (Component joint in joints) {
                Destroy (joint as DistanceJoint2D);
            }
        }
        for (int i = 0; i < segmentCnt; i++) {
            GameObject Segment = RopeHolder.transform.GetChild (i).gameObject;
            AddDistanceJoint (Segment, Player1, ((i + 1) * DistPerSegment) + 0);
            AddDistanceJoint (Segment, Player2, ((segmentCnt - i) * DistPerSegment) + 0);
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