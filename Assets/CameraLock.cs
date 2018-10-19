using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLock : MonoBehaviour {
	//public GameObject target;
	//public GameObject target;
	//private Script other;
	public GameObject g;

	COMCalculater other = g.GetComponent<COMCalculater>();

  void Start() {
		//other = GameObject.GetComponent("COMCalculator");
  }

	// Update is called once per frame
	void Update () {
		transform.position = new Vector3(other.comX()-10, other.comY()+10, other.comZ()-10);
		transform.eulerAngles = new Vector3(30, -45, 0);
	}
}
