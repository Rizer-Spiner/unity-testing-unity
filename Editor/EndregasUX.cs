using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EditorCoroutines;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityUXTesting.EndregasWarriors.Common;

namespace UnityUXTesting.Editor
{
    public class EndregasUX : EditorWindow
    {
        public static PathConfigScriptable subject;
        public string testAddress = "";
        public string newGameName = "";
        private string newBuildName = "";
        public static int GameNameIndex { get; set; }

        [MenuItem("Tools/EndregasUX/Configuration")]
        private static void ShowWindow()
        {
            subject = (PathConfigScriptable) AssetDatabase.LoadAssetAtPath(
                "Assets/UnityUXTesting/EndregasWarriors/Common/PathConfig.asset",
                typeof(PathConfigScriptable));

            EndregasUX window = (EndregasUX) GetWindow(typeof(EndregasUX));
            window.Show();
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }

        private void OnGUI()
        {
            if (subject.serverAddress == null || subject.serverAddress.Equals(""))
                ServerConfigurationView();
            else
            {
                if (subject.ServerPackageDictionary.Keys.Count == 0)
                    AddGameView();
                else ConfigurationMenuView();
            }
        }

        private void ConfigurationMenuView()
        {
            if (GUILayout.Button("Refresh"))
            {
                Refresh();
            }

            if (GUILayout.Button("Disconnect"))
            {
                Disconnect();
            }

            GUILayout.Space(20);
            EditorGUILayout.LabelField("Currently connected to: " + subject.serverAddress);
            GUILayout.Space(10);

            GameNameIndex = EditorGUILayout.Popup("Game name", GameNameIndex,
                subject.ServerPackageDictionary.Keys.ToArray());
            subject.gameName = subject.ServerPackageDictionary.Keys.ToArray()[GameNameIndex];
            subject.currentBuildID = subject.ServerPackageDictionary[subject.gameName];

            EditorGUILayout.LabelField("Current build: " + subject.currentBuildID);

            if (GUILayout.Button("Add new game registry"))
            {
                AddNewGamePopView();
            }

            if (GUILayout.Button("Add new build"))
            {
                AddNewBuildPopView();
            }
        }

        private void AddNewBuildPopView()
        {
        }

        private void Disconnect()
        {
            subject.serverAddress = null;
            subject.gameName = null;
            subject.currentBuildID = null;
            subject.ServerPackageDictionary = new Dictionary<string, string>();


            testAddress = "";
            newGameName = "";
            newBuildName = "";

            EditorUtility.SetDirty(target: subject);
        }

        private void Refresh()
        {
            this.StartCoroutine(Connect(subject.serverAddress));
        }

        private void AddNewGamePopView()
        {
            AddGameView();
        }

        private void AddGameView()
        {
            GUILayout.Space(20);
            EditorGUILayout.LabelField("Configure your UX testing environment");
            GUILayout.Space(10);

            EditorGUILayout.LabelField("Currently connected to: " + subject.serverAddress);
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Game registry empty. Add a game: ");

            newGameName = EditorGUILayout.TextField("Game name", newGameName);
            newBuildName = EditorGUILayout.TextField("Build name:", newBuildName);

            if (GUILayout.Button("Add game")) this.StartCoroutine(AddGameName(newGameName, newBuildName));
        }

        private void ServerConfigurationView()
        {
            subject.gameName = null;
            subject.currentBuildID = null;
            subject.ServerPackageDictionary = new Dictionary<string, string>();

            GUILayout.Space(20);

            EditorGUILayout.LabelField("Configure your UX testing environment");
            GUILayout.Space(10);

            testAddress = EditorGUILayout.TextField("Server address", testAddress);

            if (GUILayout.Button("Test server connection"))
            {
                if (!testAddress.Equals("") &&
                    (testAddress.StartsWith("http://") || testAddress.StartsWith("https://")))
                {
                    this.StartCoroutine(Connect(testAddress));
                }
                else
                {
                    Debug.LogWarning("EndregasUX::Configuration: Please insert a valid web address");
                }
            }

            EditorUtility.SetDirty(target: subject);
        }

        #region Http methods

        private IEnumerator AddGameName(string gameName, string buildName)
        {
            WWWForm form = new WWWForm();
            form.AddField("gameName", gameName);
            form.AddField("buildName", buildName);

            string url = String.Format("{0}/addGame", subject.serverAddress);
            UnityWebRequest request = UnityWebRequest.Post(url, form);

            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                String response = String.Format("{0} {1} {2} {3} {4}",
                    "EndregasUX::Configuration: Could not add new game option! ",
                    "\n",
                    request.downloadHandler.text,
                    "\n",
                    "Please try again!"
                );
                Debug.LogError(response);
            }
            else
            {
                subject.ServerPackageDictionary.Add(gameName, buildName);
                subject.gameName = gameName;
                subject.currentBuildID = buildName;
                EditorUtility.SetDirty(target: subject);
            }

            // subject.ServerPackageDictionary.Add(gameName, buildName);
            // subject.gameName = gameName;
            // subject.currentBuildID = buildName;
            // EditorUtility.SetDirty(target: subject);
        }

        private IEnumerator AddNewBuild(string newBuildID)
        {
            WWWForm form = new WWWForm();
            form.AddField("buildID", newBuildID);

            string url = String.Format("{0}/addNewBuild", subject.serverAddress);
            UnityWebRequest request = UnityWebRequest.Post(url, form);

            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                String response = String.Format("{0} {1} {2} {3} {4}",
                    "EndregasUX::Configuration: Could not add new game option! ",
                    "\n",
                    request.downloadHandler.text,
                    "\n",
                    "Please try again!"
                );
                Debug.LogError(response);
            }
            else
            {
                subject.ServerPackageDictionary.Remove(subject.gameName);
                subject.ServerPackageDictionary.Add(subject.gameName, newBuildID);
                EditorUtility.SetDirty(target: subject);
            }

            EditorUtility.SetDirty(target: subject);
        }


        private IEnumerator Connect(string address)
        {
            UnityWebRequest request = UnityWebRequest.Get(address + "/connect");

            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                String response = String.Format("{0} {1} {2} {3} {4}",
                    "EndregasUX::Configuration: Could not connect to respective server! ",
                    "\n",
                    request.downloadHandler.text,
                    "\n",
                    "Please try again with a different address!"
                );
                Debug.LogError(response);
            }
            else
            {
                subject.ServerPackageDictionary =
                    JsonUtility.FromJson<Dictionary<string, string>>(request.downloadHandler.text);
                subject.serverAddress = address;
                EditorUtility.SetDirty(subject);
            }

            // subject.ServerPackageDictionary = new Dictionary<string, string>();
            // subject.ServerPackageDictionary.Add("Test1", "BuildT1Nr1");
            // subject.ServerPackageDictionary.Add("Test2", "BuildT2Nr1");
            // subject.ServerPackageDictionary.Add("Test3", "BuildT3Nr1");

            // subject.serverAddress = address;
            EditorUtility.SetDirty(target: subject);
        }

        #endregion HttpMethods


        // private void OnGUI()
        // {
        //     GUILayout.Space(20);
        //
        //     EditorGUILayout.LabelField("Configure your UX testing environment");
        //     GUILayout.Space(10);
        //     Debug.Log("ServerPackage 1" + subject.serverPackage.ToString());
        //
        //     if (subject.serverAddress.Equals(START_ADDRESS)) ConfigureServer();
        //     else
        //     {
        //         if (!checkForUpdates)
        //             CheckForUpdates();
        //         ContinueToolConfiguration();
        //     }
        // }
        //
        // private void CheckForUpdates()
        // {
        //     Debug.Log("ServerPackage 2" + subject.serverPackage.ToString());
        //     checkForUpdates = true;
        //     this.StartCoroutine(Connect(subject.serverAddress));
        // }
        //
        // private void ConfigureServer()
        // {
        //     subject.gameName = "";
        //     subject.buildID = "";
        //     subject.serverPackage = new ServerPackage()
        //     {
        //         GameNames = new List<string>(),
        //         BuildIDs = new List<string>()
        //     };
        //
        //     EditorGUILayout.LabelField("Add server web address:");
        //     testAddress = EditorGUILayout.TextField("Server address", testAddress);
        //
        // if (GUILayout.Button("Test server connection"))
        // {
        //     if (!testAddress.Equals(START_ADDRESS) &&
        //         (testAddress.StartsWith("http://") || testAddress.StartsWith("https://")))
        //     {
        //         this.StartCoroutine(Connect(testAddress));
        //     }
        //     else
        //     {
        //         Debug.LogWarning("EndregasUX::Configuration: Please insert a valid web address");
        //     }
        // }
        // }
        //
        // private void ContinueToolConfiguration()
        // {
        //     subject.serverAddress = EditorGUILayout.TextField("Server web address", subject.serverAddress);
        //     GUILayout.Space(5);
        //     if (subject.serverPackage.GameNames.Count > 0)
        //     {
        //         GameNameIndex =
        //             EditorGUILayout.Popup("Game name", GameNameIndex, subject.serverPackage.GameNames.ToArray());
        //         subject.gameName = subject.serverPackage.GameNames[GameNameIndex];
        //     }
        //     else
        //     {
        //         string gameName = "";
        //         gameName = EditorGUILayout.TextField("Add game name", gameName);
        //
        //         if (!gameName.Equals("") && GUILayout.Button("Add game"))
        //         {
        //             this.StartCoroutine(AddGameName(gameName));
        //         }
        //     }
        //
        //     GUILayout.Space(5);
        //     if (subject.serverPackage.BuildIDs.Count > 0)
        //     {
        //         BuildIDIndex = EditorGUILayout.Popup("BuildID", BuildIDIndex, subject.serverPackage.BuildIDs.ToArray());
        //         subject.buildID = subject.serverPackage.BuildIDs[BuildIDIndex];
        //     }
        //     else
        //     {
        //     }
        //
        //     EditorUtility.SetDirty(target: subject);
        // }
        //
        //
        //
        //
        //
        //
    }
}