using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderController : MonoBehaviour
{
    public GlobController target;

    public Quaternion rot;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //float x = target.point[2].x;
        //float y = target.point[2].y;
        //float z = target.point[2].z;
        //float ax = Mathf.Atan2(Mathf.Sqrt(y * y + z * z), x) * Mathf.Rad2Deg; 
        //float ay = Mathf.Atan2(Mathf.Sqrt(z * z + x * x), y) * Mathf.Rad2Deg;
        //float az = Mathf.Atan2(Mathf.Sqrt(x * x + y * y), z) * Mathf.Rad2Deg;
        //rot = new Vector3(ax, ay, az);
        rot = Quaternion.LookRotation(target.point[2]);
        //rot.z -=
        //transform.rotation = 
        rot = transform.localRotation;
        transform.localPosition = target.point[0];
    }
}
