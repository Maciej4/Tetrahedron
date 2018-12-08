using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmController : MonoBehaviour {
    public WalkController[] walkScripts;
    public Transform goalPoint;
    public Transform prefab;

    public bool startMove = false;
    private bool swarmMove = false;
    private bool finished = true;
    public int tetrobotsToMake = 0;
    public bool makeQueued = false;
    public int tetrahedronCount = 0;
    public bool armDestroyAll = false;
    public bool destroyAll = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        prefab = GameObject.FindGameObjectWithTag("MainTetrahedron").transform;
        walkScripts = this.gameObject.GetComponentsInChildren<WalkController>();
        if (startMove) {
            startMove = false;
            swarmMove = true;
            for (int i = 0; i < walkScripts.Length; i++) {
                walkScripts[i].startWalk(goalPoint);
            }
        }

        if (swarmMove) {
            finished = true;
            for (int i = 0; i < walkScripts.Length; i++) {
                if (walkScripts[i].walking) {
                    finished = false;
                }
            }

            if (finished) {
                swarmMove = false;
            }
        }

        tetrahedronCount = this.gameObject.GetComponent<Transform>().childCount;

        if (makeQueued) {
            for (int i = 0; i < tetrobotsToMake; i++) {
                float x = Random.value * 50.0f - 25.0f;
                float y = 5.0f;
                float z = Random.value * 50.0f - 25.0f;
                Transform go = Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity);
                go.transform.parent = transform;
                go.tag = "Untagged";
                go.name = "Tetrahedron "+ (i+tetrahedronCount).ToString("000");
            }
            tetrobotsToMake = 0;
            makeQueued = false;
        }

        if (destroyAll && !armDestroyAll) {
            destroyAll = false;
        }

        if (armDestroyAll && destroyAll) {
            destroyAll = false;
            armDestroyAll = false;
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
	}
}
