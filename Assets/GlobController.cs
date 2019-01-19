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

    public Transform[] vertices;

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

    /*public class Tetrahedron
    {
        public Vector3 A { get; set; }
        public Vector3 B { get; set; }
        public Vector3 C { get; set; }
        public Vector3 D { get; set; }
        public Tetrahedron()
        {
            A = new Vector3(0.0f, 0.0f, 0.0f);
            B = new Vector3(0.0f, 0.0f, 0.0f);
            C = new Vector3(0.0f, 0.0f, 0.0f);
            D = new Vector3(0.0f, 0.0f, 0.0f);
        }
    }*/

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

        if (vertices.Length == 0)
        {
            GameObject go = new GameObject("Vertex 000");
            go.transform.parent = this.transform;
            Array.Resize(ref vertices, vertices.Length + 1);
            vertices[vertices.Length] = go.transform;
        }
        else
        {
            GameObject go = Instantiate(vertices[0]).gameObject;
            go.transform.parent = this.transform;
            go.name = "Vertex " + vertices.Length.ToString("000");
            Array.Resize(ref vertices, vertices.Length + 1);
            vertices[vertices.Length] = go.transform;
        }
    }

    public void findVertices()
    {
        vertices = this.gameObject.GetComponentsInChildren<Transform>();
        vertices[0] = null;
        for (int i = 0; i < vertices.Length - 1; i++)
        {
            vertices[i] = vertices[i + 1];
        }
        Array.Resize(ref vertices, vertices.Length - 1);
    }

    public Vector3 calcCOM()
    {
        float averagedX = 0.0f;
        float averagedY = 0.0f;
        float averagedZ = 0.0f;
        for (int i = 0; i < vertices.Length; i++)
        {
            averagedX += vertices[i].position.x;
            averagedY += vertices[i].position.y;
            averagedZ += vertices[i].position.z;
        }
        averagedX /= 4.0f;
        averagedY /= 4.0f;
        averagedZ /= 4.0f;
        return new Vector3(averagedX, averagedY, averagedZ);
    }

    public void calcPoints()
    {
        sideSet[0] = Mathf.Clamp(sideSet[0], min, max);
        sideSet[1] = Mathf.Clamp(sideSet[1], min, max);
        sideSet[2] = Mathf.Clamp(sideSet[2], min, max);
        sideSet[3] = Mathf.Clamp(sideSet[3], min, max);
        sideSet[4] = Mathf.Clamp(sideSet[4], min, max);
        sideSet[5] = Mathf.Clamp(sideSet[5], min, max);

        sideLength[0] = Mathf.SmoothDamp(sideLength[0], sideSet[0] + b, ref sideVel[0], 1.0f);
        sideLength[1] = Mathf.SmoothDamp(sideLength[1], sideSet[1] + b, ref sideVel[1], 1.0f);
        sideLength[2] = Mathf.SmoothDamp(sideLength[2], sideSet[2] + b, ref sideVel[2], 1.0f);
        sideLength[3] = Mathf.SmoothDamp(sideLength[3], sideSet[3] + b, ref sideVel[3], 1.0f);
        sideLength[4] = Mathf.SmoothDamp(sideLength[4], sideSet[4] + b, ref sideVel[4], 1.0f);
        sideLength[5] = Mathf.SmoothDamp(sideLength[5], sideSet[5] + b, ref sideVel[5], 1.0f);

        Tetrahedron t = tm.edge_input(
            sideLength[0] * a + b, sideLength[1] * a + b,
            sideLength[2] * a + b, sideLength[3] * a + b,
            sideLength[4] * a + b, sideLength[5] * a + b
        );

        point[0] = t.A;
        point[1] = t.B;
        point[2] = t.C;
        point[3] = t.D;

        vertices[0].localPosition = point[0];
        vertices[1].localPosition = point[1];
        vertices[2].localPosition = point[2];
        vertices[3].localPosition = point[3];
    }

    void FixedUpdate()
    {
        findVertices();

        centerMass = calcCOM();

        rotation.z = Quaternion.FromToRotation(Vector3.right, new Vector3(0, point[2].y, 0)).z;
        //rotation.z = rotation.z - Mathf.PI / 2.0f;

        // Sets the rotation so that the transform's y-axis goes along the z-axis
        //transform.rotation = Quaternion.FromToRotation(Vector3.up, transform.forward);

        calcPoints();

        if (!colorPicked)
        {
            Material newMat = new Material(Shader.Find("Standard"));
            pickedColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1.0f);
            this.gameObject.GetComponent<MeshRenderer>().material = newMat;
            this.gameObject.GetComponent<MeshRenderer>().material.color = pickedColor;
            colorPicked = true;
        }

        //tetrahedrons[0, 0] = vertices[0];
        //tetrahedrons[0, 1] = vertices[1];
        //tetrahedrons[0, 2] = vertices[2];
        //tetrahedrons[0, 3] = vertices[3];

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