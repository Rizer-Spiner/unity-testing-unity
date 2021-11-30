using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityUXTesting.EndregasWarriors.GameRecording;
using Object = UnityEngine.Object;


namespace UnityUXTesting.Editor
{
    public class AutomationFunctions : UnityEditor.Editor
    {
        [MenuItem("Tools/EndregasUX/Add UX Tools")]
        public static void AddUxTools()
        {
            var currentScenePath = EditorSceneManager.GetActiveScene().path;
            int index = 1;
            foreach (EditorBuildSettingsScene s in EditorBuildSettings.scenes)
            {
                EditorUtility.DisplayProgressBar("Adding EndregasUX tools", "Doing some work...",
                    index / EditorBuildSettings.scenes.Length + 1);
                var scene = EditorSceneManager.OpenScene(s.path);
                AddUxToolsForCurrentScene();
                EditorSceneManager.SaveScene(scene);
                index++;
            }

            EditorSceneManager.OpenScene(currentScenePath);
            EditorUtility.ClearProgressBar();
            Debug.Log("EndregasUX::Adding UX Tools finished!");
        }

        [MenuItem("Tools/EndregasUX/Clear All Tools")]
        public static void ClearUxTools()
        {
            var currentScenePath = EditorSceneManager.GetActiveScene().path;

            int index = 1;
            foreach (EditorBuildSettingsScene s in EditorBuildSettings.scenes)
            {
                EditorUtility.DisplayProgressBar("Delete all EndregasUX tools", "Doing some work...",
                    index / EditorBuildSettings.scenes.Length + 1);
                var scene = EditorSceneManager.OpenScene(s.path);
                ClearUxToolsForCurrentScene();
                EditorSceneManager.SaveScene(scene);
                index++;
            }

            EditorSceneManager.OpenScene(currentScenePath);
            EditorUtility.ClearProgressBar();
            Debug.Log("EndregasUX::Clear Ux Tools finished successfully");
        }

        [MenuItem("Tools/EndregasUX/Add Tools Current Scene")]
        private static void AddUxToolsForCurrentScene()
        {
            ClearUxToolsForCurrentScene();

            var audioListener = FindObjectOfType<AudioListener>();
            var cameras = FindObjectsOfType<Camera>();
            var mainCamera = cameras.FirstOrDefault(camera => camera.tag.Equals("MainCamera"));

            if (!(mainCamera is null)) mainCamera.gameObject.AddComponent<VideoCaptureTool>();
            else
            {
                Debug.LogError("EndregasUX::Adding tools in scene " + EditorSceneManager.GetActiveScene().name +
                               " was not possible because the mainCamera could not be found. Please add the tools manually");
                return;
            }

            if (!(audioListener is null)) audioListener.gameObject.AddComponent<AudioCaptureTool>();
            else
            {
                Debug.LogError("EndregasUX::Adding tools in scene " + EditorSceneManager.GetActiveScene().name +
                               " was not possible because the audioListener could not be found. Please add the tools manually");
                return;
            }

            var endregasUxManagerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/UnityUXTesting/Prefabs/EndregasUXManager.prefab");
            PrefabUtility.InstantiatePrefab(endregasUxManagerPrefab);
        }

        [MenuItem("Tools/EndregasUX/Clear Tools Current Scene")]
        private static void ClearUxToolsForCurrentScene()
        {
            Object[] obj = FindObjectsOfType(typeof(UXTool));

            foreach (var o in obj)
            {
                DestroyImmediate(o);
            }

            GameObject endregasUxManager = GameObject.Find("EndregasUXManager");
            if (!(endregasUxManager is null))
            {
                DestroyImmediate(endregasUxManager);
            }
        }
    }
}