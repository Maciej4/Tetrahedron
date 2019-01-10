using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvTest : MonoBehaviour
{
    public GlobController target;
    public Transform point1;
    public Transform point2;
    public Transform point3;

    public float ax;
    public float ay;
    public float az;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //point1 = target.vertices[1];
        //point1 = target.vertices[2];
        //point3 = target.vertices[3];

        float x = point2.localPosition.x - point1.localPosition.x;
        float y = point2.localPosition.y - point1.localPosition.y;
        float z = point2.localPosition.z - point1.localPosition.z;
        ax = -Mathf.Atan2(Mathf.Sqrt(y * y + z * z), x) * Mathf.Rad2Deg;
        az = Mathf.Atan2(Mathf.Sqrt(z * z + x * x), y) * Mathf.Rad2Deg;
        ay = Mathf.Atan2(Mathf.Sqrt(x * x + y * y), z) * Mathf.Rad2Deg;
        //transform.Rotate(ax * Mathf.Deg2Rad, ay * Mathf.Deg2Rad, az * Mathf.Deg2Rad);
        transform.localRotation = Quaternion.Euler(ax, ay, az);
        transform.localPosition = point1.localPosition;
        //transform.LookAt(target.point[2], Vector3.up);
    }
}
