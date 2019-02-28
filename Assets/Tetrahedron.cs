using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetrahedron
{
    public Vertex[] vertices = new Vertex[4];
    public Side[] sides;
    public bool isOrigin;
    public bool inverted = true;
    public int type = 0;

    public Tetrahedron(Vertex vertex0, Vertex vertex1, Vertex vertex2, Vertex vertex3, int type_ = 0)
    {
        type = type_;

        vertices[0] = vertex0;
        vertices[1] = vertex1;
        vertices[2] = vertex2;
        vertices[3] = vertex3;

        if (type == 0)
        {
            sides = new Side[3];
            sides[0] = new Side(vertex0, vertex3);
            sides[1] = new Side(vertex1, vertex3);
            sides[2] = new Side(vertex2, vertex3);
        }
        else if (type == 1)
        {
            sides = new Side[6];
            sides[0] = new Side(vertex0, vertex1);
            sides[1] = new Side(vertex0, vertex2);
            sides[2] = new Side(vertex0, vertex3);
            sides[3] = new Side(vertex1, vertex2);
            sides[4] = new Side(vertex1, vertex3);
            sides[5] = new Side(vertex2, vertex3);
        }
        else if (type == 2)
        {

        }
    }

    public void loop()
    {
        if (type == 0)
        {
            vertices[3].pos = TetraUtil.trilaterate(
                vertices[0].pos, vertices[1].pos, vertices[2].pos,
                sides[0].length, sides[1].length, sides[2].length,
                inverted
            );
        }
        else if (type == 1)
        {
            Vector3[] outputVertices = TetraUtil.originVertices(
                 sides[0].length, sides[1].length, sides[2].length,
                 sides[3].length, sides[4].length, sides[5].length
            );

            vertices[0].pos = outputVertices[0];
            vertices[1].pos = outputVertices[1];
            vertices[2].pos = outputVertices[2];
            vertices[3].pos = outputVertices[3];
        }
    }
}
