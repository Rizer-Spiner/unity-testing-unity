namespace UnityUXTesting.EndregasWarriors.Common
{
    public class EventDelegate
    {
        /// <summary>
        /// To be notified when an error occurs during a action session, register
        /// a delegate using this signature by calling <c>OnError += </c>.
        /// </summary>
        /// <param name='error'>
        /// The code information passed to delegate when error occurs.
        /// </param>
        public delegate void ErrorDelegate(CaptureSettings.ErrorCodeType error);
        /// <summary>
        /// To be notified when the action is complete, register a delegate 
        /// using this signature by calling <c>OnComplete += </c>.
        /// </summary>
        public delegate void CompleteDelegate();
        /// <summary>
        /// The action session error delegate variable.
        /// </summary>
        public ErrorDelegate OnError;
        /// <summary>
        /// The action session complete delegate variable.
        /// </summary>
        public CompleteDelegate OnComplete;
        
        public delegate void ReadyDelegate(string type);

        public ReadyDelegate onReady;

        public delegate void GameRecordingComplete(string finalFilePath);

        public GameRecordingComplete GameRecComplete;
        
        public delegate void PlayRunReportComplete();

        public PlayRunReportComplete PlayReportComplete;
    }
}