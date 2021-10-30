using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityUXTesting.EndregasWarriors.Common;

namespace UnityUXTesting.EndregasWarriors.DataSending
{
    public class EndregasUXController : MonoBehaviour
    {
        private static EndregasUXController _instance;
        private int waitingPermissions = 1;
        private int permissionsGranted = 0;

        private bool userRequestedQuit = false;

        private void Awake()
        {
            DontDestroyOnLoad(this);
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void OnEnable()
        {
#if !UNITY_EDITOR
            Application.wantsToQuit += KillQuitProcess();
#else
            EditorApplication.playModeStateChanged +=  change => ExitPlayMode(change);
#endif

            GameRecordingBrain.eventDelegate.gameRecComplete += GameRecComplete;
            GameRecordingBrain.eventDelegate.OnError += OnError;
        }

        private void Start()
        {
            StartCoroutine(QuitAfterPermissionGranted());
        }

        private IEnumerator QuitAfterPermissionGranted()
        {
            yield return new WaitUntil(() =>
            {
                if (permissionsGranted == waitingPermissions && userRequestedQuit)
                    return true;
                else return false;
            });
            Debug.Log("I am readyyyyyyyyyyyyy!");
            Application.Quit();
        }

        private void ExitPlayMode(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.ExitingPlayMode)
            {
                userRequestedQuit = true;
                Debug.Log("You shall not pass!");
                EditorApplication.isPlaying = true;
                
            };
        }

        
        private bool KillQuitProcess()
        {
            userRequestedQuit = true;
            Debug.Log("You shall not pass!");
            return false;
        }


        private void OnError(CaptureSettings.ErrorCodeType error)
        {
            // ToDo: pop-up with informations
            Debug.Log("Error on sending video Data");
            Application.Quit();
        }

        private void GameRecComplete(string finalfilepath)
        {
            Debug.Log("Permission granted");
            permissionsGranted++;
        }
    }
}