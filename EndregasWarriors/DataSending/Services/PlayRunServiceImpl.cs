using System;
using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Networking;
using UnityUXTesting.EndregasWarriors.Common;
using UnityUXTesting.EndregasWarriors.Common.Model;

namespace UnityUXTesting.EndregasWarriors.DataSending
{
    public class PlayRunServiceImpl : IPlayRunService
    {
        public static EventDelegate eventDelegate = new EventDelegate();
        private PathConfigScriptable _config;

        public PlayRunServiceImpl(PathConfigScriptable configuration)
        {
            _config = configuration;
        }

        public IEnumerator PostPlayRunReport(PlayRunReport report)
        {
            
            WWWForm form = new WWWForm();
            form.AddField("report", JsonUtility.ToJson(report));

            string url = String.Format("{0}/playRunReport/add", _config.serverAddress);
            UnityWebRequest request = UnityWebRequest.Post(url, form);

            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log("Response: " + request.error);
                Debug.Log("Response: " + request.downloadHandler.text);

                eventDelegate.OnError?.Invoke(CaptureSettings.ErrorCodeType.REPORT_NOT_SEND);
            }
            else
            {
                Debug.Log("Response:  " + request.downloadHandler.text);
                eventDelegate.PlayReportComplete?.Invoke();
            }
        }
    }
}