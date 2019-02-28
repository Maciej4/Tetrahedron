using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController
{
    private CameraController camera;
    public List<Toggler> togglers = new List<Toggler>();

    public InputController(CameraController camera_)
    {
        camera = camera_;

        //togglers.Add(new Toggler(KeyCode.Alpha0, camera.selection[0].GetComponent<TetraController>().addTetraFromList));
        togglers.Add(new Toggler("[1] Add tetrahedron     ", KeyCode.Alpha1, camera.addTetraFromList_));
        togglers.Add(new Toggler("[2] Add special tetra   ", KeyCode.Alpha2, camera.addSpecialFromList_));
        togglers.Add(new Toggler("[3] Invert last tetra   ", KeyCode.Alpha3, camera.invertLastTetra_));
        togglers.Add(new Toggler("[Ctrl] Select tetra pts ", KeyCode.LeftControl, camera.resetVertices, 1));
        togglers.Add(new Toggler("[Space] Toggle tracking ", KeyCode.Space, camera.toggleTracking));
        togglers.Add(new Toggler("[Tab] Toggle edge view  ", KeyCode.Tab, camera.toggleEdgeView));
    }

    public void loop()
    {
        foreach (Toggler toggler in togglers)
        {
            toggler.loop();
        }
    }
}
