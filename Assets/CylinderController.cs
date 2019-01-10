using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderController : MonoBehaviour
{
    public GlobController target;

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
        float x = target.point[2].x;
        float y = target.point[2].y;
        float z = target.point[2].z;
        ax = -Mathf.Atan2(Mathf.Sqrt(y * y + z * z), x) * Mathf.Rad2Deg; 
        az = Mathf.Atan2(Mathf.Sqrt(z * z + x * x), y) * Mathf.Rad2Deg;
        ay = Mathf.Atan2(Mathf.Sqrt(x * x + y * y), z) * Mathf.Rad2Deg;
        //transform.Rotate(ax * Mathf.Deg2Rad, ay * Mathf.Deg2Rad, az * Mathf.Deg2Rad);
        transform.localRotation = Quaternion.Euler(ax, ay, az);
        transform.localPosition = target.point[0];
        //transform.LookAt(target.point[2], Vector3.up);
    }
}
