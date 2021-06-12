using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
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
    public Text LivesDisplay, ScoreDisplay, DEDDisplay;

    private int CurrentLives;
    private int Score = 0;
    private float DistPerSegment = .2f;
    private GameObject Player1, Player2, RopeHolder, GlobalHazard;
    private bool TakeNoDamage = false;

    void Start () {
        // GM Setup
        GlobalHazard = GameObject.Find ("Global Hazard");
        CurrentLives = StartingLives;
        SpawnPlayer ();

        // UI Setup
        LivesDisplay.text = CurrentLives.ToString ();
        ScoreDisplay.text = Score.ToString ();
    }

    void Update () {
        if (Player1 != null && Player2 != null) {
            int height = Mathf.RoundToInt (Mathf.Max (Player1.transform.position.y, Player2.transform.position.y));
            if (height > Score) {
                Score = height;
                ScoreDisplay.text = Score.ToString ();
            }
        }
    }

    void SpawnPlayer () {
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
        CameraTargets.AddMember (Player1.transform, CameraWeight, CameraRadius);
        CameraTargets.AddMember (Player2.transform, CameraWeight, CameraRadius);

        // Invoke the rope
        int SegmentCount = Mathf.CeilToInt (Vector2.Distance (Player1.transform.position, Player2.transform.position) / DistPerSegment);
        CreateRope (Player1, Player2, SegmentCount);
        AddDistanceJoint (Player1, Player2, SegmentCount * DistPerSegment);
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
        GlobalHazard.transform.position = LastCheckpointPos.position + (Vector3.down * 14);
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
        DEDDisplay.gameObject.SetActive (true);
    }
}