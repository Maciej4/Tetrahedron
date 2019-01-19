using System;
using System.Linq;
using UnityEngine;

public class TetraRenderer
{
    public MeshFilter meshFilter;
    public MeshCollider meshCollider;
    public Transform[,] transforms = new Transform[2, 4];
    public Vector3[] point = new Vector3[4];

    public TetraRenderer(MeshFilter meshFilter_, MeshCollider meshCollider_)
    {
        meshFilter = meshFilter_;
        meshCollider = meshCollider_;
    }

    public Vector3[] calcVertices()
    {
        Vector3[] output = new Vector3[transforms.Length*3];
        int[] pointArray = new int[12] {0,1,2,0,2,3,2,1,3,0,3,1 };

        for (int i = 0; i < transforms.Length / 4; i++)
        {
            Vector3[] tempVertices = new Vector3[4] {
                transforms[i, 0].localPosition, transforms[i, 1].localPosition,
                transforms[i, 2].localPosition, transforms[i, 3].localPosition
            };

            for (int j = 0; j < 12; j++)
            {
                output[j + (i * 12)] = tempVertices[pointArray[j]];
            }
        }

        return output;
    }

    public int[] calcTriangles()
    {
        int[] output = new int[transforms.Length*3];

        for (int i = 0; i < output.Length; i++)
        {
            output[i] = i;
        }

        return output;
    }

    public Vector2[] calcUvs()
    {
        Vector2[] output = new Vector2[transforms.Length*3];

        Vector2 uv0 = new Vector2(0, 0);
        Vector2 uv1 = new Vector2(1, 0);
        Vector2 uv2 = new Vector2(0.5f, 1);

        for (int i = 0; i < output.Length/3; i++)
        {
            output[0+(i * 3)] = uv0;
            output[1+(i * 3)] = uv1;
            output[2+(i * 3)] = uv2;
        }

        return output;
    }

    public void tetrahedrons(Transform[,] transforms_)
    {
        transforms = transforms_;
    }

    public void loop()
    {
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

        mesh.vertices = calcVertices();

        mesh.triangles = calcTriangles();

        mesh.uv = calcUvs();

        mesh.triangles = mesh.triangles.Reverse().ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshCollider.sharedMesh = mesh;
        meshCollider.convex = true;
    }
}
