using System;
using System.Collections;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityUXTesting.EndregasWarriors.Common;

namespace UnityUXTesting.EndregasWarriors.BugReporting.Monobehaviour
{
    public class BugReportingManagerBase : UXTool
    {
        [NonSerialized]
        public float countedTime = 0f;

        protected override void Update()
        {
            if (GameRecordingBrain._instance.status == CaptureSettings.StatusType.STARTED)
            {
                countedTime += Time.deltaTime;
            }
            base.Update();
        }
    }
}