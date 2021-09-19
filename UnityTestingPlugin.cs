using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor((typeof(Dropdown)))]
public class UnityTestingPlugin : EditorWindow
{
    
    string[] options = { "Object Tracker", "Time Tracker" };
    int index = 0;
    private GameObject testingArea;

    [MenuItem("Tools/Unity Testing Plugin")]
    static void Init()
    {
        var window = GetWindow<UnityTestingPlugin>();
        window.position = new Rect(0, 0, 180, 80);
        window.Show();
    }
    
    private void OnEnable()
    {
        if (testingArea == null)
        {
            testingArea = new GameObject("UNITY_PLUGIN_TESTING_AREA");
        }
        else
        {
            // DestroyImmediate(testingArea);
            // return;
        }
    }

    void OnGUI()
    {
        index = EditorGUI.Popup(
            new Rect(0, 0, position.width, 20),
            "Component:",
            index,
            options);

        if (GUI.Button(new Rect(0, 25, 100, 50), "Add Component"))
            AddComponentToObjects();
    }

    void AddComponentToObjects()
    {
        if (!Selection.activeGameObject)
        {
            Debug.LogError("Please select at least one GameObject first");
            return;
        }

        foreach (GameObject obj in Selection.gameObjects)
        {
            switch (index)
            {
                case 0:
                    Object trackerrPrefab = AssetDatabase.LoadAssetAtPath("Assets/Editor/ToolPrefabs/PositionTrackerTool.prefab", typeof(GameObject));
                    Object newTool = PrefabUtility.InstantiatePrefab(trackerrPrefab, testingArea.transform);
                    break;

                case 1:
                   Debug.Log("Tool yet to be implemented");
                    break;
            }
        }
    }
    
    
    
    
    
    
    //
    // private static GameObject testingArea;
    //
    //
    // public static string[] list = {"Object Position Tracker", "Time Tracker"};
    //
    //
    // [MenuItem("Tools/Object Position Tracker")]
    // public static void ShowWindow()
    // {
    //     GetWindow(typeof(UnityTestingPlugin));
    // }
    //
    // private void OnEnable()
    // {
    //     if (testingArea == null)
    //     {
    //         testingArea = new GameObject("UNITY_PLUGIN_TESTING_AREA");
    //     }
    //     else
    //     {
    //         DestroyImmediate(testingArea);
    //         return;
    //     }
    // }
    //
    // private void OnGUI()
    // {
    //     GUILayout.Label("Choose testing tool", EditorStyles.boldLabel);
    //     GUIContent labelType = new GUIContent("label");
    //     GUIContent[] options = new GUIContent[] {new GUIContent("Object Tacker"), new GUIContent("Time Tracker")};
    //     int selected = 0;
    //     selected = EditorGUILayout.Popup(labelType, selected, options);
    //
    //     if (GUILayout.Button("Add Testing Tool"))
    //     {
    //         Debug.Log(options[selected].text);
    //     }
    // }
    //
    // private void AddToolToArea(string type)
    // {
    //     
    // }
}
