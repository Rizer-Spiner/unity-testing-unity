using UnityEditor;
using UnityEngine;
using UnityUXTesting.EndregasWarriors.Common;

namespace UnityUXTesting.Editor
{
    public class EndregasUX : EditorWindow
    {

        [MenuItem("Tools/EndregasUX/Configuration")]
        private static void ShowWindow()
        {
            Selection.activeObject = 
                AssetDatabase.LoadAssetAtPath("Assets/UnityUXTesting/EndregasWarriors/Common/PathConfig.asset", 
                    typeof(PathConfigScriptable));
        }
        
        // private void OnGUI()
        // {
        //     GUILayout.Space(20);
        //     
        //     EditorGUILayout.LabelField("Configure your UX testing environment");
        //     GUILayout.Space(10);
        //     PathConfig.ServerAddress = EditorGUILayout.TextField("Server web address", PathConfig.ServerAddress);
        //     PathConfig.GameName = EditorGUILayout.TextField("Game name", PathConfig.GameName);
        //     PathConfig.BuildID = EditorGUILayout.TextField("BuildID", PathConfig.BuildID);
        // }
    }

  
}