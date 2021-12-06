using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityUXTesting.EndregasWarriors.BugReporting.Monobehaviour;
using UnityUXTesting.EndregasWarriors.Common;
using UnityUXTesting.EndregasWarriors.Common.Model;

namespace UnityUXTesting.EndregasWarriors.PlayRunReporting.Checkpoints
{
    public class CheckPoint : UXTool
    {
        public string name;
        public GameObject trackedObject;
        public static EventDelegate _eventDelegate = new EventDelegate();
        [Tooltip(
            "Mention the number of minutes the checkpoint should be reached from the beginning of the level or from the last checkpoint. Value as float.")]
        [Range(0f, 60f)]
        public float recommendedTimeForCompletion;
        public UnityEvent cleanUp;


        private GameObject finishMark;
        [NonSerialized] public GameObject startMark;
        [NonSerialized] public bool check;
        [NonSerialized] public bool startCheckPoint;
        [NonSerialized] public bool checkpointFinished;
        [NonSerialized] public float startCheckPointTime;

        protected override void Start()
        {
            recommendedTimeForCompletion = recommendedTimeForCompletion * 60;
            var marks = GetComponentsInChildren<Mark>();
            startMark = marks[0].gameObject;
            finishMark = marks[1].gameObject;

            startMark.GetComponent<Mark>().trackedObject = trackedObject;
            finishMark.GetComponent<Mark>().trackedObject = trackedObject;

            startMark.GetComponent<Mark>().Detection += OnStartCheckPoint;
            finishMark.GetComponent<Mark>().Detection += OnCheckPointFinished;

            base.Start();
        }

        private void OnCheckPointFinished()
        {
            checkpointFinished = true;
            startCheckPoint = false;
        }

        private void OnStartCheckPoint()
        {
            startCheckPoint = true;
            startCheckPointTime = BugReportingManager._instance.countedTime;
            _eventDelegate.CheckPointStarted?.Invoke(this);
        }


        protected override void Update()
        {
            if (!check)
            {
                if (checkpointFinished)
                {
                    CheckPointResult result = new CheckPointResult()
                    {
                        name = name,
                        result = GameState.COMPLETED,
                        actualPlayInterval = new TimeInterval
                        {
                            start = startCheckPointTime,
                            end = BugReportingManager._instance.countedTime
                        }
                    };
                    check = true;
                    _eventDelegate.CheckPointFinished?.Invoke(this, result);
                    return;
                }

                if (startCheckPoint && BugReportingManager._instance.countedTime - startCheckPointTime >
                    recommendedTimeForCompletion)
                {
                    CheckPointResult result = new CheckPointResult()
                    {
                        name = name,
                        result = GameState.TO_SKIPP,
                        actualPlayInterval = new TimeInterval
                        {
                            start = startCheckPointTime,
                            end = BugReportingManager._instance.countedTime
                        }
                    };
                    check = true;
                    _eventDelegate.CheckPointFinishedToSkip?.Invoke(this, result, cleanUp);
                }
            }
        }
    }
}