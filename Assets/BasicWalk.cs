using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicWalk : MonoBehaviour {
    public PointControl targetController;
    public Transform targetTransform;
    public bool walking;

    public int[] face0 = { 0, 1, 2, 3, 4, 5 };
    public int[] face1 = { 0, 1, 2, 3, 4, 5 };
    public int[] face2 = { 0, 1, 2, 3, 4, 5 };
    public int[] face3 = { 0, 1, 2, 3, 4, 5 };
    private int[] relOrg = { 0, 1, 2, 3, 4, 5 };
    private float[] setArray = { 0, 0, 0, 0, 0, 0 };

    private float relSet0;
    private float relSet1;
    private float relSet2;
    private float relSet3;
    private float relSet4;
    private float relSet5;


    void Walk(int side) {
        walking = true;
    }

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {


        targetController.side0set = setArray[relOrg[0]];
        targetController.side1set = setArray[relOrg[1]];
        targetController.side2set = setArray[relOrg[2]];
        targetController.side3set = setArray[relOrg[3]];
        targetController.side4set = setArray[relOrg[4]];
        targetController.side5set = setArray[relOrg[5]];
    }
}
