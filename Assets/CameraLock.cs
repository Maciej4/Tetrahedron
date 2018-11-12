using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLock : MonoBehaviour {

    public float currentX = 0.0f;
    public float currentY = 6.0f;
    public float currentZ = 0.0f;

    public float angleX = 90;
    public float angleY = 0;
    public float angleZ = 0;

    public float currentXoffset = 0.0f;
    public float currentZoffset = 0.0f;
    public float angleXoffset = 0.0f;
    public float angleZoffset = 0.0f;

    public float radius;
    public float height;

    private float startMouseX;
    private float startMouseY;

    private float mouseX;
    private float mouseY;
    private float scrollY;
    private float factor;

    private float pi = Mathf.PI;
    private float cr = Mathf.Deg2Rad;

    public bool button3;
    public bool spaceBar;

    void Start() {

    }

    private float scrollFactor(float inputVal) {
        return (1.87f / inputVal) + 0.08f;
    }

    private void OnGUI()
    {
        Event e = Event.current;
        
        if (!spaceBar)
        {
            factor = scrollFactor(currentY);
            mouseX = e.mousePosition.x / (200f * scrollFactor(currentY));
            mouseY = e.mousePosition.y / (200f * scrollFactor(currentY));
        }
        else if (spaceBar)
        {
            mouseX = e.mousePosition.x / 50f;
            mouseY = e.mousePosition.y / 50f;
        }
        
        scrollY = Input.GetAxis("Mouse ScrollWheel");
    }

    private void panUpdate()
    {
        if (Input.GetMouseButtonDown(2))
        {
            button3 = true;
            startMouseX = mouseX;
            startMouseY = mouseY;
        }
        else if (Input.GetMouseButtonUp(2))
        {
            button3 = false;
            currentX -= currentXoffset;
            currentZ += currentZoffset;
            currentXoffset = 0.0f;
            currentZoffset = 0.0f;
        }

        if (button3)
        {
            currentXoffset = mouseX - startMouseX;
            currentZoffset = mouseY - startMouseY;
        }

        currentY += 2 * scrollY;
        currentY = Mathf.Clamp(currentY, 0.1f, 10f);

        transform.position = new Vector3(currentX - currentXoffset, currentY, currentZ + currentZoffset);
        transform.eulerAngles = new Vector3(90, 0, 0);
    }

    private void spaceUpdate() {
        if (Input.GetMouseButtonDown(2))
        {
            button3 = true;
            startMouseX = mouseX;
            startMouseY = mouseY;
        }
        else if (Input.GetMouseButtonUp(2))
        {
            button3 = false;
            angleX -= angleXoffset;
            angleZ -= angleZoffset;
            angleXoffset = 0.0f;
            angleZoffset = 0.0f;
        }

        if (button3)
        {
            angleXoffset = 10 * (mouseX - startMouseX);
            angleZoffset = 10 * (mouseY - startMouseY);
            angleZoffset = Mathf.Clamp(angleZoffset, -89f+angleZ, -30f+angleZ);
        }

        currentY += 2 * scrollY;
        currentY = Mathf.Clamp(currentY, 0.1f, 10f);
        angleZ = Mathf.Clamp(angleZ, 30f, 89f);

        radius = currentY * Mathf.Sin(pi/2-cr*(angleZ-angleZoffset));
        height = currentY * Mathf.Sin(cr*(angleZ-angleZoffset));

        float x = currentX + radius * Mathf.Cos(cr*(angleX-angleXoffset));
        float z = currentZ + radius * Mathf.Sin(cr*(angleX-angleXoffset));
        float y = height;
        
        transform.position = new Vector3(x, y, z);
        transform.LookAt(new Vector3(currentX,0.1f,currentZ));
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            spaceBar = !spaceBar;
            angleX = -90.0f;
            angleZ = 89.0f;
        }

        if (!spaceBar)
        {
            panUpdate();
        }
        else if (spaceBar)
        {
            spaceUpdate();
        }
    }
}
