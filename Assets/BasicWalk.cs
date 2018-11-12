using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BasicWalk : MonoBehaviour {
    public PointControl targetController;
    public Transform goalTransform;
    public SidePicker sidePicker;
    public Rigidbody targetRigidbody;

    private Vector3 aPos;
    private Vector3 bPos;
    private Vector3 cPos;
    private Vector3 dPos;

    public List<Transform> distanceSortedList;
    private List<Transform> heightSortedList;
    public List<Transform> groundSortedList;

    public bool walking;
    public bool startWalk = false;
    public bool kinematicsEnabled = true;
    private int walkSide = 0;
    private int highestPoint = 0;
    //private float[] startArray = { 1.0f, 0.0f, 0.5f, 0.0f, 0.5f, 1.0f };
    private float[] startArray = { 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
    public float[] setArray = { };
    private int curHighPoint;

    private int[][] pointSideArray = {   
        //new int[] {11, 0, 4, 3 },
        //new int[] {0, 12, 2, 1 },
        //new int[] {4, 2, 13, 5 },
        //new int[] {3, 1, 5, 14 }
        new int[] {11, 0, 1, 2 },
        new int[] {0, 12, 3, 4 },
        new int[] {1, 3, 13, 5 },
        new int[] {2, 4, 5, 14 }
    };

    float[] walk() {
        walking = true;
        int p0 = findTransform(groundSortedList[0]);
        int p1 = findTransform(groundSortedList[1]);
        int p2 = findTransform(groundSortedList[2]);
        int p3 = highestPoint; /*
        int a0 = pointSide(p0, p1);
        int a1 = pointSide(p0, p2);
        int a2 = pointSide(p0, p3);
        int a3 = pointSide(p1, p2);
        int a4 = pointSide(p1, p3);
        int a5 = pointSide(p2, p3); */
        int a0 = pointSide(p0, p1);
        int a1 = pointSide(p1, p3);
        int a2 = pointSide(p1, p2);
        int a3 = pointSide(p0, p3);
        int a4 = pointSide(p0, p2);
        int a5 = pointSide(p3, p2);
        return new float[] { startArray[a0], startArray[a1], startArray[a2], startArray[a3], startArray[a4], startArray[a5] };
    }

    void stopWalking() {
        walking = false;
    }

    // Use this for initialization
    void Start() {
        
    }

    int findTransform(Transform t) {
        int z = 4;
        if (t.Equals(targetController.alpha)) { z = 0; }
        else if (t.Equals(targetController.beta)) { z = 1; }
        else if (t.Equals(targetController.charlie)) { z = 2; }
        else if (t.Equals(targetController.delta)) { z = 3; }
        return z;
    }

    int pointSide(int a, int b)
    {
        return pointSideArray[a][b];
    }

    void zero() {
        targetController.side0set = 0.0f;
        targetController.side1set = 0.0f;
        targetController.side2set = 0.0f;
        targetController.side3set = 0.0f;
        targetController.side4set = 0.0f;
        targetController.side5set = 0.0f;
    }

    void calcDist() {
        List<Transform> unsortedList = new List<Transform>();
        unsortedList.Add(targetController.alpha);
        unsortedList.Add(targetController.beta);
        unsortedList.Add(targetController.charlie);
        unsortedList.Add(targetController.delta);
        distanceSortedList = unsortedList.OrderBy(o => Vector3.Distance(o.transform.TransformPoint(Vector3.zero), goalTransform.position)).ToList();
        groundSortedList = distanceSortedList;
        heightSortedList = unsortedList.OrderBy(o => o.transform.TransformPoint(Vector3.zero).y).ToList();
        highestPoint = findTransform(heightSortedList[3]);
        if (heightSortedList[3].Equals(groundSortedList[0])) { groundSortedList.RemoveAt(0); }
        else if (heightSortedList[3].Equals(groundSortedList[1])) { groundSortedList.RemoveAt(1); }
        else if (heightSortedList[3].Equals(groundSortedList[2])) { groundSortedList.RemoveAt(2); }
        else if (heightSortedList[3].Equals(groundSortedList[3])) { groundSortedList.RemoveAt(3); }
        
    }
	
	// Update is called once per frame
	void Update() {
        calcDist();
        int closestPoint1 = findTransform(groundSortedList[0]);
        int closestPoint2 = findTransform(groundSortedList[1]);
        walkSide = pointSide(closestPoint1, closestPoint2);

        if (startWalk && !walking)
        {
            setArray = walk();
            curHighPoint = highestPoint;
        }

        startWalk = false;

        if (walking)
        {
            targetController.side0set = setArray[0];
            targetController.side1set = setArray[1];
            targetController.side2set = setArray[2];
            targetController.side3set = setArray[3];
            targetController.side4set = setArray[4];
            targetController.side5set = setArray[5];
        }
        else
        {
            zero();
        }

        if (!(curHighPoint == highestPoint)) { stopWalking(); }

        targetRigidbody.isKinematic = kinematicsEnabled;
        
        sidePicker.targetSide = walkSide;
    }
}
