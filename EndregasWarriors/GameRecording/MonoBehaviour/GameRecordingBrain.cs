using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUXTesting.EndregasWarriors.Common;
using UnityUXTesting.EndregasWarriors.GameRecording;


public class GameRecordingBrain : GameRecordingBrainBase
{
    public CaptureSettings.StatusType status;

    public static GameRecordingBrain _instance;
    private bool hasAwakenBefore;
    private bool cameraRecorderReady;
    private bool audioRecorderReady;

    private Thread garbageCollectionThread;
    private bool areComponentsInitialized;
    
    public static EventDelegate eventDelegate = new EventDelegate();
    
    
    
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
            Destroy(gameObject);
            return;
        }

        garbageCollectionThread = new Thread(GarbageCollectionThreadFunction);
        garbageCollectionThread.Priority = System.Threading.ThreadPriority.Lowest;
        garbageCollectionThread.IsBackground = true;
        garbageCollectionThread.Start();

        if (!hasAwakenBefore)
        {
            base.Awake();
            VideoCaptureTool.eventDelegate.onReady += SetComponentReady;
            AudioCaptureTool.eventDelegate.onReady += SetComponentReady;
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
        status = CaptureSettings.StatusType.STOPPED;
        while (!_encoder.FinishEncoding())
        {
            Thread.Sleep(1000);
        }


        CloseLibAPIs();
        if(MixAudioWithVideo())
            eventDelegate.gameRecComplete?.Invoke(finalVideoFilePath);
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
     
        return true;
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