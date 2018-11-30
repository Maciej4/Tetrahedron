using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BasicWalk : MonoBehaviour {
    public Transform goalTransform;

    private Context context;
    private PointControl pointController;
    private SidePicker sidePicker;
    private Rigidbody targetRigidbody;

    private Vector3 aPos;
    private Vector3 bPos;
    private Vector3 cPos;
    private Vector3 dPos;

    private List<Transform> distanceSortedList;
    private List<Transform> heightSortedList;
    private List<Transform> groundSortedList;

    public bool walking;
    public bool startWalk = false;
    //private bool kinematicsEnabled = true;
    private int walkSide = 0;
    private int highestPoint = 0;
    public float[] setArray = { };
    private int curHighPoint;
    //private float lastRun;

    // Use this for initialization
    void Start()
    {
        //lastRun = Time.time;
    }

    private int[][] pointSideArray = {   
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
        int p3 = highestPoint;
        int a0 = pointSide(p0, p1);
        int a1 = pointSide(p1, p3);
        int a2 = pointSide(p1, p2);
        int a3 = pointSide(p0, p3);
        int a4 = pointSide(p0, p2);
        int a5 = pointSide(p3, p2);
        float[] output =  { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
        output[a0] = 1.0f;
        output[a1] = 0.3f;
        output[a2] = 0.0f;
        output[a3] = 0.3f;
        output[a4] = 0.0f;
        output[a5] = 1.0f;
        return output;
    }

    void stopWalking() {
        walking = false;
    }

    int findTransform(Transform t) {
        int z = 4;
        if (t.Equals(context.transforms[1])) { z = 0; }
        else if (t.Equals(context.transforms[2])) { z = 1; }
        else if (t.Equals(context.transforms[3])) { z = 2; }
        else if (t.Equals(context.transforms[4])) { z = 3; }
        return z;
    }

    int pointSide(int a, int b)
    {
        return pointSideArray[a][b];
    }

    void zero() {
        pointController.side0set = 0.0f;
        pointController.side1set = 0.0f;
        pointController.side2set = 0.0f;
        pointController.side3set = 0.0f;
        pointController.side4set = 0.0f;
        pointController.side5set = 0.0f;
    }

    void calcDist() {
        List<Transform> unsortedList = new List<Transform>();
        unsortedList.Add(context.transforms[1]);
        unsortedList.Add(context.transforms[2]);
        unsortedList.Add(context.transforms[3]);
        unsortedList.Add(context.transforms[4]);
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
    void FixedUpdate() {
        context = this.gameObject.GetComponent<Context>();
        pointController = context.pointControl;
        sidePicker = context.sidePicker;
        targetRigidbody = this.gameObject.GetComponent<Rigidbody>();

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
            pointController.side0set = setArray[0];
            pointController.side1set = setArray[1];
            pointController.side2set = setArray[2];
            pointController.side3set = setArray[3];
            pointController.side4set = setArray[4];
            pointController.side5set = setArray[5];
        }
        else
        {
            zero();
        }

        //if (lastRun + 0.05 <= Time.time) {
        //    kinematicsEnabled = !kinematicsEnabled;
        //    lastRun = Time.time;
        //}

        if (!(curHighPoint == highestPoint)) { stopWalking(); }

        //targetRigidbody.isKinematic = kinematicsEnabled;
        
        sidePicker.targetSide = walkSide;
    }
}
