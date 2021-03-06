namespace UnityUXTesting.EndregasWarriors.Common
{
    public class CaptureSettings
    {
        public enum StatusType
        {
            NOT_START,
            STARTED,
            PAUSED,
            STOPPED,
            FINISH,
        }

        public enum ErrorCodeType
        {
            CAMERA_AUDIO_CAPTURE_NOT_FOUND = -1,
            FFMPEG_NOT_FOUND = -2,
            VIDEO_AUDIO_MERGE_TIMEOUT = -3,
            VIDEO_NOT_SENT= -4,
            REPORT_NOT_SEND = -5
        }

        public enum FrameSizeType
        {
            _640x480,
            _720x480,
            _960x540,
            _1280x720,
            _1920x1080,
            _2048x1080,
            _3840x2160,
            _4096x2160,
            _7680x4320,
            // Add your custom resolution here (modify frameWidth, frameHeight accordingly):
        }
        

        public enum EncodeQualityType
        {
            Low,
            Medium,
            High,
        }

        public enum AntiAliasingType
        {
            _1,
            _2,
            _4,
            _8,
        }

        public enum TargetFramerateType
        {
            _18,
            _24,
            _30,
            _45,
            _60,
        }

        public enum StereoFormatType
        {
            FULL,
            HALF
        }

      }
}