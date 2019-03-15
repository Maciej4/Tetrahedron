using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public uiController ui;
    public InputController inc;
    private int boundary = 100; // distance from edge scrolling starts
    private int speed = 10;
    private int screenWidth;
    private int screenHeight;
    private int zoomSpeed = 3;
    private float zoomMin = 3f;
    private float zoomMax = 50f;
    public float trackingDistance = 5;
    public bool trackingActive = false;
    public bool edgeView = false;
    public List<TetraController> selection;
    public Transform goalPoint;
    private float optimalZ;
    public int globVertexId = -1;
    public Vector3 hitPos;
    public int inputID;
    public bool buildHover = false;
    public float heightOffsetY = 0f;
    private bool yawLeft = false;
    private bool yawRight = false;
    private bool pitchUp = false;
    private bool pitchDown = false;

    void Start()
    {
        ui = new uiController(this.transform);
        inc = new InputController(this);

        string controlsSum = "Controls:";
        int nameCount = 1;
        foreach (Toggler toggle in inc.togglers)
        {
            controlsSum += "\n" + toggle.name;
            nameCount++;
        }

        ui.controls = controlsSum;
        ui.controlsLength = nameCount;

        screenWidth = Screen.width;
        screenHeight = Screen.height;
        transform.rotation = Quaternion.LookRotation(new Vector3(0, -10, 10)); // Look from (0,0,0) towards this point
    }

    void Update()
    {
        if (ui == null)
        {
            Start();
        }

        goalPoint = GameObject.FindGameObjectWithTag("GoalPoint").transform;
        Vector3 newPosition = transform.position;
        float zoomAmount = zoomSpeed * Input.GetAxis("Mouse ScrollWheel");

        if (!trackingActive) //Not Tracking
        {
            // Screen scrolling
            if (Input.mousePosition.x < screenWidth && Input.mousePosition.x > screenWidth - boundary)
            {
                newPosition.x += speed * Time.deltaTime; // move on +X axis
            }
            if (Input.mousePosition.x > 0 && Input.mousePosition.x < 0 + boundary)
            {
                newPosition.x -= speed * Time.deltaTime; // move on -X axis
            }
            if (Input.mousePosition.y < screenHeight && Input.mousePosition.y > screenHeight - boundary)
            {
                newPosition.z += speed * Time.deltaTime; // move on +Z axis
            }
            if (Input.mousePosition.y > 0 && Input.mousePosition.y < 0 + boundary)
            {
                newPosition.z -= speed * Time.deltaTime; // move on -Z axis
            }

            //Zoom while untracked
            Vector3 zoomDelta;
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            zoomDelta = ray.direction.normalized * zoomAmount;

            if (newPosition.y + zoomDelta.y >= zoomMin && newPosition.y + zoomDelta.y <= zoomMax)
            {
                newPosition += zoomDelta;
            }
        }
        else if (trackingActive && selection.Count > 0) //Tracking Tetrahedron(s)
        {
            //Movment while tracked
            Vector3 trackingCenter = TetraUtil.averageSelection(selection);

            newPosition.x = trackingCenter.x;
            if (trackingCenter.y > 1f)
            {
                heightOffsetY = trackingCenter.y;
            }
            else
            {
                heightOffsetY = 0f;
            }
            optimalZ = trackingCenter.z;

            //Zooming while tracked
            trackingDistance -= zoomAmount / 2;
            trackingDistance = Mathf.Clamp(trackingDistance, zoomMin, zoomMax);
            newPosition.y = trackingDistance + heightOffsetY;
            newPosition.z = optimalZ - trackingDistance;

            //Rotate selected tetrahedron
            if (buildHover && selection.Count == 1)
            {
                if (yawLeft)
                {
                    selection[0].setYawRate(1.0f);
                }
                else if (yawRight)
                {
                    selection[0].setYawRate(-1.0f);
                }
                else
                {
                    selection[0].setYawRate(0.0f);
                }

                if (pitchUp)
                {
                    selection[0].setPitchRate(1.0f);
                }
                else if (pitchDown)
                {
                    selection[0].setPitchRate(-1.0f);
                }
                else
                {
                    selection[0].setPitchRate(0.0f);
                }

                yawLeft = false;
                yawRight = false;
                pitchUp = false;
                pitchDown = false;
            }
        }



        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            inputID = 0;
        }





        //Unit Selection
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    if (!hit.transform.CompareTag("Terrain"))
                    {
                        bool copyDetected = false;

                        for (int i = 0; i < selection.Count; i++) {
                            if (hit.transform == selection[i]) {
                                copyDetected = true;
                            }
                        }

                        if (!copyDetected)
                        {
                            selection.Add(hit.transform.GetComponent<TetraController>());
                        }
                    }
                }
                else if (!hit.transform.CompareTag("Terrain") && Input.GetKey(KeyCode.LeftControl) && inputID < 4)
                {
                    globVertexId = hit.transform.GetComponent<TetraController>().closestVertex(hit.point);
                    hitPos = hit.point;
                    hit.transform.GetComponent<TetraController>().newTetraVtx[inputID] = globVertexId;
                    inputID++;
                    Debug.Log("Detected vertex: " + globVertexId + " added on ID: " + inputID);
                }
                else
                {
                    if (!hit.transform.CompareTag("Terrain"))
                    {
                        selection.Clear();
                        selection.Add(hit.transform.GetComponent<TetraController>());
                    }
                    else
                    {
                        if (buildHover)
                        {
                            toggleHover_();
                        }
                        selection.Clear();
                        trackingActive = false;
                        globVertexId = -1;
                    }
                }
            }
        }

        //Unit Movement Command
        if (Input.GetMouseButtonDown(1) && (selection.Count > 0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.transform.CompareTag("Terrain"))
                {
                    goalPoint.position = hit.point;

                    for (int i = 0; i < selection.Count; i++)
                    {
                        selection[i].sw.startContinuousWalk(goalPoint);
                    }
                }
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, newPosition, 0.5f);
    }
    
    void OnGUI()
    {
        //Display tracking and selected tetrahedrons
        if (selection.Count == 1)
        {
            ui.singleTracking(trackingActive, selection[0].name);

            if (trackingActive)
            {
                if (edgeView)
                {
                    foreach (Tetrahedron tetrahedron in selection[0].tetrahedrons)
                    {
                        if (tetrahedron.type != 2)
                        {
                            foreach (Side side in tetrahedron.sides)
                            {
                                Vector3 finalPos = TetraUtil.averageVertices(side.vertices, selection[0].transform);

                                ui.numberAtPos(finalPos, side.ID, false, "s");
                            }
                        }
                    }
                }
                else
                {
                    foreach (Vertex vertex in selection[0].vertices)
                    {
                        Vector3 tempPos = selection[0].transform.TransformPoint(vertex.pos);

                        ui.numberAtPos(tempPos, vertex.ID, selection[0].newTetraVtx.Contains(vertex.ID));
                    }
                }
            }
        }
        else if (selection.Count > 1)
        {
            ui.multipleTracking(trackingActive, selection.Count);
        }

        ui.loop();

        inc.loop();
    }

    public bool oneSelected(bool showAlert = true)
    {
        if (selection.Count == 1)
        {
            return true;
        }
        else if (selection.Count == 0 && showAlert)
        {
            ui.addCenterAlert("No tetrahedron selected!");
        }
        else if (showAlert)
        {
            ui.addCenterAlert("Multiple tetrahedrons selected!");
        }
        return false;
    }

    public void addTetraFromList_()
    {
        if (oneSelected() && selection[0].newTetraVtx[2] != -1)
        {
            selection[0].addTetraFromList();
            ui.addCenterAlert("You added a tetrahedron!");
        }
    }

    public void addSpecialFromList_()
    {
        if (oneSelected() && selection[0].newTetraVtx[2] != -1)
        {
            selection[0].addSpecialFromList();
            ui.addCenterAlert("You added a special tetrahedron!");
        }
    }

    public void invertLastTetra_()
    {
        if (oneSelected())
        {
            selection[0].invertLastTetra();
            ui.addCenterAlert("You inverted the last tetrahedron!");
        }
    }

    public void resetVertices()
    {
        inputID = 0;
    }

    public void toggleTracking()
    {
        if (selection.Count > 0)
        {
            if (buildHover)
            {
                toggleHover_();
            }
            trackingActive = !trackingActive;
            trackingDistance = transform.position.y;
            edgeView = false;
        }
    }

    public void toggleEdgeView()
    {
        if (oneSelected())
        {
            edgeView = !edgeView;
            ui.addCenterAlert("You toggled edge view!");
        }
    }

    public void printVertices_()
    { 
        if (oneSelected())
        {
            selection[0].printVertices();
            ui.addCenterAlert("Printed vertices of " + selection[0].name);
        }
    }

    public void removeLastTetra_()
    { 
        if (oneSelected())
        {
            selection[0].removeLastTetra();
            ui.addCenterAlert("Removed last tetrahedron");
        }
    }

    public void toggleHover_()
    { 
        if (oneSelected() && trackingActive)
        {
            buildHover = !buildHover;
            selection[0].toggleHover();
            ui.addCenterAlert("Toggled hover build mode");
        }
    }

    public void increaseYaw_()
    { 
        if (oneSelected() && buildHover)
        {
            yawLeft = true;
        }
    }

    public void decreaseYaw_()
    {
        if (oneSelected() && buildHover)
        {
            yawRight = true;
        }
    }

    public void increasePitch_()
    {
        if (oneSelected() && buildHover)
        {
            pitchUp = true;
        }
    }

    public void decreasePitch_()
    {
        if (oneSelected() && buildHover)
        {
            pitchDown = true;
        }
    }
}
