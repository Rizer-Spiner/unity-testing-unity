
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUXTesting.EndregasWarriors.Common;


public class GameRecordingBrain : GameRecordingBrainBase
{
    public CaptureSettings.StatusType status;
    
    public static GameRecordingBrain _instance;

    private bool hasAwakenBefore;
    private void Awake()
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

        if(!hasAwakenBefore) {
            base.Awake();
            hasAwakenBefore = true;
        }
    }

    private void Start()
    {
        SceneManager.sceneUnloaded += arg0 => status = CaptureSettings.StatusType.PAUSED;
        SceneManager.sceneLoaded += (arg0, mode) => status = CaptureSettings.StatusType.STARTED;

        Application.focusChanged += hasFocus =>
        {
            if (hasFocus) status = CaptureSettings.StatusType.STARTED;

            else status = CaptureSettings.StatusType.PAUSED;
        };
        Application.wantsToQuit += WantsToQuit;
        
        // status = CaptureSettings.StatusType.STARTED;
    }

    private bool WantsToQuit()
    {
        MixAudioWithVideo();
        status = CaptureSettings.StatusType.FINISH;
        //todo: send Video
        return true;
    }
}