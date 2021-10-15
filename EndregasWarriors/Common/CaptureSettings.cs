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

        public struct DefaultSettings
        {
            public FrameSizeType framesize => FrameSizeType._1280x720;

            public EncodeQualityType encodeQualityType
            {
                get => EncodeQualityType.High;
                internal set => encodeQualityType = value;
            }

            public StereoFormatType stereoFormatType => StereoFormatType.HALF;

            public TargetFramerateType framerate
            {
                get => TargetFramerateType._60;
                internal set => framerate = value;
            }

            public int frameWith => 1280;
            public int frameHeight => 720;
        }
    }
}