using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class uiController
{
    private Transform cameraTransform;
    public List<string> alerts = new List<string>();
    private bool alertComplete = true;
    private float startTime = -10;
    private const float alertLenConst = 1.0f;
    private float alertLen = 1.0f;
    public string controls = "Controls not set up";
    public int controlsLength = 1;
    private int currentRepeats = 0;

    public uiController(Transform cameraTransform_)
    {
        cameraTransform = cameraTransform_;
    }

    public void loop()
    {
        displayControls();
        cameraPosition();

        if(alerts.Count > 5)
        {
            alertLen = 1.0f / (alerts.Count - 5);
        }
        else
        {
            alertLen = alertLenConst;
        }

        if (!alertComplete && alerts.Count != 0)
        {
            if (alerts[0] != null)
            {
                centerAlert(alerts[0] + countRepeats());
            }
            else
            {
                alerts.RemoveAt(0);
            }

            if (Time.time > startTime + alertLen)
            {
                alertComplete = true;
                alerts.RemoveAt(0);
                currentRepeats = 0;
            }
        }
        else if (alerts.Count > 0)
        {
            startTime = Time.time;
            alertComplete = false;
        }
    }

    public void addCenterAlert(string textInput)
    {
        alerts.Add(textInput);
    }

    private void centerAlert(string textInput)
    {
        GUI.Box(new Rect((Screen.width / 2) - 140, (Screen.height / 2) - 12, 280, 25), textInput);
    }

    public void singleTracking(bool tracked, string targetName)
    {
        if (tracked)
        {
            GUI.Box(new Rect((Screen.width / 2) - 140, 35, 280, 25), "Tracking: " + targetName);
        }
        else
        {
            GUI.Box(new Rect((Screen.width / 2) - 140, 35, 280, 25), "Selection: " + targetName);
        }
    }

    public void multipleTracking(bool tracked, int count)
    {
        if (tracked)
        {
            GUI.Box(new Rect((Screen.width / 2) - 140, 35, 280, 25), "Tracking: " + count.ToString("000") + " Tetrahedrons");
        }
        else
        {
            GUI.Box(new Rect((Screen.width / 2) - 140, 35, 280, 25), "Selection: " + count.ToString("000") + " Tetrahedrons");
        }
    }

    private void cameraPosition()
    {
        GUI.Box(new Rect((Screen.width / 2) - 140, 5, 280, 25), "Camera Position: " + cameraTransform.position);
    }

    private void displayControls()
    {
        GUI.Box(new Rect(10, 5, 200, 15 * controlsLength + 7), controls);
    }

    public void numberAtPos(Vector3 targetPos, int number, bool selected = false, string label = "p")
    {
        Vector3 screenPos = cameraTransform.GetComponent<Camera>().WorldToScreenPoint(targetPos);

        if (!selected)
        {
            GUI.Box(new Rect(screenPos.x, Screen.height - screenPos.y, 30, 25), label + number.ToString("00"));
        }
        else
        {
            GUI.Box(new Rect(screenPos.x, Screen.height - screenPos.y, 35, 25), "[" + number.ToString("00") + "]");
        }
    }

    public string countRepeats()
    {
        int loopNumber = 0;
        foreach (string alert in alerts)
        {
            if (alert == alerts[0] && loopNumber != 0)
            {
                currentRepeats++;
                alerts[loopNumber] = null;
                loopNumber--;
            }
            loopNumber++;
        }

        string repeatString = "";

        if (currentRepeats > 0)
        {
            repeatString = " x " + (currentRepeats + 1).ToString("00");
        }

        return repeatString;
    }
}
