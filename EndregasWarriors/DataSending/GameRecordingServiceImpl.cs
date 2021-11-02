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
            String fileName = String.Format("{0:d}_{1}.mp4", Utils.StringUtils.GetTimeString(), Environment.UserName);

            if (!File.Exists(filePath))
            {
                Debug.LogWarning("File at " + filePath + " does not exists. Please provide a valid file path!");
                eventDelegate.OnError?.Invoke(CaptureSettings.ErrorCodeType.VIDEO_NOT_SENT);
                yield break;
            }

            WWWForm form = new WWWForm();
            form.AddBinaryData("file", File.ReadAllBytes(filePath), fileName);

            string url = String.Format("{0}/video?Game={1}&Build={2}", _config.serverAddress, _config.gameName,
                _config.buildID);
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
                eventDelegate.gameRecComplete?.Invoke(filePath);
            }
        }
    }
}