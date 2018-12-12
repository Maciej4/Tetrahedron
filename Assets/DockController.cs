using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockController : MonoBehaviour {
    public Transform globPrefab;

	// Use this for initialization
	void Start () {
		
	}

    void makeGlob() {
        Transform go = Instantiate(globPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
        go.transform.parent = transform;
        go.tag = "Untagged";
        go.name = "Glob " + (this.GetComponentsInChildren<GlobController>().Length).ToString("000");
    }
	
	// Update is called once per frame
	void Update () {
        globPrefab = GameObject.FindGameObjectWithTag("MainTetrahedron").transform;

    }
}
