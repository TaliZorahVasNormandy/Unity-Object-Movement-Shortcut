using System;
using System.Collections;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class moveObjectShortcut : EditorWindow {
    private static bool ctrlHeld = false;
    private static bool shiftHeld = false;
    private static Vector2 mouseDelta = Vector2.zero;
    private static Vector2 lastMousePosition = Vector2.zero;
    public static float sensitivity = 0.01f;
    private static float _sensitivity = sensitivity;
    public static bool globalDisable = false;

    [MenuItem("Tools/Move Object Shortcut")]
    static void Init()
    {
        var window = (moveObjectShortcut)GetWindow(typeof(moveObjectShortcut));
        window.Show();
    }

    void OnGUI()
    {
        sensitivity = EditorGUILayout.FloatField("Sensitivity", sensitivity);
        globalDisable = EditorGUILayout.Toggle("Disable all Shortcuts", globalDisable);
    }

    void OnFocus()
    {
        if (EditorPrefs.HasKey("sensitivity")) sensitivity = EditorPrefs.GetFloat("sensitivity");
        if (EditorPrefs.HasKey("globalDisable")) globalDisable = EditorPrefs.GetBool("globalDisable");
    }
    void OnLostFocus()
    {
        EditorPrefs.SetFloat("sensitivity", sensitivity);
        EditorPrefs.SetBool("globalDisable", globalDisable);
    }
    void OnDestroy()
    {
        EditorPrefs.SetFloat("sensitivity", sensitivity);
        EditorPrefs.SetBool("globalDisable", globalDisable);
    }

    static moveObjectShortcut(){
		//avoid registering twice to the SceneGUI delegate
		SceneView.duringSceneGui -= OnSceneView;
		SceneView.duringSceneGui += OnSceneView;
	}

    static void OnSceneView(SceneView sceneView) {
        Event e = Event.current;
        //disable on global disable
        if (!globalDisable)
        {
            //Check for key presses
            if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.LeftControl) ctrlHeld = true;
                if (e.keyCode == KeyCode.LeftShift) shiftHeld = true;
            }
            else if (e.type == EventType.KeyUp)
            {
                if (e.keyCode == KeyCode.LeftControl) ctrlHeld = false;
                if (e.keyCode == KeyCode.LeftShift) shiftHeld = false;
            }
            if (ctrlHeld && shiftHeld)
            {
                if (e.type == EventType.MouseDown)
                {
                    //get current mouse position as start position of the drag
                    if (e.button == 1) lastMousePosition = e.mousePosition;
                }
                if (e.type == EventType.MouseDrag)
                {
                    //get mouse speed based on frame time
                    mouseDelta = e.mousePosition - lastMousePosition;
                    lastMousePosition = e.mousePosition;
                    //move objects accordingly along axis
                    if (e.button == 1) moveObject(mouseDelta, "y", sceneView);
                }
            }
            else if (ctrlHeld)
            {
                if (e.type == EventType.MouseDown)
                {
                    if (e.button == 1) lastMousePosition = e.mousePosition;
                }
                if (e.type == EventType.MouseDrag)
                {
                    mouseDelta = e.mousePosition - lastMousePosition;
                    lastMousePosition = e.mousePosition;
                    if (e.button == 1) moveObject(mouseDelta, "x", sceneView);
                }
            }
            else if (shiftHeld)
            {
                if (e.type == EventType.MouseDown)
                {
                    if (e.button == 1) lastMousePosition = e.mousePosition;
                }
                if (e.type == EventType.MouseDrag)
                {
                    mouseDelta = e.mousePosition - lastMousePosition;
                    lastMousePosition = e.mousePosition;
                    if (e.button == 1) moveObject(mouseDelta, "z", sceneView);
                }
            }
        }
	}

    private static void moveObject(Vector3 distance, string axis, SceneView sceneView)
    {
        //Put all selected objects into array
        var selection = Selection.gameObjects;
        //Disable right click viewport navigation
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        //calculate distance from view to selected object, I'm using the first selected object here but this could also use an average
        float dist = Vector3.Distance(selection[0].transform.position, sceneView.camera.transform.position);
        _sensitivity = sensitivity * dist * 0.1f;

        if (axis == "x")
        {
            distance.y = 0;
            distance.z = 0;
            distance.x = -distance.x;
            distance = distance * _sensitivity;
            for (int i = 0; i < selection.Length; i++) selection[i].transform.position = selection[i].transform.position + distance;
        }
        if (axis == "y")
        {
            distance.x = 0;
            distance.z = 0;
            distance.y = -distance.y;
            distance = distance * _sensitivity;
            for (int i = 0; i < selection.Length; i++) selection[i].transform.position = selection[i].transform.position + distance;
        }
        if (axis== "z")
        {
            distance.y = 0;
            distance.z = distance.x;
            distance = distance * _sensitivity;
            Vector3 move = new Vector3(0, 0, distance.x);
            for (int i = 0; i < selection.Length; i++) selection[i].transform.position = selection[i].transform.position + move;
        }
        Event.current.Use();
    }
}
