using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvTest2 : MonoBehaviour
{
    private TetraMath tm = new TetraMath();
    public Transform point0;
    public Transform point1;
    public Transform point2;
    public Transform point3;
    public Vector3 posTarget;
    public float[] sideLengths = new float[3] {2f, 2f, 2f};

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

    // Update is called once per frame
    void Update()
    {
        posTarget = tm.trilaterate(point0.localPosition, point1.localPosition, point2.localPosition, sideLengths[0], sideLengths[1], sideLengths[2]);
        
        transform.localPosition = posTarget;
    }
}
