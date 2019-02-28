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
    private int zoomMin = 3;
    private float trackingDistance = 5;
    public bool trackingActive = false;
    public bool edgeView = false;
    public List<Transform> selection;
    public Transform goalPoint;
    private float optimalZ;
    public int globVertexId = -1;
    public Vector3 hitPos;
    public int inputID;

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
        transform.rotation = Quaternion.LookRotation(new Vector3(0,-10,10)); // Look from (0,0,0) towards this point
    }

    void Update()
    {
        if (ui == null)
        {
            Start();
        }

        goalPoint = GameObject.FindGameObjectWithTag("GoalPoint").transform;
        Vector3 newPosition = transform.position;

        // Scroll
        if (Input.mousePosition.x < screenWidth && Input.mousePosition.x > screenWidth - boundary && !trackingActive)
        {
            newPosition.x += speed * Time.deltaTime; // move on +X axis
        }
        if (Input.mousePosition.x > 0 && Input.mousePosition.x < 0 + boundary && !trackingActive)
        {
            newPosition.x -= speed * Time.deltaTime; // move on -X axis
        }
        if (Input.mousePosition.y < screenHeight && Input.mousePosition.y > screenHeight - boundary && !trackingActive)
        {
            newPosition.z += speed * Time.deltaTime; // move on +Z axis
        }
        if (Input.mousePosition.y > 0 && Input.mousePosition.y < 0 + boundary && !trackingActive)
        {
            newPosition.z -= speed * Time.deltaTime; // move on -Z axis
        }

        //Tracking
        if (trackingActive && selection.Count != 0 && selection != null)
        {
            float totalAverageX = 0.0f;
            float totalAverageZ = 0.0f;

            for (int i = 0; i < selection.Count; i++)
            {
                if (selection[i].GetComponent<TetraController>() != null)
                {
                    totalAverageX += selection[i].GetComponent<TetraController>().centerMass.x;
                    totalAverageZ += selection[i].GetComponent<TetraController>().centerMass.z;
                }
            }

            newPosition.x = totalAverageX / (float)selection.Count;
            optimalZ = totalAverageZ / (float)selection.Count;
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            inputID = 0;
        }

        //Zoom
        float zoomAmount = zoomSpeed * Input.GetAxis("Mouse ScrollWheel");
        if (zoomAmount < 0 || transform.position.y > zoomMin)
        {
            Vector3 zoomDelta;

            if (!trackingActive)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                zoomDelta = ray.direction.normalized * zoomAmount;
                newPosition += zoomDelta;
            }
            
        }
        else
        {
            newPosition.y = zoomMin;
        }

        if (trackingActive)
        {
            trackingDistance -= zoomAmount / 2;
            trackingDistance = Mathf.Clamp(Mathf.Clamp(trackingDistance, 3, 50), transform.position.y - 3, transform.position.y + 3);
            newPosition.y = trackingDistance;
            newPosition.z = optimalZ - trackingDistance;
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
                            selection.Add(hit.transform);
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
                        selection.Add(hit.transform);
                    }
                    else
                    {
                        selection.Clear();
                        trackingActive = false;
                        globVertexId = -1;
                    }
                }
            }
        }

        //Placment of waypoint
        //if (selectedTetrahedrons.Count > 0)
        //{
        //    if (selectedTetrahedrons.Count == 1 && (selectedTetrahedrons[0].GetComponent<GlobController>().sw != null))
        //    {
        //        goalPoint.position = selectedTetrahedrons[0].GetComponent<GlobController>().sw.goalPos;
        //    }
        //    else if (selectedTetrahedrons[0].GetComponent<GlobController>().sw != null)
        //    {
        //        bool allGoalsSame = true;
        //        Vector3 refercnceGoal = selectedTetrahedrons[0].GetComponent<GlobController>().sw.goalPos;

        //        for (int i = 1; i < selectedTetrahedrons.Count; i++)
        //        {
        //            if (selectedTetrahedrons[0].GetComponent<GlobController>().sw != null)
        //            {
        //                if (!(selectedTetrahedrons[i].GetComponent<GlobController>().sw.goalPos == refercnceGoal))
        //                {
        //                    allGoalsSame = false;
        //                }
        //            }
        //        }

        //        if (allGoalsSame)
        //        {
        //            goalPoint.position = refercnceGoal;
        //        }
        //        else
        //        {
        //            goalPoint.position = new Vector3(230.0f, -0.5f, 230.0f);
        //        }
        //    }
        //}
        //else
        //{
        //    goalPoint.position = new Vector3(230.0f, -0.5f, 230.0f);
        //}

        //Unit Movement Command
        if (Input.GetMouseButtonDown(1) && (selection.Count>0))
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
                        selection[i].GetComponent<TetraController>().sw.startContinuousWalk(goalPoint);
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
                    foreach (Tetrahedron tetrahedron in selection[0].GetComponent<TetraController>().tetrahedrons)
                    {
                        foreach (Side side in tetrahedron.sides)
                        {
                            Vector3 finalPos = TetraUtil.averageVertices(side.vertices, selection[0]);

                            ui.numberAtPos(finalPos, side.ID, false,"s");
                        }
                    }
                }
                else
                {
                    foreach (Vertex vertex in selection[0].GetComponent<TetraController>().vertices)
                    {
                        Vector3 tempPos = selection[0].transform.TransformPoint(vertex.pos);

                        ui.numberAtPos(tempPos, vertex.ID, selection[0].GetComponent<TetraController>().newTetraVtx.Contains(vertex.ID));
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
        if (oneSelected())
        {
            selection[0].GetComponent<TetraController>().addTetraFromList();
            ui.addCenterAlert("You added a tetrahedron!");
        }
    }

    public void addSpecialFromList_()
    {
        if (oneSelected())
        {
            selection[0].GetComponent<TetraController>().addSpecialFromList();
            ui.addCenterAlert("You added a special tetrahedron!");
        }
    }

    public void invertLastTetra_()
    {
        if (oneSelected())
        {
            selection[0].GetComponent<TetraController>().invertLastTetra();
            ui.addCenterAlert("You inverted the last tetrahedron!");
        }
    }

    public void resetVertices()
    {
        inputID = 0;
    }

    public void toggleTracking()
    { 
        if(selection.Count > 0)
        {
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
}
