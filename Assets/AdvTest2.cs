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

        float x = point1.localPosition.x - point0.localPosition.x;
        float y = point1.localPosition.y - point0.localPosition.y;
        float z = point1.localPosition.z - point0.localPosition.z;
        float ax = -Mathf.Atan2(Mathf.Sqrt(y * y + z * z), x) * Mathf.Rad2Deg;
        float az = Mathf.Atan2(Mathf.Sqrt(z * z + x * x), y) * Mathf.Rad2Deg;
        float ay = Mathf.Atan2(Mathf.Sqrt(x * x + y * y), z) * Mathf.Rad2Deg;

        float x_ = point3.localPosition.x - point2.localPosition.x;
        float y_ = point3.localPosition.y - point2.localPosition.y;
        float z_ = point3.localPosition.z - point2.localPosition.z;
        float ax_ = -Mathf.Atan2(Mathf.Sqrt(y_ * y_ + z_ * z_), x_) * Mathf.Rad2Deg;
        float az_ = Mathf.Atan2(Mathf.Sqrt(z_ * z_ + x_ * x_), y_) * Mathf.Rad2Deg;
        float ay_ = Mathf.Atan2(Mathf.Sqrt(x_ * x_ + y_ * y_), z_) * Mathf.Rad2Deg;

        //transform.Rotate(ax * Mathf.Deg2Rad, ay * Mathf.Deg2Rad, az * Mathf.Deg2Rad);
        transform.localRotation = Quaternion.Euler(ax, ay, az);
        transform.localPosition = point0.localPosition;
        //transform.LookAt(target.point[2], Vector3.up);
    }
}
