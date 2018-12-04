using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NewWalk : MonoBehaviour {
    public Transform goalTransform;

    private Builder builder;

    private Vector3 aPos;
    private Vector3 bPos;
    private Vector3 cPos;
    private Vector3 dPos;

    public Transform[] transforms;

    private List<Transform> distanceSortedList;
    public List<Transform> heightSortedList;
    public List<Transform> groundSortedList;

    public bool walking;
    public bool startWalk = false;
    //private bool kinematicsEnabled = true;
    private int highestPoint = 0;
    public float[] setArray = { };
    private int curHighPoint;
    //private float lastRun;

    // Use this for initialization
    void Start()
    {
        builder = this.gameObject.GetComponent<Builder>();
        transforms = this.gameObject.GetComponentsInChildren<Transform>();
    }

    private int[][] pointSideArray = {
        new int[] {11, 0, 1, 2 },
        new int[] {0, 12, 3, 4 },
        new int[] {1, 3, 13, 5 },
        new int[] {2, 4, 5, 14 }
    };

    float[] walk()
    {
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
        float[] output = { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
        output[a0] = 1.0f;
        output[a1] = 0.3f;
        output[a2] = 0.0f;
        output[a3] = 0.3f;
        output[a4] = 0.0f;
        output[a5] = 1.0f;
        return output;
    }

    void stopWalking()
    {
        walking = false;
    }

    int findTransform(Transform t)
    {
        int z = 4;
        if (t.Equals(transforms[1])) { z = 0; }
        else if (t.Equals(transforms[2])) { z = 1; }
        else if (t.Equals(transforms[3])) { z = 2; }
        else if (t.Equals(transforms[4])) { z = 3; }
        return z;
    }

    int pointSide(int a, int b)
    {
        return pointSideArray[a][b];
    }

    void zero()
    {
        builder.side0set = 0.0f;
        builder.side1set = 0.0f;
        builder.side2set = 0.0f;
        builder.side3set = 0.0f;
        builder.side4set = 0.0f;
        builder.side5set = 0.0f;
    }

    void calcDist()
    {
        List<Transform> unsortedList = new List<Transform>();
        transforms[1].localPosition = builder.p0;
        transforms[2].localPosition = builder.p1;
        transforms[3].localPosition = builder.p2;
        transforms[4].localPosition = builder.p3;
        unsortedList.Add(transforms[1]);
        unsortedList.Add(transforms[2]);
        unsortedList.Add(transforms[3]);
        unsortedList.Add(transforms[4]);
        distanceSortedList = unsortedList.OrderBy(o => Vector3.Distance(o.position, goalTransform.position)).ToList();
        groundSortedList = distanceSortedList;
        heightSortedList = unsortedList.OrderBy(o => o.position.y).ToList();
        highestPoint = findTransform(heightSortedList[3]);
        if (heightSortedList[3].Equals(groundSortedList[0])) { groundSortedList.RemoveAt(0); }
        else if (heightSortedList[3].Equals(groundSortedList[1])) { groundSortedList.RemoveAt(1); }
        else if (heightSortedList[3].Equals(groundSortedList[2])) { groundSortedList.RemoveAt(2); }
        else if (heightSortedList[3].Equals(groundSortedList[3])) { groundSortedList.RemoveAt(3); }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transforms = this.gameObject.GetComponentsInChildren<Transform>();
        calcDist();

        if (startWalk && !walking)
        {
            setArray = walk();
            curHighPoint = highestPoint;
        }

        startWalk = false;

        if (walking)
        {
            builder.side0set = setArray[0];
            builder.side1set = setArray[1];
            builder.side2set = setArray[2];
            builder.side3set = setArray[3];
            builder.side4set = setArray[4];
            builder.side5set = setArray[5];
        }
        else
        {
            zero();
        }

        if (!(curHighPoint == highestPoint)) { stopWalking(); }
    }
}
