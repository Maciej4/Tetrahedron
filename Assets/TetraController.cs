using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Rigidbody))]

[ExecuteInEditMode]
public class TetraController : MonoBehaviour
{
    MeshFilter meshFilter = null;
    MeshCollider meshCollider = null;

    public Vector3[] point = new Vector3[4] {
        new Vector3(0, 0, 0),
        new Vector3(1, 0, 0),
        new Vector3(0.5f, 0, Mathf.Sqrt(0.75f)),
        new Vector3(0.5f, Mathf.Sqrt(0.75f), Mathf.Sqrt(0.75f) / 3)
    };

    public float[] sideSet = {0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };

    private float[] sideVel = { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };

    private float[] sideLength = { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f };

    public Transform[] transforms;

    private float a = 0.9f;
    private float b = 1.0f;
    private float min = 0.0f;
    private float max = 1.0f;
    public bool colorPicked = false;
    private Color pickedColor;
    public Vector3 centerMass;
    public bool wantsToConnect = false;
    public TetraController connectTarget = null;

    public class Tetrahedron
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
    }

    // Use this for initialization
    void Start()
    {
        colorPicked = false;
    }

    Tetrahedron edge_input(float AsB_m, float AsC_m, float AsD_m, float BsC_m, float BsD_m, float CsD_m)
    {
        float AsB_m2 = AsB_m * AsB_m;
        float AsC_m2 = AsC_m * AsC_m;
        float AsD_m2 = AsD_m * AsD_m;
        float BsC_m2 = BsC_m * BsC_m;
        float BsD_m2 = BsD_m * BsD_m;
        float CsD_m2 = CsD_m * CsD_m;
        float qx = AsB_m;
        float rx = (AsB_m2 + AsC_m2 - BsC_m2) / (2.0f * AsB_m);
        float ry = Mathf.Sqrt(AsC_m2 - rx * rx);
        float sx = (AsB_m2 + AsD_m2 - BsD_m2) / (2.0f * AsB_m);
        float sy = (BsD_m2 - (sx - qx) * (sx - qx) - CsD_m2 + (sx - rx) * (sx - rx) + ry * ry) / (2 * ry);
        float sz = Mathf.Sqrt(AsD_m2 - sx * sx - sy * sy);
        Tetrahedron t = new Tetrahedron();
        t.A = new Vector3(0.0f, 0.0f, 0.0f);
        t.B = new Vector3(qx, 0.0f, 0.0f);
        t.C = new Vector3(rx, ry, 0.0f);
        t.D = new Vector3(sx, sy, sz);
        return t;
    }

    public Vector3 calcCOM()
    {
        float averagedX = 0.0f;
        float averagedY = 0.0f;
        float averagedZ = 0.0f;
        for (int i = 1; i < 5; i++) {
            averagedX += transforms[i].position.x;
            averagedY += transforms[i].position.y;
            averagedZ += transforms[i].position.z;
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

        Tetrahedron t = edge_input(
            sideLength[0] * a + b, sideLength[1] * a + b, 
            sideLength[2] * a + b, sideLength[3] * a + b, 
            sideLength[4] * a + b, sideLength[5] * a + b
        );

        point[0] = t.A;
        point[1] = t.B;
        point[2] = t.C;
        point[3] = t.D;

        transforms[1].localPosition = point[0];
        transforms[2].localPosition = point[1];
        transforms[3].localPosition = point[2];
        transforms[4].localPosition = point[3];
    }

    void FixedUpdate()
    {
        transforms = this.gameObject.GetComponentsInChildren<Transform>();

        centerMass = calcCOM();

        calcPoints();

        if (!colorPicked) {
            Material newMat = new Material(Shader.Find("Standard"));
            pickedColor = new Color(Random.value, Random.value, Random.value, 1.0f);
            this.gameObject.GetComponent<MeshRenderer>().material = newMat;
            this.gameObject.GetComponent<MeshRenderer>().material.color = pickedColor;
            colorPicked = true;
        }

        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();

        if (meshFilter == null)
        {
            Debug.LogError("MeshFilter not found!");
            return;
        }

        Mesh mesh = meshFilter.sharedMesh;
        if (mesh == null)
        {
            meshFilter.mesh = new Mesh();
            mesh = meshFilter.sharedMesh;
        }

        mesh.Clear();

        mesh.vertices = new Vector3[]{
            point[0],point[1],point[2],
            point[0],point[2],point[3],
            point[2],point[1],point[3],
            point[0],point[3],point[1]
        };

        mesh.triangles = new int[]{
            0,1,2,
            3,4,5,
            6,7,8,
            9,10,11
        };

        Vector2 uv0 = new Vector2(0, 0);
        Vector2 uv1 = new Vector2(1, 0);
        Vector2 uv2 = new Vector2(0.5f, 1);

        mesh.uv = new Vector2[]{
            uv0,uv1,uv2,
            uv0,uv1,uv2,
            uv0,uv1,uv2,
            uv0,uv1,uv2
        };

        mesh.triangles = mesh.triangles.Reverse().ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshCollider.sharedMesh = mesh;
        meshCollider.convex = true;
    }
}