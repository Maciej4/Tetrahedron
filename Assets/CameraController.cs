using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public int Boundary = 100; // distance from edge scrolling starts
    public int speed = 10;
    private int theScreenWidth;
    private int theScreenHeight;
    public int zoomSpeed = 3;
    public int zoomMax = 100;
    public int zoomMin = 5;
    public float trackingDistance = 5;
    public bool trackingActive = false;
    public List<Transform> selectedTetrahedrons;
    public Transform goalPoint;
    private float optimalZ;

    void Start()
    {
        theScreenWidth = Screen.width;
        theScreenHeight = Screen.height;
        transform.rotation = Quaternion.LookRotation(new Vector3(0,-10,10)); // Look from (0,0,0) towards this point
    }

    void Update()
    {
        goalPoint = GameObject.FindGameObjectWithTag("GoalPoint").transform;
        Vector3 newPosition = transform.position;

        // Scroll
        if (Input.mousePosition.x < theScreenWidth && Input.mousePosition.x > theScreenWidth - Boundary && !trackingActive)
        {
            newPosition.x += speed * Time.deltaTime; // move on +X axis
        }
        if (Input.mousePosition.x > 0 && Input.mousePosition.x < 0 + Boundary && !trackingActive)
        {
            newPosition.x -= speed * Time.deltaTime; // move on -X axis
        }
        if (Input.mousePosition.y < theScreenHeight && Input.mousePosition.y > theScreenHeight - Boundary && !trackingActive)
        {
            newPosition.z += speed * Time.deltaTime; // move on +Z axis
        }
        if (Input.mousePosition.y > 0 && Input.mousePosition.y < 0 + Boundary && !trackingActive)
        {
            newPosition.z -= speed * Time.deltaTime; // move on -Z axis
        }

        //Unit Tracking
        if (Input.GetKeyDown(KeyCode.Space) && !(selectedTetrahedrons == null))
        {
            trackingActive = !trackingActive;

            trackingDistance = transform.position.y;
        }

        if (trackingActive)
        {
            float totalAverageX = 0.0f;
            float totalAverageZ = 0.0f;

            for (int i = 0; i < selectedTetrahedrons.Count; i++)
            {
                if (selectedTetrahedrons[i].GetComponent<TetraController>() != null)
                {
                    totalAverageX += selectedTetrahedrons[i].GetComponent<TetraController>().centerMass.x;
                    totalAverageZ += selectedTetrahedrons[i].GetComponent<TetraController>().centerMass.z;
                }
                else if (selectedTetrahedrons[i].GetComponent<GlobController>() != null)
                {
                    totalAverageX += selectedTetrahedrons[i].GetComponent<GlobController>().centerMass.x;
                    totalAverageZ += selectedTetrahedrons[i].GetComponent<GlobController>().centerMass.z;
                }
            }

            newPosition.x = totalAverageX / (float)selectedTetrahedrons.Count;
            optimalZ = totalAverageZ / (float)selectedTetrahedrons.Count;
        }

        // Zoom
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

                        for (int i = 0; i < selectedTetrahedrons.Count; i++) {
                            if (hit.transform == selectedTetrahedrons[i]) {
                                copyDetected = true;
                            }
                        }

                        if (!copyDetected)
                        {
                            selectedTetrahedrons.Add(hit.transform);
                        }
                    }
                }
                else
                {
                    if (!hit.transform.CompareTag("Terrain"))
                    {
                        selectedTetrahedrons.Clear();
                        selectedTetrahedrons.Add(hit.transform);
                    }
                    else
                    {
                        selectedTetrahedrons.Clear();
                        trackingActive = false;
                    }
                }
            }
        }

        //Placment of waypoint
        if (selectedTetrahedrons.Count > 0)
        {
            if (selectedTetrahedrons.Count == 1 && (selectedTetrahedrons[0].GetComponent<WalkController>() != null))
            {
                goalPoint.position = selectedTetrahedrons[0].GetComponent<WalkController>().goalPos;
            }
            else if (selectedTetrahedrons[0].GetComponent<WalkController>() != null)
            {
                bool allGoalsSame = true;
                Vector3 refercnceGoal = selectedTetrahedrons[0].GetComponent<WalkController>().goalPos;

                for (int i = 1; i < selectedTetrahedrons.Count; i++)
                {
                    if (selectedTetrahedrons[0].GetComponent<WalkController>() != null)
                    {
                        if (!(selectedTetrahedrons[i].GetComponent<WalkController>().goalPos == refercnceGoal))
                        {
                            allGoalsSame = false;
                        }
                    }
                }

                if (allGoalsSame)
                {
                    goalPoint.position = refercnceGoal;
                }
                else
                {
                    goalPoint.position = new Vector3(230.0f, -0.5f, 230.0f);
                }
            }
        }
        else
        {
            goalPoint.position = new Vector3(230.0f, -0.5f, 230.0f);
        }

        //Unit Movement Command
        if (Input.GetMouseButtonDown(1) && (selectedTetrahedrons.Count>0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.transform.CompareTag("Terrain"))
                {
                    goalPoint.position = hit.point;

                    for (int i = 0; i < selectedTetrahedrons.Count; i++)
                    {
                        selectedTetrahedrons[i].GetComponent<WalkController>().startContinuousWalk(goalPoint);
                    }
                }
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, newPosition, 0.5f);
    }

    void OnGUI()
    {
        GUI.Box(new Rect((Screen.width / 2) - 140, 5, 280, 25), "Camera Position = " + transform.position);
        if (selectedTetrahedrons.Count == 1)
        {
            if (trackingActive)
            {
                GUI.Box(new Rect((Screen.width / 2) - 100, 35, 200, 25), "Tracking: " + selectedTetrahedrons[0].name);
            }
            else
            {
                GUI.Box(new Rect((Screen.width / 2) - 100, 35, 200, 25), "Selection: " + selectedTetrahedrons[0].name);
            }
        }
        else if (selectedTetrahedrons.Count > 1)
        {
            if (trackingActive)
            {
                GUI.Box(new Rect((Screen.width / 2) - 100, 35, 200, 25), "Tracking: " + selectedTetrahedrons.Count.ToString("000") + " Tetrahedrons");
            }
            else
            {
                GUI.Box(new Rect((Screen.width / 2) - 100, 35, 200, 25), "Selection: " + selectedTetrahedrons.Count.ToString("000") + " Tetrahedrons");
            }
        }
    }
}
