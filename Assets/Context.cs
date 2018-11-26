using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Context : MonoBehaviour {
    public Transform[] transforms;
    public MeshRenderer[] renderers;
    public BasicWalk basicWalk;
    public PointControl pointControl;
    public SidePicker sidePicker;


    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        transforms = this.gameObject.GetComponentsInChildren<Transform>();
        renderers = this.gameObject.GetComponentsInChildren<MeshRenderer>();
        basicWalk = this.gameObject.GetComponent<BasicWalk>();
        pointControl = this.gameObject.GetComponent<PointControl>();
        sidePicker = this.gameObject.GetComponent<SidePicker>();
    }
}
