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
            hasAwakenBefore = true;
        }
    }

    protected override void Start()
    {
        SceneManager.sceneUnloaded += arg0 => status = CaptureSettings.StatusType.PAUSED;
        Application.focusChanged += hasFocus =>
        {
            if (hasFocus) status = CaptureSettings.StatusType.STARTED;

            else status = CaptureSettings.StatusType.PAUSED;
        };
        VideoCaptureTool.eventDelegate.onReady += SetComponentReady;
        // AudioCaptureTool.eventDelegate.onReady += SetComponentReady;

#if !UNITY_EDITOR
        Application.wantsToQuit += WantsToQuit;
#else
        EditorApplication.playModeStateChanged += change => WantsToQuit();
#endif
    }

    private void SetComponentReady(string type)
    {
        if (type.Equals("video"))
            cameraRecorderReady = true;
        // else audioRecorderReady = true;

        if (cameraRecorderReady)
            // if (cameraRecorderReady && audioRecorderReady)
        {
            Debug.Log("Step7");
            status = CaptureSettings.StatusType.STARTED;
            cameraRecorderReady = false;
            // audioRecorderReady = false;
        }
    }

    private bool WantsToQuit()
    {
        status = CaptureSettings.StatusType.STOPPED;
        while (!_encoder.FinishEncoding())
        {
            Thread.Sleep(1000);
        }

        CloseLibAPIs();
        MixAudioWithVideo();
        CleanLibAPIs();
        status = CaptureSettings.StatusType.FINISH;
        //todo: send Video
        return true;
    }
    
    void GarbageCollectionThreadFunction()
    {
        while (status == CaptureSettings.StatusType.STARTED)
        {
            // TODO, adjust gc interval dynamic.
            Thread.Sleep(1000);
            System.GC.Collect();
        }
    }
}