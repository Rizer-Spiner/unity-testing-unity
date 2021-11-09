using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EditorCoroutines;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityUXTesting.Editor
{
    public class EndregasUX : EditorWindow
    {
        public static PathConfigScriptable subject;
        public string testAddress = "";
        public string newGameName = "";
        private string newBuildName = "";
        public static int GameNameIndex { get; set; }

        public Boolean addNewGameButtonPressed = false;
        public Boolean addNewBuildButtonPressed = false;

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
                addNewGameButtonPressed = true;
            }

            if (GUILayout.Button("Add new build"))
            {
                addNewBuildButtonPressed = true;
            }

            if (addNewGameButtonPressed)
            {
                AddGamePopView();
            }


            if (addNewBuildButtonPressed)
            {
                AddNewBuildPopView();
            }
        }

        private void AddGamePopView()
        {
            GUILayout.Space(20);
            newGameName = EditorGUILayout.TextField("Game name", newGameName);
            newBuildName = EditorGUILayout.TextField("Build name:", newBuildName);

            if (GUILayout.Button("Add game"))
            {
                addNewBuildButtonPressed = false;
                this.StartCoroutine(AddGameName(newGameName, newBuildName));
            }
        }

        private void AddNewBuildPopView()
        {
            GUILayout.Space(20);
            newBuildName = EditorGUILayout.TextField("Build name:", newBuildName);

            if (GUILayout.Button("Add build"))
            {
                addNewGameButtonPressed = false;
                this.StartCoroutine(AddNewBuild(newBuildName));
            }
        }

        private void Disconnect()
        {
            subject.serverAddress = null;
            subject.gameName = null;
            subject.currentBuildID = null;
           


            testAddress = "";
            newGameName = "";
            newBuildName = "";
            GameNameIndex = 0;

            addNewBuildButtonPressed = false;
            addNewGameButtonPressed = false;
            
            subject.ServerPackageDictionary = new Dictionary<string, string>();

            EditorUtility.SetDirty(target: subject);
        }

        private void Refresh()
        {
            this.StartCoroutine(Connect(subject.serverAddress));
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

            if (GUILayout.Button("Add game"))
            {
                this.StartCoroutine(AddGameName(newGameName, newBuildName));
            }
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
            }

            subject.ServerPackageDictionary.Add(gameName, buildName);
            subject.gameName = gameName;
            subject.currentBuildID = buildName;

            newGameName = "";
            newBuildName = "";
            addNewGameButtonPressed = false;
            
            EditorUtility.SetDirty(target: subject);
        }

        private IEnumerator AddNewBuild(string newBuildID)
        {
            WWWForm form = new WWWForm();
            form.AddField("gameName", subject.gameName);
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
                subject.currentBuildID = newBuildID;
            }
            
            subject.ServerPackageDictionary.Remove(subject.gameName);
            subject.ServerPackageDictionary.Add(subject.gameName, newBuildID);
            subject.currentBuildID = newBuildID;
            
            newGameName = "";
            newBuildName = "";
            addNewBuildButtonPressed = false;
            
            
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

            subject.ServerPackageDictionary = new Dictionary<string, string>();
            // subject.ServerPackageDictionary.Add("Test1", "BuildT1Nr1");
            // subject.ServerPackageDictionary.Add("Test2", "BuildT2Nr1");
            // subject.ServerPackageDictionary.Add("Test3", "BuildT3Nr1");

            subject.serverAddress = address;
            EditorUtility.SetDirty(target: subject);
        }

        #endregion HttpMethods
    }
}