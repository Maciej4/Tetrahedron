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
public class TetraController : MonoBehaviour
{
    //Variable and object declarations
    private TetraRenderer tr;
    public SimpleWalkController sw;
    public HashSet<Vertex> vertices = new HashSet<Vertex>();
    public HashSet<Side> sides = new HashSet<Side>();
    public List<Tetrahedron> tetrahedrons = new List<Tetrahedron>();
    public List<float> sideSetList = new List<float>();
    public List<int[]> vertexList = new List<int[]>();
    public List<int> newTetraVtx = new List<int>();
    public bool colorPicked = false;
    public bool walkControl = false;
    public bool runSetup = true;
    public Vector3 centerMass;
    public bool runOppositeSide = false;
    public int sideVal = 0;

    //Runs once at start of program
    void Start()
    {
        colorPicked = false;
        runSetup = true;
        tr = new TetraRenderer(this.gameObject);
        sw = new SimpleWalkController(this.gameObject);

        vertexList.Add(new int[5] { 0, 1, 2, 3, 0 });
        //vertexList.Add(new int[4] { 2, 1, 0, 4 });
        //vertexList.Add(new int[4] { 1, 2, 3, 5 });
        newTetraVtx[0] = 2;
        newTetraVtx[1] = 1;
        newTetraVtx[2] = 0;
        newTetraVtx[3] = 0;
    }

    public int closestVertex(Vector3 hitPos)
    {
        int closestID = 0;
        float closestDist = 10.0f;

        foreach (Tetrahedron tetrahedron in tetrahedrons)
        {
            foreach (Vertex vertex in tetrahedron.vertices) {
                Vector3 absolutePos = transform.TransformPoint(vertex.pos);
                float distance = Vector3.Distance(absolutePos, hitPos);

                if (distance < closestDist)
                {
                    closestID = vertex.ID;
                    closestDist = distance;
                }
            }
        }

        return closestID;
    }

    public int maxVertex()
    {
        int maxVertexID = 0;

        for (int o = 0; o < vertexList.Count; o++)
        {
            for (int l = 0; l < 4; l++)
            {
                if (vertexList[o][l] > maxVertexID)
                {
                    maxVertexID = vertexList[o][l];
                }
            }
        }

        maxVertexID++;

        return maxVertexID;
    }

    public void clearNewTetraList()
    {
        newTetraVtx[0] = -1;
        newTetraVtx[1] = -1;
        newTetraVtx[2] = -1;
        newTetraVtx[3] = -1;
    }

    public void addTetra(int a, int b, int c)
    {
        vertexList.Add(new int[5] { a, b, c, maxVertex() + 1, 0 });

        setupTetras();
    }

    public void addTetraFromList()
    {
        addTetra(newTetraVtx[0], newTetraVtx[1], newTetraVtx[2]);
        clearNewTetraList();
    }

    public void addSpecial(int a, int b, int c, int d)
    {
        vertexList.Add(new int[5] { a, b, c, d, 2 });

        setupTetras();
    }

    public void addSpecialFromList()
    {
        addSpecial(newTetraVtx[0], newTetraVtx[1], newTetraVtx[2], newTetraVtx[3]);
        clearNewTetraList();
    }

    public void invertLastTetra()
    {
        tetrahedrons[tetrahedrons.Count - 1].inverted = !tetrahedrons[tetrahedrons.Count - 1].inverted;
    }

    public void upendLastTetra()
    {
        Vertex tempVertex = tetrahedrons[tetrahedrons.Count - 1].vertices[0];
        tetrahedrons[tetrahedrons.Count - 1].vertices[0] = tetrahedrons[tetrahedrons.Count - 1].vertices[2];
        tetrahedrons[tetrahedrons.Count - 1].vertices[2] = tempVertex;
    }

    public Side oppositeSide(Tetrahedron tetrahedron, Side side)
    {
        HashSet<Vertex> x = side.vertices;
        HashSet<Vertex> y = new HashSet<Vertex>();
        foreach (Vertex v in tetrahedron.vertices)
        {
            y.Add(v);
        }

        HashSet<Vertex> com = new HashSet<Vertex>(y.Except(x));

        if (com.Count == 2)
        { 
            foreach (Tetrahedron t in tetrahedrons)
            { 
                foreach (Side s in t.sides)
                {
                    HashSet<Vertex> tempX = s.vertices;
                    tempX.IntersectWith(com);

                    if (tempX.Count == 2)
                    {
                        return s;
                    }
                }
            }
        }
        Debug.Log("Failure to find opposite side");
        return side;

    }

    //Run to update list of tetrahedrons
    public void setupTetras() 
    {
        Debug.Log("---------Vertices---------");

        vertices.Clear();
        tetrahedrons.Clear();
        tr.renderTetras.Clear();

        int maxVertexCount = maxVertex();

        Debug.Log("max vertex count: " + maxVertexCount);

        Vertex[] tempVertices = new Vertex[maxVertexCount];

        for (int i = 0; i < maxVertexCount; i++)
        {
            Vertex tempVertex = new Vertex(i);
            Debug.Log("Vertex: " + tempVertex + "; ID: " + tempVertex.ID + "; Pos: " + tempVertex.pos);
            tempVertices[i] = tempVertex;
            vertices.Add(tempVertex);
        }

        TetraUtil.isNull(tempVertices);

        tetrahedrons.Add(new Tetrahedron(tempVertices[0], tempVertices[1], tempVertices[2], tempVertices[3], 1));

        for (int j = 1; j < vertexList.Count; j++)
        {
            tetrahedrons.Add(new Tetrahedron(
                tempVertices[vertexList[j][0]],
                tempVertices[vertexList[j][1]],
                tempVertices[vertexList[j][2]],
                tempVertices[vertexList[j][3]],
                vertexList[j][4]
            ));
        }

        Debug.Log(tetrahedrons.Count + " tetrahedron(s)");

        foreach (Tetrahedron tetra in tetrahedrons)
        {
            tr.renderTetras.Add(tetra);
        }

        Debug.Log(tr.renderTetras.Count + " render tetrahedron(s)");
    }

    //Physics update, runs repeatedly
    void FixedUpdate()
    {
        //Sets center of mass
        centerMass = TetraUtil.averageVertices(vertices, this.transform);

        if(tr == null)
        {
            Start();
        }

        //Tells tetraRenderer to pick color
        if (!colorPicked)
        {
            colorPicked = true;
            tr.pickRandomColor();
        }

        //Runs setup of tetrahedrons
        if (runSetup)
        {
            runSetup = false;
            setupTetras();
        }

        //Sets side lengths in tetrahedrons
        int p = 0;

        foreach (Tetrahedron tetrahedron in tetrahedrons)
        {
            if (tetrahedron.type != 2)
            {
                foreach (Side side in tetrahedron.sides)
                {
                    if (sideSetList.Count < p + 1)
                    {
                        sideSetList.Add(2.0f);
                    }

                    side.length = Mathf.SmoothStep(side.length, sideSetList[p], 0.10f);
                    side.ID = p;

                    if (runOppositeSide && p == sideVal)
                    {
                        runOppositeSide = false;
                        Debug.Log("----------");
                        int oppositeID = oppositeSide(tetrahedron, side).ID;
                        Debug.Log("Opposite side is: " + oppositeID);
                    }

                    p++;
                }
            }
        }

        //Runs loop of tetrahedrons
        foreach(Tetrahedron tetra in tetrahedrons)
        {
            tetra.loop();
        }

        //Catches if tetraRenderer does not exist
        if (tr == null)
        {
            tr = new TetraRenderer(this.gameObject);
        }

        //Loops tetraRenderer
        tr.loop();

        if (tetrahedrons.Count == 1 && walkControl)
        {
            sw.loop();
        }
    }
}