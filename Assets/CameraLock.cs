using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLock : MonoBehaviour {

    public float currentX = 0.0f;
    public float currentY = 2.0f;
    public float currentZ = 0.0f;

    public float angleX = 90;
    public float angleY = 0;
    public float angleZ = 0;

    public float currentXoffset = 0.0f;
    public float currentZoffset = 0.0f;
    public float angleXoffset = 0.0f;
    public float angleZoffset = 0.0f;

    public float startMouseX;
    public float startMouseY;

    public float mouseX;
    public float mouseY;
    public float scrollY;
    public float factor;

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
            startMouseX = mouseX*10;
            startMouseY = mouseY*10;
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
            angleXoffset = mouseX*10 - startMouseX;
            angleZoffset = mouseY*10 - startMouseY;
            angleZoffset = Mathf.Clamp(angleZoffset, -70f+angleZ, 30f+angleZ);
        }

        currentY += 2 * scrollY;
        currentY = Mathf.Clamp(currentY, 0.1f, 10f);
        angleZ = Mathf.Clamp(angleZ, -30f, 70f);

        float x = currentX + currentY * Mathf.Cos(((angleX-angleXoffset)*3.14f)/180.0f);
        float z = currentZ + currentY * Mathf.Sin(((angleX-angleXoffset)*3.14f)/180.0f);
        float distance = Vector3.Distance(new Vector3(x, 0f, z), new Vector3(currentX, 0f, currentZ));
        float y = currentY + distance*Mathf.Tan(((angleZ-angleZoffset)*3.14f)/180.0f);
        
        transform.position = new Vector3(x, y, z);
        transform.LookAt(new Vector3(currentX,0.1f,currentZ));
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            spaceBar = !spaceBar;
            angleX = -90.0f;
            angleZ = 70.0f;
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
