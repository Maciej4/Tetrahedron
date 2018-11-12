using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidePicker : MonoBehaviour {
    public MeshRenderer alpha;
    public MeshRenderer beta;
    public MeshRenderer charlie;
    public MeshRenderer delta;

    private bool[] colorState = {false, false, false, false};

    public int targetSide = 0;

    // Use this for initialization
    void Start () {
		
	}

    private void UpdateColors()
    {
        if (colorState[0]) { alpha.material.color = Color.blue; } else { alpha.material.color = Color.red; }
        if (colorState[1]) { beta.material.color = Color.blue; } else { beta.material.color = Color.red; }
        if (colorState[2]) { charlie.material.color = Color.blue; } else { charlie.material.color = Color.red; }
        if (colorState[3]) { delta.material.color = Color.blue; } else { delta.material.color = Color.red; }
    }

    private void setTrue(int firstPoint, int secondPoint)
    {
        colorState[firstPoint] = true;
        colorState[secondPoint] = true;
    }

    private void setFalse()
    {
        colorState[0] = false;
        colorState[1] = false;
        colorState[2] = false;
        colorState[3] = false;
    }

    // Update is called once per frame
    void Update () {
        targetSide = targetSide % 6;
        setFalse();

        if (targetSide == 0)
        {
            setTrue(0, 1);
        }
        else if (targetSide == 1)
        {
            setTrue(0, 2);
        }
        else if (targetSide == 2)
        {
            setTrue(0, 3);
        }
        else if (targetSide == 3)
        {
            setTrue(1, 2);
        }
        else if (targetSide == 4)
        {
            setTrue(1, 3);
        }
        else if (targetSide == 5)
        {
            setTrue(2, 3);
        }

        UpdateColors();
	}
}
