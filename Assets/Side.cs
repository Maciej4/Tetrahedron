using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Side
{
    public HashSet<Vertex> vertices = new HashSet<Vertex>();

    public float length = 2.0f;
    public int ID = 0;

    public Side(Vertex firstVertex, Vertex secondVertex) 
    {
        vertices.Add(firstVertex);
        vertices.Add(secondVertex);
    }
}
