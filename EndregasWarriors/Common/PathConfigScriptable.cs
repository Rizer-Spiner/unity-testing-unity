using System.Collections.Generic;
using UnityEngine;
using UnityUXTesting.EndregasWarriors.Common;

[CreateAssetMenu(menuName = "PathConfigScriptable")]
[System.Serializable]
public class PathConfigScriptable : ScriptableObject
{
    public string serverAddress = null;
    public string gameName = null;
    public string currentBuildID = null;

    public Dictionary<string, string> ServerPackageDictionary = new Dictionary<string, string>();

}
