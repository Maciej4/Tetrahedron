using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLock : MonoBehaviour {
	public GameObject alpha;
	public GameObject beta;
	public GameObject charlie;
	public GameObject delta;

	public float comX;
	public float comY;
	public float comZ;

  void Start() {

  }

	// Update is called once per frame
	void Update () {
		comX = (alpha.transform.position.x+beta.transform.position.x
		+charlie.transform.position.x+delta.transform.position.x)/4;
		comY = (alpha.transform.position.y+beta.transform.position.y
		+charlie.transform.position.y+delta.transform.position.y)/4;
		comZ = (alpha.transform.position.z+beta.transform.position.z
		+charlie.transform.position.z+delta.transform.position.z)/4;

		transform.position = new Vector3(comX+0.3f, comY+0.3f, comZ+0.3f);
		transform.eulerAngles = new Vector3(30, 225, 0);
	}
}
