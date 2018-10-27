using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineScript : MonoBehaviour {
	private LineRenderer lineRenderer;

	public Transform first;
	public Transform second;
	public Transform third;
	public Transform fourth;

	// Use this for initialization
	void Start () {
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.SetPosition(0, first.position);
	}

	// Update is called once per frame
	void Update () {
		lineRenderer.SetPosition(0, first.position);
		lineRenderer.SetPosition(1, second.position);
		lineRenderer.SetPosition(2, third.position);
		lineRenderer.SetPosition(3, first.position);
		lineRenderer.SetPosition(4, fourth.position);
		lineRenderer.SetPosition(5, third.position);
		lineRenderer.SetPosition(6, second.position);
		lineRenderer.SetPosition(7, fourth.position);
	}
}
