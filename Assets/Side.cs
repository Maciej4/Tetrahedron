using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Side : MonoBehaviour
{
    public HashSet<Vertex> vertices;

    public float length = 2.0f;

    public Side(Vertex firstVertex, Vertex secondVertex) 
    {
        vertices.Add(firstVertex);
        vertices.Add(secondVertex);
    }
}
