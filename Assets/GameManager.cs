using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public GameObject PlayerPrefab;
    public GameObject RopeSegment;
    public CinemachineTargetGroup CameraTargets;
    public float CameraWeight, CameraRadius;
    public Transform LastCheckpointPos;
    public int LastCheckpointScore = 0;
    public int StartingLives;
    public float RespawnDelay = 3;
    public float GHRespawnOffset = 20;
    public Text LivesDisplay, ScoreDisplay, DeadScoreDisplay;
    public AudioSource BGNormal, BGPanic;
    public float CrossFadeStart, CrossFadeEnd;
    public int MinRopeSegs, MaxRopeSegs;
    public int Score = 0;

    public GameObject[] DisableOnStart, EnableOnStart, DisableOnGameOver, EnableOnGameOver;

    private int CurrentLives;
    private float DistPerSegment = .2f;
    private GameObject Player1, Player2, RopeHolder, GlobalHazard;
    private bool TakeNoDamage, DoingRopeOperation = false;

    void Start () {
        // GM Setup
        GlobalHazard = GameObject.Find ("Global Hazard");
        CurrentLives = StartingLives;
        SpawnPlayer ();

        // UI Setup
        LivesDisplay.text = CurrentLives.ToString ();
        ScoreDisplay.text = Score.ToString ();

        BulkSetActive (EnableOnStart, true);
        BulkSetActive (DisableOnStart, false);

    }

    void Update () {
        if (Player1 != null && Player2 != null) {
            int max_height = Mathf.RoundToInt (Mathf.Max (Player1.transform.position.y, Player2.transform.position.y));
            int min_height = Mathf.RoundToInt (Mathf.Min (Player1.transform.position.y, Player2.transform.position.y));
            if (max_height > Score) {
                Score = max_height;
                ScoreDisplay.text = Score.ToString ();
            }

            float distance = min_height - GlobalHazard.transform.position.y - (GlobalHazard.transform.localScale.y / 2) - CrossFadeStart;
            // Debug.Log (distance);
            float crossfade = Mathf.Clamp01 (distance / (CrossFadeEnd - CrossFadeStart));
            // Debug.Log (crossfade);
            BGNormal.volume = crossfade;
            BGPanic.volume = 1 - crossfade;
        }

        if (!DoingRopeOperation && Input.GetButtonDown ("RetractRope")) {
            DoingRopeOperation = true;
            StartCoroutine ("RetractRope");
        }

        if (!DoingRopeOperation && Input.GetButtonDown ("ExtendRope")) {
            DoingRopeOperation = true;
            StartCoroutine ("ExtendRope");
        }
    }

    void SpawnPlayer () {
        DoingRopeOperation = true;
        // Extract exact spawn positions
        Transform spawn1 = LastCheckpointPos.Find ("1");
        Transform spawn2 = LastCheckpointPos.Find ("2");

        // Player Prefab Setup
        Player1 = Instantiate (PlayerPrefab, spawn1.position, spawn1.rotation);
        Player2 = Instantiate (PlayerPrefab, spawn2.position, spawn2.rotation);
        Player1.GetComponent<PlayerController> ().PlayerID = "1";
        Player2.GetComponent<PlayerController> ().PlayerID = "2";
        Player1.GetComponent<PlayerController> ().OtherPlayer = Player2;
        Player2.GetComponent<PlayerController> ().OtherPlayer = Player1;
        Player1.GetComponent<PlayerController> ().OverrideGrab = true;
        Player2.GetComponent<PlayerController> ().OverrideGrab = true;
        CameraTargets.AddMember (Player1.transform, CameraWeight, CameraRadius);
        CameraTargets.AddMember (Player2.transform, CameraWeight, CameraRadius);

        // Invoke the rope
        int SegmentCount = Mathf.CeilToInt (Vector2.Distance (Player1.transform.position, Player2.transform.position) / DistPerSegment);
        CreateRope (Player1, Player2, SegmentCount);
        // AddDistanceJoint (Player1, Player2, SegmentCount * DistPerSegment + 0.5f);
        DoingRopeOperation = false;
    }

    void CreateRope (GameObject t1, GameObject t2, int segments) {
        RopeHolder = new GameObject ("Spawned Rope");
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

    IEnumerator RetractRope () {
        yield return new WaitForFixedUpdate ();
        if (RopeHolder.transform.childCount > MinRopeSegs) {
            GameObject RopeToRemove = RopeHolder.transform.GetChild (RopeHolder.transform.childCount - 1).gameObject;
            GameObject NewEnd = RopeHolder.transform.GetChild (RopeHolder.transform.childCount - 2).gameObject;
            RopeToRemove.GetComponent<HingeJoint2D> ().enabled = false;
            HingeJoint2D NewHinge = NewEnd.AddComponent<HingeJoint2D> ();
            NewHinge.enableCollision = false;
            NewHinge.autoConfigureConnectedAnchor = false;
            NewHinge.anchor = new Vector2 (0.1f, 0);
            NewHinge.connectedAnchor = Vector2.zero;
            NewHinge.connectedBody = Player2.GetComponent<Rigidbody2D> ();
            Destroy (RopeToRemove);
            RecalcDistanceJoints ();
        }
        DoingRopeOperation = false;
    }
    IEnumerator ExtendRope () {
        yield return new WaitForFixedUpdate ();
        if (RopeHolder.transform.childCount < MaxRopeSegs) {
            GameObject NextSeg = RopeHolder.transform.GetChild (RopeHolder.transform.childCount - 1).gameObject;
            GameObject rseg = Instantiate (RopeSegment, RopeHolder.transform);
            rseg.transform.position = NextSeg.transform.position;
            rseg.transform.rotation = NextSeg.transform.rotation;
            NextSeg.gameObject.transform.SetAsLastSibling ();
            NextSeg.GetComponent<HingeJoint2D> ().connectedBody = rseg.GetComponent<Rigidbody2D> ();
            GameObject PreviousSeg = RopeHolder.transform.GetChild (RopeHolder.transform.childCount - 3).gameObject;
            HingeJoint2D end = rseg.GetComponent<HingeJoint2D> ();
            end.connectedBody = PreviousSeg.GetComponent<Rigidbody2D> ();

            RecalcDistanceJoints ();
        }
        DoingRopeOperation = false;
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

    void Checkpoint (Transform point) {
        LastCheckpointPos = point;
        LastCheckpointScore = Score;
    }

    IEnumerator Respawn () {
        Destroy (Player1, RespawnDelay / 2);
        Destroy (Player2, RespawnDelay / 2);
        Destroy (RopeHolder, RespawnDelay / 2);
        yield return new WaitForSeconds (RespawnDelay);
        SpawnPlayer ();
        GlobalHazard.transform.position = LastCheckpointPos.position + (Vector3.down * GHRespawnOffset);
        GlobalHazard.SendMessage ("Start");
        Score = LastCheckpointScore;
        TakeNoDamage = false;
    }

    void Damage () {
        if (!TakeNoDamage) {
            CurrentLives -= 1;
            LivesDisplay.text = CurrentLives.ToString ();
            TakeNoDamage = true;
            if (CurrentLives < 1) {
                GameOver ();
            } else {
                StartCoroutine ("Respawn");
            }
        }
    }

    void GameOver () {
        Debug.Log ("Ded");
        BulkSetActive (EnableOnGameOver, true);
        BulkSetActive (DisableOnGameOver, false);

        DeadScoreDisplay.text = "Final Score: " + Score.ToString ();
        Invoke ("ReloadScene", 10);
    }

    void ReloadScene () {
        SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
    }

    void BulkSetActive (GameObject[] go, bool status) {
        for (int i = 0; i < go.Length; i++) {
            go[i].SetActive (status);
        }
    }
}