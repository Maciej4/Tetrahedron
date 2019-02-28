using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimpleWalkController {
    public Vector3 goalPos = new Vector3(220.0f, -0.5f, 220.0f);

    private TetraController globController;
    private GameObject targetGlob;

    private List<Vertex> walkVertices = new List<Vertex>();
    private List<Vertex> heightSortedList = new List<Vertex>();
    private List<Vertex> finalSortedList = new List<Vertex>();

    public bool walking = false;
    public bool continuousWalk = false;
    private float currentEndDistance = 2.0f;
    public bool initWalk = false;
    public float[] setArray = new float[6];
    private int walkStartHighPoint;
    public float startWalkTime;
    public float currentTime;
    public float endTime;
    public const float timeoutTime = 7.0f;

    public SimpleWalkController(GameObject targetGlob_)
    {
        targetGlob = targetGlob_;
        globController = targetGlob.GetComponent<TetraController>();
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
        output[a0] = 2.9f; //1.0
        output[a1] = 2.3f; //0.3
        output[a2] = 2.0f; //0.0
        output[a3] = 2.3f; //0.3
        output[a4] = 2.0f; //0.0
        output[a5] = 2.9f; //1.0
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

    private int findVertex(Vertex vertex)
    {
        return vertex.ID;
    }

    private int pointSide(int a, int b)
    {
        return pointSideArray[a][b];
    }

    public void zero()
    {
        for(int q = 0; q < globController.sideSetList.Count; q++)
        {
            globController.sideSetList[q] = 2.0f;
        }
    }

    private void calcDist()
    {
        Debug.Log("Vertices: " + walkVertices.Count);
        List<Vertex> unsortedList = new List<Vertex>();
        unsortedList.Add(walkVertices[0]);
        unsortedList.Add(walkVertices[1]);
        unsortedList.Add(walkVertices[2]);
        unsortedList.Add(walkVertices[3]);
        Debug.Log("Unsorted List: " + unsortedList.Count);
        finalSortedList = unsortedList.OrderBy(o => Vector3.Distance(targetGlob.transform.TransformPoint(o.pos), goalPos)).ToList();
        heightSortedList = unsortedList.OrderBy(o => targetGlob.transform.TransformPoint(o.pos).y).ToList();
        if (heightSortedList[3].Equals(finalSortedList[0])) { finalSortedList.RemoveAt(0); }
        else if (heightSortedList[3].Equals(finalSortedList[1])) { finalSortedList.RemoveAt(1); }
        else if (heightSortedList[3].Equals(finalSortedList[2])) { finalSortedList.RemoveAt(2); }
        else if (heightSortedList[3].Equals(finalSortedList[3])) { finalSortedList.RemoveAt(3); }
        finalSortedList.Add(heightSortedList[3]);
    }

    // Update is called once per frame
    public void loop()
    {
        int i = 0;
        walkVertices = new List<Vertex>();
        foreach(Vertex vertex in globController.vertices)
        {
            walkVertices.Add(vertex);
            i++;
        }

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
            for (int j = 0; j < 5; j++)
            {
                globController.sideSetList[j] = setArray[j];
            }
        }
        else
        {
            zero();
        }

        if (continuousWalk)
        {
            if (Vector3.Distance(globController.centerMass, goalPos) < currentEndDistance)
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
