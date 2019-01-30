using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TetraRenderer
{
    public MeshFilter meshFilter;
    public MeshCollider meshCollider;
    public MeshRenderer meshRenderer;
    public Transform[,] transforms = new Transform[2, 4];
    public List<NewTetrahedron> renderTetras = new List<NewTetrahedron>();
    public Vector3[] point = new Vector3[4];

    public TetraRenderer(GameObject renderTarget)
    {
        meshFilter = renderTarget.GetComponent<MeshFilter>();
        meshCollider = renderTarget.GetComponent<MeshCollider>();
        meshRenderer = renderTarget.GetComponent<MeshRenderer>();
    }

    public void pickColor()
    {
        Material newMat = new Material(Shader.Find("Standard"));
        Color pickedColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1.0f);
        meshRenderer.material = newMat;
        meshRenderer.material.color = pickedColor;
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

    //public Vector3[] calcVertices(int tetraCount)
    //{
    //    Vector3[] output = new Vector3[tetraCount * 12];
    //    int[] pointArray = new int[12] { 0, 1, 2, 0, 2, 3, 2, 1, 3, 0, 3, 1 };

    //    for (int i = 0; i < tetraCount; i++)
    //    {
    //        Vector3[] tempVertices = new Vector3[4];
    //        for (int l = 0; l < 4; l++) 
    //        {
    //             tempVertices[l] = renderTetras[i].vertices[l].pos;
    //        }

    //        for (int j = 0; j < 12; j++)
    //        {
    //            output[j + (i * 12)] = tempVertices[pointArray[j]];
    //        }
    //    }
    //    return output;
    //}

    public int[] calcTriangles(int tetraCount)
    {
        int[] output = new int[tetraCount * 12];

        for (int i = 0; i < output.Length; i++)
        {
            output[i] = i;
        }

        return output;
    }

    public Vector2[] calcUvs(int tetraCount)
    {
        Vector2[] output = new Vector2[tetraCount * 12];

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

        mesh.vertices = calcVertices(); //2

        mesh.triangles = calcTriangles(2);

        mesh.uv = calcUvs(2);

        mesh.triangles = mesh.triangles.Reverse().ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshCollider.sharedMesh = mesh;
        meshCollider.convex = true;
    }
}
