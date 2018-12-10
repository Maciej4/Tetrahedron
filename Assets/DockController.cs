using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockController : MonoBehaviour {
    public TetraController[] tetraControllers;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        tetraControllers = this.gameObject.GetComponentsInChildren<TetraController>();

        for (int i = 0; i < tetraControllers.Length; i++)
        {
            if (Vector3.Distance(tetraControllers[i].centerMass, tetraControllers[i].connectTarget.centerMass) < 2.0f)
            {

            }
        }
    }
}
