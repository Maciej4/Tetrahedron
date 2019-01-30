using System;
using UnityEngine;

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
