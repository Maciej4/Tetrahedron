using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainHeight : MonoBehaviour {
    public float y;
    public float height;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 pos = transform.position;
        y = transform.position.y;
        height = Terrain.activeTerrain.SampleHeight(transform.position);
        pos.y = height;
        if (y <= pos.y+0.5f) {
            pos.y += 0.01f;
            transform.position = pos;
        }
    }
}
