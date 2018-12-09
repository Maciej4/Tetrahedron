using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WalkController : MonoBehaviour {
    public Vector3 goalPos = new Vector3(220.0f, -0.5f, 220.0f);

    private TetraController builder;

    private Vector3 aPos;
    private Vector3 bPos;
    private Vector3 cPos;
    private Vector3 dPos;

    private Transform[] transforms;

    private List<Transform> heightSortedList;
    public List<Transform> finalSortedList;

    public bool walking = false;
    public bool continuousWalk = false;
    private float currentEndDistance;
    public bool initWalk = false;
    //private bool kinematicsEnabled = true;
    public float[] setArray = { };
    private int walkStartHighPoint;
    //private float lastRun;
    public float startWalkTime;
    public float currentTime;
    public float endTime;
    public const float timeoutTime = 7.0f;

    // Use this for initialization
    void Start()
    {
        builder = this.gameObject.GetComponent<TetraController>();
        transforms = this.gameObject.GetComponentsInChildren<Transform>();
    }

    private int[][] pointSideArray = {
        new int[] {11, 0, 1, 2 },
        new int[] {0, 12, 3, 4 },
        new int[] {1, 3, 13, 5 },
        new int[] {2, 4, 5, 14 }
    };

    private float[] walk()
    {
        walking = true;
        int p0 = findVertex(finalSortedList[0]);
        int p1 = findVertex(finalSortedList[1]);
        int p2 = findVertex(finalSortedList[2]);
        int p3 = findVertex(finalSortedList[3]);
        int a0 = pointSide(p0, p1);
        int a1 = pointSide(p1, p3);
        int a2 = pointSide(p1, p2);
        int a3 = pointSide(p0, p3);
        int a4 = pointSide(p0, p2);
        int a5 = pointSide(p3, p2);
        float[] output = { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
        output[a0] = 1.0f;
        output[a1] = 0.3f;
        output[a2] = 0.0f;
        output[a3] = 0.3f;
        output[a4] = 0.0f;
        output[a5] = 1.0f;
        return output;
    }

    public void startWalk(Transform newTargetTransform) {
        initWalk = true;
        if (!(newTargetTransform == null))
        {
            goalPos = newTargetTransform.position;
        }
    }

    public void stopWalking()
    {
        walking = false;
    }

    public void startContinuousWalk(Transform newTargetTransform, float endDistance = 2.0f)
    {
        continuousWalk = true;
        goalPos = newTargetTransform.position;
        currentEndDistance = endDistance;
    }

    private int findVertex(Transform t)
    {
        int z = 4;
        if (t.Equals(transforms[1])) { z = 0; }
        else if (t.Equals(transforms[2])) { z = 1; }
        else if (t.Equals(transforms[3])) { z = 2; }
        else if (t.Equals(transforms[4])) { z = 3; }
        return z;
    }

    private int pointSide(int a, int b)
    {
        return pointSideArray[a][b];
    }

    private void zero()
    {
        builder.sideSet[0] = 0.0f;
        builder.sideSet[1] = 0.0f;
        builder.sideSet[2] = 0.0f;
        builder.sideSet[3] = 0.0f;
        builder.sideSet[4] = 0.0f;
        builder.sideSet[5] = 0.0f;
    }

    private void calcDist()
    {
        List<Transform> unsortedList = new List<Transform>();
        List<Transform> heightSortedList = new List<Transform>();
        unsortedList.Add(transforms[1]);
        unsortedList.Add(transforms[2]);
        unsortedList.Add(transforms[3]);
        unsortedList.Add(transforms[4]);
        finalSortedList = unsortedList.OrderBy(o => Vector3.Distance(o.position, goalPos)).ToList();
        heightSortedList = unsortedList.OrderBy(o => o.position.y).ToList();
        if (heightSortedList[3].Equals(finalSortedList[0])) { finalSortedList.RemoveAt(0); }
        else if (heightSortedList[3].Equals(finalSortedList[1])) { finalSortedList.RemoveAt(1); }
        else if (heightSortedList[3].Equals(finalSortedList[2])) { finalSortedList.RemoveAt(2); }
        else if (heightSortedList[3].Equals(finalSortedList[3])) { finalSortedList.RemoveAt(3); }
        finalSortedList.Add(heightSortedList[3]);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transforms = this.gameObject.GetComponentsInChildren<Transform>();

        calcDist();

        if (initWalk && !walking)
        {
            setArray = walk();
            walkStartHighPoint = findVertex(finalSortedList[3]);
            startWalkTime = Time.time;
        }

        initWalk = false;

        if (walking)
        {
            builder.sideSet = setArray;
        }
        else
        {
            zero();
        }

        if (continuousWalk)
        {
            if (Vector3.Distance(builder.centerMass, goalPos) < currentEndDistance)
            {
                continuousWalk = false;
                goalPos = new Vector3(220.0f, -0.5f, 220.0f);
            }

            if (!walking)
            {
                initWalk = true;
            }
        }

        currentTime = Time.time;
        endTime = startWalkTime + timeoutTime;

        if (
            !(walkStartHighPoint == findVertex(finalSortedList[3]))
            ||(Time.fixedTime>=startWalkTime+timeoutTime)
        )
        {
            stopWalking();
        }
    }
}
