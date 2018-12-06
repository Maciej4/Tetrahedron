using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmController : MonoBehaviour {
    public NewWalk[] walkScripts;

    public bool startMove = false;
    public bool swarmMove = false;
    public bool finished = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        walkScripts = this.gameObject.GetComponentsInChildren<NewWalk>();
        if (startMove) {
            startMove = false;
            swarmMove = true;
            for (int i = 0; i < walkScripts.Length; i++) {
                walkScripts[i].startWalk = true;
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
	}
}
