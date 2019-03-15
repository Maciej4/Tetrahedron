using System;
using System.Collections.Generic;
using UnityEngine;

public static class TetraUtil
{

    public static Vector3[] originVertices(float AsB_m, float AsC_m, float AsD_m, float BsC_m, float BsD_m, float CsD_m)
    {
        float AsB_m2 = AsB_m * AsB_m;
        float AsC_m2 = AsC_m * AsC_m;
        float AsD_m2 = AsD_m * AsD_m;
        float BsC_m2 = BsC_m * BsC_m;
        float BsD_m2 = BsD_m * BsD_m;
        float CsD_m2 = CsD_m * CsD_m;
        float qx = AsB_m;
        float rx = (AsB_m2 + AsC_m2 - BsC_m2) / (2.0f * AsB_m);
        float ry = Mathf.Sqrt(AsC_m2 - rx * rx);
        float sx = (AsB_m2 + AsD_m2 - BsD_m2) / (2.0f * AsB_m);
        float sy = (BsD_m2 - (sx - qx) * (sx - qx) - CsD_m2 + (sx - rx) * (sx - rx) + ry * ry) / (2 * ry);
        float sz = Mathf.Sqrt(AsD_m2 - sx * sx - sy * sy);
        Vector3[] output = new Vector3[4];
        output[0] = new Vector3(0.0f, 0.0f, 0.0f);
        output[1] = new Vector3(qx, 0.0f, 0.0f);
        output[2] = new Vector3(rx, ry, 0.0f);
        output[3] = new Vector3(sx, sy, sz);
        return output;
    }

    public static Vector3 trilaterate(Vector3 P1, Vector3 P2, Vector3 P3, float r1, float r2, float r3, bool inverse = false)
    {
        Vector3 temp1 = P2 - P1;
        Vector3 e_x = temp1 / Vector3.Magnitude(temp1);
        Vector3 temp2 = P3 - P1;
        float i = Vector3.Dot(e_x, temp2);
        Vector3 temp3 = temp2 - i * e_x;
        Vector3 e_y = temp3 / Vector3.Magnitude(temp3);
        Vector3 e_z = Vector3.Cross(e_x, e_y);
        float d = Vector3.Magnitude(P2 - P1);
        float j = Vector3.Dot(e_y, temp2);
        float x = (r1 * r1 - r2 * r2 + d * d) / (2 * d);
        float y = (r1 * r1 - r3 * r3 - 2 * i * x + i * i + j * j) / (2 * j);
        float temp4 = r1 * r1 - x * x - y * y;
        //if (temp4 < 0)
        float z = Mathf.Sqrt(temp4);

        if(inverse)
        {
            Vector3 p_12_a = P1 + x * e_x + y * e_y + z * e_z;
            return (p_12_a);
        }
        else
        {
            Vector3 p_12_b = P1 + x * e_x + y * e_y - z * e_z;
            return (p_12_b);
        }
    }

    public static void isNull(object target)
    {
        if (target == null)
        {
            Debug.Log(target.GetType() + " does not exist");
        }
    }

    public static Vector3 averageVertices(HashSet<Vertex> vertices)
    {
        float totalX = 0f;
        float totalY = 0f;
        float totalZ = 0f;
        int e = 0;

        foreach (Vertex vertex in vertices)
        {
            Vector3 tempPos;

            tempPos = vertex.pos;

            totalX += tempPos.x;
            totalY += tempPos.y;
            totalZ += tempPos.z;

            e++;
        }

        Vector3 finalPos = new Vector3();

        finalPos.x = totalX / e;
        finalPos.y = totalY / e;
        finalPos.z = totalZ / e;

        return finalPos;
    }

    public static Vector3 averageVertices(HashSet<Vertex> vertices, Transform targetTransform)
    {
        float totalX = 0f;
        float totalY = 0f;
        float totalZ = 0f;
        int e = 0;

        foreach (Vertex vertex in vertices)
        {
            Vector3 tempPos = vertex.pos;

            totalX += tempPos.x;
            totalY += tempPos.y;
            totalZ += tempPos.z;

            e++;
        }

        Vector3 finalPos = new Vector3();

        finalPos.x = totalX / e;
        finalPos.y = totalY / e;
        finalPos.z = totalZ / e;

        return targetTransform.transform.TransformPoint(finalPos);
    }

    public static Vector3 averageSelection(List<TetraController> selection)
    {
        float totalX = 0f;
        float totalY = 0f;
        float totalZ = 0f;

        foreach (TetraController tetraCtrl in selection)
        {
            Vector3 tempPos = tetraCtrl.centerMass;

            totalX += tempPos.x;
            totalY += tempPos.y;
            totalZ += tempPos.z;
        }

        Vector3 result = new Vector3();

        result.x = totalX / (float)selection.Count;
        result.y = totalY / (float)selection.Count;
        result.z = totalZ / (float)selection.Count;

        return result;
    }

    public static HashSet<int> vertexIDs(HashSet<Vertex> vertices)
    {
        HashSet<int> result = new HashSet<int>();

        foreach (Vertex vertex in vertices)
        {
            result.Add(vertex.ID);
        }

        return result;
    }

    public static HashSet<int> vertexIDs(Vertex[] vertices)
    {
        HashSet<int> result = new HashSet<int>();

        foreach (Vertex vertex in vertices)
        {
            result.Add(vertex.ID);
        }

        return result;
    }
}
