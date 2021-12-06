using UnityEngine.Events;
using UnityUXTesting.EndregasWarriors.PlayRunReporting.Checkpoints;

namespace UnityUXTesting.EndregasWarriors.Common
{
    public class EventDelegate
    {
        public delegate void ErrorDelegate(CaptureSettings.ErrorCodeType error);
        public delegate void CompleteDelegate();

        public ErrorDelegate OnError;

        public CompleteDelegate OnComplete;

        public delegate void ReadyDelegate(string type);

        public ReadyDelegate onReady;

        public delegate void GameRecordingComplete(string finalFilePath);

        public GameRecordingComplete GameRecComplete;

        public delegate void PlayRunReportComplete();

        public PlayRunReportComplete PlayReportComplete;


        public delegate void CheckPointResult(CheckPoint self, Model.CheckPointResult result);

        public CheckPointResult CheckPointFinished;
        public delegate void CheckPointResultToSkip(CheckPoint self, Model.CheckPointResult result, UnityEvent cleanup);

        public CheckPointResultToSkip CheckPointFinishedToSkip;
        
        public delegate void SaveCheckPoints(Model.CheckPointResult[] results);

        public SaveCheckPoints SaveCheckPointsForLevel;

        public delegate void CheckPointStart(CheckPoint self);

        public CheckPointStart CheckPointStarted;
    }
}