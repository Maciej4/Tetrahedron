using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Rigidbody))]

[ExecuteInEditMode]
public class Builder : MonoBehaviour
{
    MeshFilter meshFilter = null;
    MeshCollider meshCollider = null;

    public Vector3 p0 = new Vector3(0, 0, 0);
    public Vector3 p1 = new Vector3(1, 0, 0);
    public Vector3 p2 = new Vector3(0.5f, 0, Mathf.Sqrt(0.75f));
    public Vector3 p3 = new Vector3(0.5f, Mathf.Sqrt(0.75f), Mathf.Sqrt(0.75f) / 3);

    public float side0set = 0.0f;
    public float side1set = 0.0f;
    public float side2set = 0.0f;
    public float side3set = 0.0f;
    public float side4set = 0.0f;
    public float side5set = 0.0f;

    private float side0vel = 0.0f;
    private float side1vel = 0.0f;
    private float side2vel = 0.0f;
    private float side3vel = 0.0f;
    private float side4vel = 0.0f;
    private float side5vel = 0.0f;

    private float side0 = 1.0f;
    private float side1 = 1.0f;
    private float side2 = 1.0f;
    private float side3 = 1.0f;
    private float side4 = 1.0f;
    private float side5 = 1.0f;

    private float a = 0.9f;
    private float b = 1.0f;
    private float min = 0.0f;
    private float max = 1.0f;

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

    public float limitHeight(float altitude)
    {
        return Mathf.Clamp(altitude, 0, 0);
    }

    public void calcPoints()
    {
        side0set = Mathf.Clamp(side0set, min, max);
        side1set = Mathf.Clamp(side1set, min, max);
        side2set = Mathf.Clamp(side2set, min, max);
        side3set = Mathf.Clamp(side3set, min, max);
        side4set = Mathf.Clamp(side4set, min, max);
        side5set = Mathf.Clamp(side5set, min, max);

        side0 = Mathf.SmoothDamp(side0, side0set + b, ref side0vel, 1.0f);
        side1 = Mathf.SmoothDamp(side1, side1set + b, ref side1vel, 1.0f);
        side2 = Mathf.SmoothDamp(side2, side2set + b, ref side2vel, 1.0f);
        side3 = Mathf.SmoothDamp(side3, side3set + b, ref side3vel, 1.0f);
        side4 = Mathf.SmoothDamp(side4, side4set + b, ref side4vel, 1.0f);
        side5 = Mathf.SmoothDamp(side5, side5set + b, ref side5vel, 1.0f);

        Tetrahedron t = edge_input(side0 * a + b, side1 * a + b, side2 * a + b, side3 * a + b, side4 * a + b, side5 * a + b);

        p0 = t.A;
        p1 = t.B;
        p2 = t.C;
        p3 = t.D;
    }

    void FixedUpdate()
    {
        calcPoints();

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
            p0,p1,p2,
            p0,p2,p3,
            p2,p1,p3,
            p0,p3,p1
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


        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshCollider.sharedMesh = mesh;
        meshCollider.convex = true;
    }
}