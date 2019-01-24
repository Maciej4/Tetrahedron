using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex : MonoBehaviour
{
    public Transform vertexTransform;
    public Vector3 vertexPos;

    public Vertex(Transform _vertexTransform)
    {
        vertexTransform = _vertexTransform;
    }
}
