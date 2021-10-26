using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityUXTesting.EndregasWarriors.Common;
using UnityUXTesting.EndregasWarriors.GameRecording;

namespace Tests
{
    public class VideoEncoderTest
    {
        private VideoEncoder subject;
        private bool condition;
        private GameRecordingBrain brain;

        [SetUp]
        public void SetUp()
        {
            var gameObject = new GameObject("GameBrain");
            gameObject.AddComponent<AudioListener>();
            gameObject.AddComponent<GameRecordingBrain>();
            gameObject.AddComponent<AudioCaptureTool>();

            subject = gameObject.GetComponent<GameRecordingBrain>()._encoder;
            brain = gameObject.GetComponent<GameRecordingBrain>();
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator Initialize()
        {
            yield return null;
            Assert.False(subject.FinishEncoding());
        }

        [UnityTest]
        public IEnumerator EnqueueFrame()
        {
            yield return null;
            Texture2D frameTexture = new Texture2D(GameRecordingBrain._instance.GetFrameWidth(),
                GameRecordingBrain._instance.GetFrameHeight(),
                TextureFormat.RGB24,
                false);

            frameTexture.hideFlags = HideFlags.HideAndDontSave;
            frameTexture.wrapMode = TextureWrapMode.Clamp;
            frameTexture.filterMode = FilterMode.Trilinear;
            frameTexture.hideFlags = HideFlags.HideAndDontSave;
            frameTexture.anisoLevel = 0;

            yield return typeof(WaitForEndOfFrame);
            frameTexture.ReadPixels(new Rect(0, 0,
                Screen.width,
                Screen.height), 0, 0, false);

            frameTexture.Apply();

            subject.EnqueueFrame(new VideoEncoder.FrameData(frameTexture.GetRawTextureData(), 1));

            var type = subject.GetType();
            BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Instance;
            FieldInfo queue = type.GetField("frameQueue", bindingAttr);

            Queue<VideoEncoder.FrameData> frameDatas = (Queue<VideoEncoder.FrameData>) queue.GetValue(subject);
            if (!(queue is null)) Assert.Equals(frameDatas.Count, 1);
        }

        [UnityTest]
        public IEnumerator FinishAfterStopCommand()
        {
            yield return null;
            brain.status = CaptureSettings.StatusType.STOPPED;

            Assert.True(subject.FinishEncoding());
        }

        [UnityTest]
        public IEnumerator Abort()
        {
            yield return null;
            var type = subject.GetType();
            BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Instance;
            FieldInfo threadInfo = type.GetField("encodeThread", bindingAttr);

            Thread thread = (Thread) threadInfo.GetValue(subject);

            subject.Abort();

            Assert.False(thread.IsAlive);
        }
    }
}