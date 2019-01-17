using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvTest2 : MonoBehaviour
{
    public GlobController target;
    public Transform point0;
    public Transform point1;
    public Transform point2;
    public Transform point3;
    public Vector3 posTarget;

    // Start is called before the first frame update
    void Start()
    {

    }

    public Vector3 calcAngle(Vector3 target0, Vector3 target1)
    {
        float x = target1.x - target0.x;
        float y = target1.y - target0.y;
        float z = target1.z - target0.z;
        float ax = -Mathf.Atan2(Mathf.Sqrt(y * y + z * z), x) * Mathf.Rad2Deg;
        float az = Mathf.Atan2(Mathf.Sqrt(z * z + x * x), y) * Mathf.Rad2Deg;
        float ay = Mathf.Atan2(Mathf.Sqrt(x * x + y * y), z) * Mathf.Rad2Deg;

        return new Vector3(ax, ay, az);
    }

    Vector3 trilaterate(Vector3 P1, Vector3 P2, Vector3 P3, float r1, float r2, float r3)
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
        Vector3 p_12_a = P1 + x * e_x + y * e_y + z * e_z;
        Vector3 p_12_b = P1 + x * e_x + y * e_y - z * e_z;
        //return (p_12_a); 
        //return Vector3.zero;
        return (p_12_b);
    }

    // Update is called once per frame
    void Update()
    {
        //point1 = target.vertices[1];
        //point1 = target.vertices[2];
        //point3 = target.vertices[3];
        //rot0 = calcAngle(point0.localPosition, point1.localPosition);

        //rot1 = calcAngle(point2.localPosition, point3.localPosition);

        //transform.Rotate(ax * Mathf.Deg2Rad, ay * Mathf.Deg2Rad, az * Mathf.Deg2Rad);
        //rot2 = rot0 + rot1;

        //rot2 = Vector3.ProjectOnPlane((point1.localPosition - point2.localPosition), point3.localPosition);

        //transform.localRotation = Quaternion.Euler(rot1);
        //Vector3 rot4 = new Vector3(0.0f, 0.0f, 0.0f);
        //transform.rotation = Quaternion.Euler(rot4);
        posTarget = trilaterate(point0.localPosition, point1.localPosition, point2.localPosition, 2f, 2f, 2f);
        //transform.localPosition = 2f*Vector3.up;
        transform.localPosition = posTarget;
        //transform.LookAt(target.point[2], Vector3.up);
    }
}
