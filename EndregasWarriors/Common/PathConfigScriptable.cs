using UnityEngine;

// [CreateAssetMenu(menuName = "PathConfigScriptable")]
[System.Serializable]
public class PathConfigScriptable : ScriptableObject
{
    public string serverAddress = "http://localhost:8080";
    public string gameName = "TestGame";
    public string buildID = "BuildNr1";
    
}
