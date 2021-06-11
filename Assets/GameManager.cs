using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour{
    public GameObject PlayerPrefab;
    public GameObject RopeSegment;

    
    void CreateRope(GameObject t1, GameObject t2, int segments){
        List<GameObject> rsegs = new List<GameObject>();
        for (int i = 0; i < segments; i++){
            GameObject rseg = Instantiate(RopeSegment);
            HingeJoint2D[] ends = rseg.GetComponents<HingeJoint2D>();
            if(i == 0){
                ends[0].connectedBody = t1.GetComponent<Rigidbody2D>();
            }else{
                ends[0].connectedBody = rsegs[i-1].GetComponent<Rigidbody2D>();
            }

            if(i == segments - 1){
                ends[1].connectedBody = t2.GetComponent<Rigidbody2D>();
            }
        }
    }
}
