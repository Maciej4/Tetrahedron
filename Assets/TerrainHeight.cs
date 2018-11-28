using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainHeight : MonoBehaviour {
    private float x;
    private float y;
    private float z;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        x = transform.position.x;
        y = transform.position.y;
        z = transform.position.z;
        pos.y = Terrain.activeTerrain.SampleHeight(new Vector3(x, y-0.25f, z))+0.25f;
        transform.position = pos;
    }
}
