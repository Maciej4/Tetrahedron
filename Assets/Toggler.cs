using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toggler
{
    public string name;
    public KeyCode toggleKey;
    public Action method;
    public int edgeSet = 0;
    private bool lastRun = false;

    public Toggler(string name_, KeyCode toggleKey_, Action method_, int edgeSet_ = 0)
    {
        toggleKey = toggleKey_;
        method = method_;
        edgeSet = edgeSet_;
        name = name_;
    }

    public void loop()
    {
        switch (edgeSet)
        {
            case 0:
                if (Input.GetKeyDown(toggleKey))
                {
                    if (!lastRun)
                    {
                        method.Invoke();
                    }
                    lastRun = true;
                }
                else
                {
                    lastRun = false;
                }
                break;

            case 1:
                if (Input.GetKeyUp(toggleKey))
                {
                    if (!lastRun)
                    {
                        method.Invoke();
                    }
                    lastRun = true;
                }
                else
                {
                    lastRun = false;
                }
                break;

            case 2:
                if (Input.GetKey(toggleKey))
                {
                    method.Invoke();
                }
                break;
        }
    }
}
