using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex
{
    public int ID { get; set; }
    //public Transform vertexTransform;
    public Vector3 pos = new Vector3(0.0f, 0.0f, 0.0f);

    public Vertex(int ID_)
    {
        ID = ID_;
    }
}
