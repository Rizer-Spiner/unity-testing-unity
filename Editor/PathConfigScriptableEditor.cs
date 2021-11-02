using UnityEditor;
using UnityEngine;

namespace UnityUXTesting.Editor
{
    [CustomEditor(typeof(PathConfigScriptable))]
    public class PathConfigScriptableEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.Space(20);
            
            EditorGUILayout.LabelField("Configure your UX testing environment");
            base.OnInspectorGUI();
        }
    }
}