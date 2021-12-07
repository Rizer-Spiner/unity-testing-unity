using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityUXTesting.EndregasWarriors.Common;

namespace UnityUXTesting.EndregasWarriors.DataSending
{
    public class GameRecordingServiceImpl : IGameRecordingService
    {
        public static EventDelegate eventDelegate = new EventDelegate();
        private PathConfigScriptable _config;

        public GameRecordingServiceImpl(PathConfigScriptable configuration)
        {
            _config = configuration;
        }

        public IEnumerator PostPlayThrough(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogWarning("File at " + filePath + " does not exists. Please provide a valid file path!");
                eventDelegate.OnError?.Invoke(CaptureSettings.ErrorCodeType.VIDEO_NOT_SENT);
                yield break;
            }

            WWWForm form = new WWWForm();
            form.AddField("game", _config.gameName);
            form.AddField("build", _config.currentBuildID);
            form.AddBinaryData("file", File.ReadAllBytes(filePath), new FileInfo(filePath).Name);

            string url = String.Format("{0}/video", _config.serverAddress);
            UnityWebRequest request = UnityWebRequest.Post(url, form);

            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log("Response: " + request.error);
                Debug.Log("Response: " + request.downloadHandler.text);

                eventDelegate.OnError?.Invoke(CaptureSettings.ErrorCodeType.VIDEO_NOT_SENT);
            }
            else
            {
                Debug.Log("Response:  " + request.downloadHandler.text);
                eventDelegate.GameRecComplete?.Invoke(filePath);
            }
        }
    }
}