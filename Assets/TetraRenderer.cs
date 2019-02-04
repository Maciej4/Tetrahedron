using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TetraRenderer
{
    //Variable and object declarations
    public MeshFilter meshFilter;
    public MeshCollider meshCollider;
    public MeshRenderer meshRenderer;
    public List<NewTetrahedron> renderTetras = new List<NewTetrahedron>();

    //Class initializer, gets mesh related objects from globController
    public TetraRenderer(GameObject renderTarget)
    {
        meshFilter = renderTarget.GetComponent<MeshFilter>();
        meshCollider = renderTarget.GetComponent<MeshCollider>();
        meshRenderer = renderTarget.GetComponent<MeshRenderer>();
    }

    //Picks a random color for the glob
    public void pickColor()
    {
        Material newMat = new Material(Shader.Find("Standard"));
        Color pickedColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1.0f);
        meshRenderer.material = newMat;
        meshRenderer.material.color = pickedColor;
    }

    //Arranges the local coordinates of the tetrahedrons into order for rendering
    public Vector3[] calcVertices(int tetraCount)
    {
        Vector3[] output = new Vector3[tetraCount * 12];
        int[] pointArray = new int[12] { 0, 1, 2, 0, 2, 3, 2, 1, 3, 0, 3, 1 };

        for (int i = 0; i < tetraCount; i++)
        {
            Vector3[] tempVertices = new Vector3[4];
            for (int l = 0; l < 4; l++) 
            {
                 tempVertices[l] = renderTetras[i].vertices[l].pos;
            }

            for (int j = 0; j < 12; j++)
            {
                output[j + (i * 12)] = tempVertices[pointArray[j]];
            }
        }
        return output;
    }

    //Makes numbered list for rendering of triangles
    public int[] calcTriangles(int tetraCount)
    {
        int[] output = new int[tetraCount * 12];

        for (int i = 0; i < output.Length; i++)
        {
            output[i] = i;
        }

        return output;
    }

    //Makes list of vector2s for shading of tetrahedrons
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

    //Loop, called by globController
    public void loop()
    {
        //Sets up mesh on first run
        Mesh mesh = meshFilter.sharedMesh;

        if (mesh == null)
        {
            meshFilter.mesh = new Mesh();
            mesh = meshFilter.sharedMesh;
        }

        //Clears the mesh and draws the updated mesh
        mesh.Clear();

        int tetraCount = renderTetras.Count();

        mesh.vertices = calcVertices(tetraCount);

        mesh.triangles = calcTriangles(tetraCount);

        mesh.uv = calcUvs(tetraCount);

        mesh.triangles = mesh.triangles.Reverse().ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshCollider.sharedMesh = mesh;
        meshCollider.convex = true;
    }
}
