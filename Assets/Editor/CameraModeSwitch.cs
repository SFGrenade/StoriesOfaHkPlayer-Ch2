using UnityEditor;
using UnityEngine;

public static class CameraModeSwitch
{
    [MenuItem("Camera/Orthographic")]
    static public void OrthographicCamera()
    {
        foreach (var camGo in GameObject.FindGameObjectsWithTag("MainCamera"))
            foreach (var cam in camGo.GetComponentsInChildren<Camera>())
                cam.transparencySortMode = TransparencySortMode.Orthographic;
    }
    [MenuItem("Camera/Perspective")]
    static public void PerspectiveCamera()
    {
        foreach (var camGo in GameObject.FindGameObjectsWithTag("MainCamera"))
            foreach (var cam in camGo.GetComponentsInChildren<Camera>())
                cam.transparencySortMode = TransparencySortMode.Default;
    }
}