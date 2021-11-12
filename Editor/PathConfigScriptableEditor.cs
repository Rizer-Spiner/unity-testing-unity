using System.Collections;
using UnityEditor;
using UnityEngine;

namespace UnityUXTesting.Editor
{
    [CustomEditor(typeof(PathConfigScriptable))]
    public class PathConfigScriptableEditor : UnityEditor.Editor
    {
       
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("To configure the configuration go under Tools->EndregasUX->Configuration");

        }
    }
}