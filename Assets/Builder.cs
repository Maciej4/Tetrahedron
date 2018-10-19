using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Rigidbody))]

public class Builder : MonoBehaviour
{
    public float thrust = 10f;
    public Rigidbody rigidBody = null;
    MeshFilter meshFilter = null;
    MeshCollider meshCollider = null;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        rigidBody = GetComponent<Rigidbody>();
        meshCollider = GetComponent<MeshCollider>();

        if (meshFilter == null)
        {
            Debug.LogError("MeshFilter not found!");
            return;
        }

        Vector3 p0 = new Vector3(0, 0, 0);
        Vector3 p1 = new Vector3(1, 0, 0);
        Vector3 p2 = new Vector3(0.5f, 0, Mathf.Sqrt(0.75f));
        Vector3 p3 = new Vector3(0.5f, Mathf.Sqrt(0.75f), Mathf.Sqrt(0.75f) / 3);

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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Debug.Log("Adding force");
            rigidBody.AddForce(Vector3.up * thrust, ForceMode.Impulse);
            rigidBody.AddTorque(Vector3.right * 0.01f, ForceMode.Impulse);
        }
    }
}