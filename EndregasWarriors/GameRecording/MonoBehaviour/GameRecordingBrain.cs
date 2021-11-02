using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Retrofit;
using Retrofit.HttpImpl;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityUXTesting.EndregasWarriors.Common;
using UnityUXTesting.EndregasWarriors.DataSending;
using UnityUXTesting.EndregasWarriors.GameRecording;


public class GameRecordingBrain : GameRecordingBrainBase
{
    public CaptureSettings.StatusType status;

    public static GameRecordingBrain _instance;
    private bool hasAwakenBefore;
    private bool hasQuitBefore;
    private bool cameraRecorderReady;
    private bool audioRecorderReady;

    private Thread garbageCollectionThread;
    private bool areComponentsInitialized;

    public static EventDelegate eventDelegate = new EventDelegate();
    private IGameRecordingService _service;


    protected override void Awake()
    {
        status = CaptureSettings.StatusType.NOT_START;
        DontDestroyOnLoad(this);
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        if (!hasAwakenBefore)
        {
            base.Awake();
            VideoCaptureTool.eventDelegate.onReady += SetComponentReady;
            AudioCaptureTool.eventDelegate.onReady += SetComponentReady;

            garbageCollectionThread = new Thread(GarbageCollectionThreadFunction);
            garbageCollectionThread.Priority = System.Threading.ThreadPriority.Lowest;
            garbageCollectionThread.IsBackground = true;
            garbageCollectionThread.Start();

            //
            // RetrofitAdapter adapter = new RetrofitAdapter.Builder()
            //     .SetEndpoint("http://" + PathConfig.serverAddress)
            //     // .SetClient(new UnityWebRequestImpl())
            //     .Build();
            // _service = adapter.Create<IGameRecordingService>();

            hasAwakenBefore = true;
        }
    }

    protected override void Start()
    {
        SceneManager.sceneUnloaded += arg0 => status = CaptureSettings.StatusType.PAUSED;
        Application.focusChanged += hasFocus =>
        {
            if (!areComponentsInitialized) return;
            if (hasFocus)
            {
                status = CaptureSettings.StatusType.STARTED;
            }
            else status = CaptureSettings.StatusType.PAUSED;
        };


#if !UNITY_EDITOR
        Application.wantsToQuit += WantsToQuit;
#else
        EditorApplication.playModeStateChanged += change => ExitPlayMode(change);
#endif
    }


    private void SetComponentReady(string type)
    {
        if (type.Equals("video"))
            cameraRecorderReady = true;
        else audioRecorderReady = true;

        if (cameraRecorderReady && audioRecorderReady)
        {
            areComponentsInitialized = true;
            status = CaptureSettings.StatusType.STARTED;
            cameraRecorderReady = false;
            audioRecorderReady = false;
        }
    }

    private void ExitPlayMode(PlayModeStateChange change)
    {
        if (change == PlayModeStateChange.ExitingPlayMode) WantsToQuit();
    }

    private bool WantsToQuit()
    {
        if (!hasQuitBefore)
        {
            hasQuitBefore = true;
            status = CaptureSettings.StatusType.STOPPED;
            while (!_encoder.FinishEncoding())
            {
                // Thread.Sleep(1000);
            }

            CloseLibAPIs();
            if (MixAudioWithVideo())
            {
                // FileInfo fileInfo = new FileInfo(finalVideoFilePath);
                // MultipartBody multipartBody = new MultipartBody(fileInfo);
                //
                // var ob = _service.PostPlayRun(multipartBody, "multipart/form-data");
                //
                // ob.SubscribeOn(Scheduler.MainThread)
                //     .ObserveOn(Scheduler.ThreadPool)
                //     .Subscribe(data =>
                //     {
                //         Debug.Log("Response:  " + data);
                //
                //         eventDelegate.gameRecComplete?.Invoke(finalVideoFilePath);
                //     }, error =>
                //     {
                //         Debug.Log("Response: " + error);
                //         eventDelegate.OnError?.Invoke(CaptureSettings.ErrorCodeType.VIDEO_NOT_SENT);
                //     });

                StartCoroutine(PostVideo());
            }
            else eventDelegate.OnError?.Invoke(CaptureSettings.ErrorCodeType.VIDEO_AUDIO_MERGE_TIMEOUT);

            CleanLibAPIs();
            garbageCollectionThread.Abort();
            AudioCaptureTool.eventDelegate.onReady -= SetComponentReady;
            VideoCaptureTool.eventDelegate.onReady -= SetComponentReady;
#if !UNITY_EDITOR
        Application.wantsToQuit -= WantsToQuit;
#else
            EditorApplication.playModeStateChanged -= change => ExitPlayMode(change);
#endif
            status = CaptureSettings.StatusType.FINISH;
        }

        return true;
    }

    private IEnumerator PostVideo()
    {
        // finalVideoFilePath = "C:/Users/spiri/OneDrive/Documente/RockVR/Video/2021-10-31-11-23-27-ZNZBY.mp4";
        
        // byte[] boundary = UnityWebRequest.GenerateBoundary();
        // List<IMultipartFormSection> file = new List<IMultipartFormSection>();
        // file.Add(new MultipartFormFileSection("file", File.ReadAllBytes(finalVideoFilePath)));
        //
        // var request = UnityWebRequest.Post(PathConfig.serverAddress + "/video?Game=versioned&Build=1", file, boundary);
        //
        // //
        // // byte[] formSections = UnityWebRequest.SerializeFormSections(file, boundary);
        // // byte[] terminate = Encoding.UTF8.GetBytes(String.Concat("\r\n--", Encoding.UTF8.GetString(boundary), "--"));
        // //
        // // byte[] body = new byte[formSections.Length + terminate.Length];
        // // Buffer.BlockCopy(formSections, 0, body, 0, formSections.Length);
        // // Buffer.BlockCopy(terminate, 0, body, formSections.Length, terminate.Length);
        //
        // string contentType = String.Concat("multipart/form-data; boundary=", Encoding.UTF8.GetString(boundary));
        // // string contentType = "application/octet-stream";
        // // string contentType = "multipart/form-data";
        //
        //
        // // UnityWebRequest request = new UnityWebRequest(PathConfig.serverAddress + "/video?Game=versioned&Build=1", "POST");
        // // UploadHandler uploader = new UploadHandlerFile(finalVideoFilePath);
        // request.uploadHandler.contentType = contentType;
        // // request.uploadHandler = uploader;
        // request.downloadHandler = new DownloadHandlerBuffer(); 
        //
        // yield return request.SendWebRequest();
        //
        // if (request.isNetworkError || request.isHttpError)
        // {
        //     Debug.Log("Response: " + request.error);
        //     Debug.Log("Response: " + request.url);
        //     Debug.Log("Response: " + request.uri);
        //     Debug.Log("Response: " + request.downloadHandler.text);
        //
        //     eventDelegate.OnError?.Invoke(CaptureSettings.ErrorCodeType.VIDEO_NOT_SENT);
        // }
        // else
        // {
        //     Debug.Log("Response:  " + request.downloadHandler.text);
        //     eventDelegate.gameRecComplete?.Invoke(finalVideoFilePath);
        // }

        WWWForm form = new WWWForm();
        form.AddBinaryData("file", File.ReadAllBytes(finalVideoFilePath), "PlayRun2Identifier.mp4");
        
        UnityWebRequest request = UnityWebRequest.Post(PathConfig.serverAddress + "/video?Game=versioned&Build=1", form);
        yield return request.SendWebRequest();
        
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log("Response: " + request.error);
            Debug.Log("Response: " + request.url);
            Debug.Log("Response: " + request.uri);
            Debug.Log("Response: " + request.downloadHandler.text);

            eventDelegate.OnError?.Invoke(CaptureSettings.ErrorCodeType.VIDEO_NOT_SENT);
        }
        else
        {
            Debug.Log("Response:  " + request.downloadHandler.text);
            eventDelegate.gameRecComplete?.Invoke(finalVideoFilePath);
        }
        
        File.Delete(finalVideoFilePath);
    }

    void GarbageCollectionThreadFunction()
    {
        while (true)
        {
            if (status == CaptureSettings.StatusType.STARTED)
            {
                Thread.Sleep(1000);
                System.GC.Collect();
            }
        }
    }
}