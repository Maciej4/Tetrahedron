using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Rigidbody))]

[ExecuteInEditMode]
public class GlobController : MonoBehaviour
{
    MeshFilter meshFilter = null;
    MeshCollider meshCollider = null;
    private TetraMath tm = new TetraMath();
    private TetraRenderer tr;

    public HashSet<Vertex> vertices = new HashSet<Vertex>();

    public HashSet<Side> sides = new HashSet<Side>();

    public Vector3[] point = new Vector3[4] {
        new Vector3(0, 0, 0),
        new Vector3(1, 0, 0),
        new Vector3(0.5f, 0, Mathf.Sqrt(0.75f)),
        new Vector3(0.5f, Mathf.Sqrt(0.75f), Mathf.Sqrt(0.75f) / 3)
    };

    public float[] sideSet = { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };

    private float[] sideVel = { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };

    private float[] sideLength = { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f };

    public Transform[,] tetrahedrons = new Transform[1,4];

    public Transform[] vertexTransforms;

    private float a = 0.9f;
    private float b = 1.0f;
    private float min = 0.0f;
    private float max = 1.0f;
    public bool colorPicked = false;
    private Color pickedColor;
    public Vector3 centerMass;
    public bool wantsToConnect = false;
    public TetraController connectTarget = null;
    public Quaternion rotation;

    // Use this for initialization

    void Start()
    {
        colorPicked = false;
        MeshFilter meshFilter_ = GetComponent<MeshFilter>();
        MeshCollider meshCollider_ = GetComponent<MeshCollider>();
        tr = new TetraRenderer(meshFilter_, meshCollider_);
    }

    public Quaternion findRotation(Vector3 v3Pos1, Vector3 v3Pos2) 
    {
        return Quaternion.FromToRotation(point[2], v3Pos2 - v3Pos1);
    }

    public void makeVertex()
    {
        findVertices();

        if (vertexTransforms.Length == 0)
        {
            GameObject go = new GameObject("Vertex 000");
            go.transform.parent = this.transform;
            Array.Resize(ref vertexTransforms, vertexTransforms.Length + 1);
            vertexTransforms[vertexTransforms.Length] = go.transform;
        }
        else
        {
            GameObject go = Instantiate(vertexTransforms[0]).gameObject;
            go.transform.parent = this.transform;
            go.name = "Vertex " + vertexTransforms.Length.ToString("000");
            Array.Resize(ref vertexTransforms, vertexTransforms.Length + 1);
            vertexTransforms[vertexTransforms.Length] = go.transform;
        }
    }

    public void findVertices()
    {
        vertexTransforms = this.gameObject.GetComponentsInChildren<Transform>();
        vertexTransforms[0] = null;
        for (int i = 0; i < vertexTransforms.Length - 1; i++)
        {
            vertexTransforms[i] = vertexTransforms[i + 1];
            vertices.Add(new Vertex(vertexTransforms[i + 1]));
        }
        Array.Resize(ref vertexTransforms, vertexTransforms.Length - 1);
    }

    public Vector3 calcCOM()
    {
        float averagedX = 0.0f;
        float averagedY = 0.0f;
        float averagedZ = 0.0f;
        for (int i = 0; i < vertexTransforms.Length; i++)
        {
            averagedX += vertexTransforms[i].position.x;
            averagedY += vertexTransforms[i].position.y;
            averagedZ += vertexTransforms[i].position.z;
        }
        averagedX /= 4.0f;
        averagedY /= 4.0f;
        averagedZ /= 4.0f;
        return new Vector3(averagedX, averagedY, averagedZ);
    }

    public void calcPoints()
    {
        for(int i = 0; i < 5; i++)
        {
            sideSet[i] = Mathf.Clamp(sideSet[i], min, max);
            sideLength[i] = Mathf.SmoothDamp(sideLength[i], sideSet[i] + b, ref sideVel[i], 1.0f);
        }

        Tetrahedron t = tm.edge_input(
            sideLength[0] * a + b, sideLength[1] * a + b,
            sideLength[2] * a + b, sideLength[3] * a + b,
            sideLength[4] * a + b, sideLength[5] * a + b
        );

        point[0] = t.A;
        point[1] = t.B;
        point[2] = t.C;
        point[3] = t.D;

        vertexTransforms[0].localPosition = point[0];
        vertexTransforms[1].localPosition = point[1];
        vertexTransforms[2].localPosition = point[2];
        vertexTransforms[3].localPosition = point[3];
    }

    void FixedUpdate()
    {
        findVertices();

        centerMass = calcCOM();

        rotation.z = Quaternion.FromToRotation(Vector3.right, new Vector3(0, point[2].y, 0)).z;

        Debug.Log(vertices.Count);

        calcPoints();

        if (!colorPicked)
        {
            Material newMat = new Material(Shader.Find("Standard"));
            pickedColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1.0f);
            this.gameObject.GetComponent<MeshRenderer>().material = newMat;
            this.gameObject.GetComponent<MeshRenderer>().material.color = pickedColor;
            colorPicked = true;
        }

        if (tr == null)
        {
            MeshFilter meshFilter_ = GetComponent<MeshFilter>();
            MeshCollider meshCollider_ = GetComponent<MeshCollider>();
            tr = new TetraRenderer(meshFilter_, meshCollider_);
        }

        tr.transforms[0, 0] = GetComponentsInChildren<Transform>()[1];
        tr.transforms[0, 1] = GetComponentsInChildren<Transform>()[2];
        tr.transforms[0, 2] = GetComponentsInChildren<Transform>()[3];
        tr.transforms[0, 3] = GetComponentsInChildren<Transform>()[4];

        tr.transforms[1, 0] = GetComponentsInChildren<Transform>()[3];
        tr.transforms[1, 1] = GetComponentsInChildren<Transform>()[2];
        tr.transforms[1, 2] = GetComponentsInChildren<Transform>()[1];
        tr.transforms[1, 3] = GetComponentsInChildren<Transform>()[5];

        tr.loop();
        
    }
}