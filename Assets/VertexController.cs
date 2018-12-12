using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexController : MonoBehaviour {
    public bool overideControl;
    public Vector3 overidePos = new Vector3(0f, 0f, 0f);
    public TetraController parentController;
    public int relName;
    private bool relNameFound;

	// Use this for initialization
	void Start () {
		
	}

    void findRelName() {
        for (int i = 0; i < 3; i++) {
            if (this.transform == parentController.transforms[i]) {
                relNameFound = true;
                relName = i;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (!overideControl) {
            transform.localPosition = parentController.point[relName];
        } else {
            transform.localPosition = overidePos;
        }
	}
}
