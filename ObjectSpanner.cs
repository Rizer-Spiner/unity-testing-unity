using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectSpanner : EditorWindow
{
    private string objectBaseName = "";
    private int objectID = 1;
    private GameObject objectToSpawn;
    private float objectScale;
    private float spawnRadius = 5f;

    [MenuItem("Tools/Basic Object Spawner")]
    public static void ShowWindow()
    {
        GetWindow(typeof(ObjectSpanner));
    }

    private void OnGUI()
    {
        GUILayout.Label("Spawn new object", EditorStyles.boldLabel);

        objectBaseName = EditorGUILayout.TextField("Base name", objectBaseName);
        objectID = EditorGUILayout.IntField("Object ID", objectID);
        objectScale = EditorGUILayout.Slider("Object Scale", objectScale, 0.5f, 3f);
        spawnRadius = EditorGUILayout.FloatField("Spawn Radius", spawnRadius);
        objectToSpawn =
            EditorGUILayout.ObjectField("Prefab to spawn", objectToSpawn, typeof(GameObject), false) as GameObject;

        if (GUILayout.Button("Spawn Object"))
        {
            SpawnObject();
        }
    }

    private void SpawnObject()
    {
        if (objectToSpawn == null)
        {
            Debug.LogError("Error: Please assign an object to spawn");
            return;
        }

        if (objectBaseName == string.Empty)
        {
            Debug.LogError("Error: Please enter a base name for the object.");
            return;
        }

        var spawnCircle = Random.insideUnitCircle * spawnRadius;
        var spawnPos = new Vector3(spawnCircle.x, 0f, spawnCircle.y);


        var newObject = Instantiate(objectToSpawn, spawnPos, Quaternion.identity);
        newObject.name = objectBaseName + objectID;
        newObject.transform.localScale = Vector3.one * objectScale;

        objectID++;
    }
}
