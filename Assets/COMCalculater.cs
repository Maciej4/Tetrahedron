using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class COMCalculater : MonoBehaviour {
	public GameObject alpha;
	public GameObject beta;
	public GameObject charlie;
	public GameObject delta;

	public float comX;
	public float comY;
	public float comZ;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		comX = (alpha.transform.position.x+beta.transform.position.x
		+charlie.transform.position.x+delta.transform.position.x)/4;
		comY = (alpha.transform.position.y+beta.transform.position.y
		+charlie.transform.position.y+delta.transform.position.y)/4;
		comZ = (alpha.transform.position.z+beta.transform.position.z
		+charlie.transform.position.z+delta.transform.position.z)/4;
	}
}
