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
        private int waitingPermissions = 2;
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
                Destroy(this);
            }
        }

        private void OnEnable()
        {
#if !UNITY_EDITOR
            Application.wantsToQuit += ApplicationOnwantsToQuit;
#else
            EditorApplication.playModeStateChanged += change => ExitPlayMode(change);
#endif

            GameRecordingServiceImpl.eventDelegate.GameRecComplete += GameRecComplete;
            GameRecordingServiceImpl.eventDelegate.OnError += OnError;

            PlayRunServiceImpl.eventDelegate.PlayReportComplete += PlayReportComplete;
            PlayRunServiceImpl.eventDelegate.OnError += OnError;
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
                {
#if !UNITY_EDITOR
                    Application.wantsToQuit -= ApplicationOnwantsToQuit;
#else
                    EditorApplication.playModeStateChanged -= change2 => ExitPlayMode(change2);
                    return true;
#endif
                }
                else return false;
            });
#if !UNITY_EDITOR
            Application.Quit();
#else
            EditorApplication.isPlaying = false;
#endif
        }

        private void ExitPlayMode(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.ExitingPlayMode && !userRequestedQuit)
            {
                userRequestedQuit = true;
                EditorApplication.isPlaying = true;
            }
        }

        private bool ApplicationOnwantsToQuit()
        {
            if (!userRequestedQuit)
            {
                userRequestedQuit = true;
                return false;
            }
            else return false;
        }


        private void OnError(CaptureSettings.ErrorCodeType error)
        {
            // ToDo: pop-up with informations
            Debug.Log("Error on sending Data");

            switch (error)
            {
                case CaptureSettings.ErrorCodeType.VIDEO_NOT_SENT:
                {
                    GameRecordingServiceImpl.eventDelegate.GameRecComplete -= GameRecComplete;
                    GameRecordingServiceImpl.eventDelegate.OnError -= OnError;
                    break;
                }
                case CaptureSettings.ErrorCodeType.REPORT_NOT_SEND:
                {
                    PlayRunServiceImpl.eventDelegate.PlayReportComplete -= PlayReportComplete;
                    PlayRunServiceImpl.eventDelegate.OnError -= OnError;
                    break;
                }
                default:
                {
                    break;
                }
            }
            permissionsGranted++;
        }

        private void GameRecComplete(string finalfilepath)
        {
            GameRecordingServiceImpl.eventDelegate.GameRecComplete -= GameRecComplete;
            GameRecordingServiceImpl.eventDelegate.OnError -= OnError;
            permissionsGranted++;
        }
        
        private void PlayReportComplete()
        {
            PlayRunServiceImpl.eventDelegate.PlayReportComplete -= PlayReportComplete;
            PlayRunServiceImpl.eventDelegate.OnError -= OnError;
            permissionsGranted++;
        }
    }
}