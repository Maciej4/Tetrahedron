using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewTetrahedron
{
    public Vertex[] vertices = new Vertex[4];
    public Side[] sides;
    public bool isOrigin;

    public NewTetrahedron(Vertex vertex0, Vertex vertex1, Vertex vertex2, Vertex vertex3, bool isOrigin_ = false)
    {
        isOrigin = isOrigin_;

        vertices[0] = vertex0;
        vertices[1] = vertex1;
        vertices[2] = vertex2;
        vertices[3] = vertex3;

        if (!isOrigin)
        {
            sides = new Side[3];
            sides[0] = new Side(vertex0, vertex3);
            sides[1] = new Side(vertex1, vertex3);
            sides[2] = new Side(vertex2, vertex3);
        }
        else
        {
            sides = new Side[6];
            sides[0] = new Side(vertex0, vertex1);
            sides[1] = new Side(vertex0, vertex2);
            sides[2] = new Side(vertex0, vertex3);
            sides[3] = new Side(vertex1, vertex2);
            sides[4] = new Side(vertex1, vertex3);
            sides[5] = new Side(vertex2, vertex3);
        }
    }

    public void loop()
    {
        if (!isOrigin)
        {
            vertices[3].pos = TetraUtil.trilaterate(
                vertices[0].pos, vertices[1].pos, vertices[2].pos,
                sides[0].length, sides[1].length, sides[2].length,
                true
            );
        }
        else
        {
            Tetrahedron t = TetraUtil.originVertices(
                 sides[0].length, sides[1].length, sides[2].length, 
                 sides[3].length, sides[4].length, sides[5].length
            );

            vertices[0].pos = t.A;
            vertices[1].pos = t.B;
            vertices[2].pos = t.C;
            vertices[3].pos = t.D;
        }
    }
}
