using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityUXTesting.EndregasWarriors.BugReporting.Monobehaviour;
using UnityUXTesting.EndregasWarriors.Common;
using UnityUXTesting.EndregasWarriors.Common.Model;
using UnityUXTesting.EndregasWarriors.PlayRunReporting.Checkpoints.UI;

namespace UnityUXTesting.EndregasWarriors.PlayRunReporting.Checkpoints
{
    public class CheckPointsManager : MonoBehaviour
    {
        [SerializeField] private List<CheckPoint> levelCheckPoints;
        [SerializeField] private Canvas canvas;
        [SerializeField] private Button skipButton;
        [SerializeField] private GameObject ynPrefab;
        private Dictionary<string, CheckPointResult> resultsDict;

        private CheckPoint currentCheckPoint;
        private CheckPoint nextCheckPoint;

        public static EventDelegate _eventDelegate = new EventDelegate();

        private void Start()
        {
            CheckPoint._eventDelegate.CheckPointStarted += MarkCurrentCheckpoint;
            CheckPoint._eventDelegate.CheckPointFinished += CheckPointFinished;
            CheckPoint._eventDelegate.CheckPointFinishedToSkip += CheckPointFinishedToSkip;

            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void CheckPointFinished(CheckPoint self, CheckPointResult result)
        {
            resultsDict.Add(self.name, result);
        }
        
        private void CheckPointFinishedToSkip(CheckPoint self, CheckPointResult result, UnityEvent cleanup)
        {
            string text = "We noticed you are having trouble finishing this part of the game. Would you like to skip?";
            StartCoroutine(ConfirmSkipping(self, result, cleanup, text));
        }

        
        private void MarkCurrentCheckpoint(CheckPoint self)
        {
            currentCheckPoint = self;

            var nextIndex = levelCheckPoints.FindIndex(point => point.name.Equals(self.name)) + 1;
            if (nextIndex < levelCheckPoints.Count)
                nextCheckPoint = levelCheckPoints[nextIndex];
            else
            {
                skipButton.interactable = false;
                nextCheckPoint = null;
            }
        }

        public void SkipCheckPoint()
        {
            StartCoroutine(ConfirmPlayerSkip(currentCheckPoint));
        }

        private IEnumerator ConfirmPlayerSkip(CheckPoint self)
        {
            GameObject dialogObj = Instantiate(ynPrefab, canvas.transform);

            dialogObj.transform.SetParent(canvas.transform);
            YnDialog dialog = dialogObj.GetComponent<YnDialog>();

            dialog.setTitle("Are you sure you want to skip this part of the game?");

            while (dialog.status == YnDialog.STATUS.WAITING)
            {
                yield return null; // wait
            }

            if (dialog.status == YnDialog.STATUS.YES)
            {
                resultsDict.Add(self.name, new CheckPointResult
                {
                    name = self.name,
                    actualPlayInterval = new TimeInterval
                    {
                        start = self.startCheckPointTime,
                        end = BugReportingManager._instance.countedTime
                    },
                    result = GameState.PLAYER_SKIPPED
                });

                self.check = true;

                currentCheckPoint.trackedObject.transform.position = nextCheckPoint.startMark.transform.position;
                MarkCurrentCheckpoint(nextCheckPoint);

                self.cleanUp?.Invoke();
            }

            Destroy(dialog.gameObject);
        }

        private IEnumerator ConfirmSkipping(CheckPoint self, CheckPointResult result, UnityEvent cleanup, string text)
        {
            GameObject dialogObj = Instantiate(ynPrefab, canvas.transform);

            dialogObj.transform.SetParent(canvas.transform);
            YnDialog dialog = dialogObj.GetComponent<YnDialog>();

            dialog.setTitle(text);

            while (dialog.status == YnDialog.STATUS.WAITING)
            {
                yield return null; // wait
            }

            if (dialog.status == YnDialog.STATUS.YES)
            {
                result.result = GameState.SKIPPED;
                resultsDict.Add(self.name, result);


                currentCheckPoint.trackedObject.transform.position = nextCheckPoint.startMark.transform.position;
                MarkCurrentCheckpoint(nextCheckPoint);

                cleanup?.Invoke();
            }
            else
            {
                //reset checkpoint and wait to see if player can finish the checkpoint
                self.startCheckPointTime = BugReportingManager._instance.countedTime;
                self.check = false;
            }

            Destroy(dialog.gameObject);
        }
        
        private void OnSceneUnloaded(Scene arg0)
        {
            List<CheckPointResult> results = new List<CheckPointResult>();
            results.AddRange(resultsDict.Values);

            foreach (var checkPoint in levelCheckPoints)
            {
                if (!resultsDict.ContainsKey(checkPoint.name))
                {
                    results.Add(new CheckPointResult
                    {
                        name = checkPoint.name,
                        actualPlayInterval = new TimeInterval
                        {
                            start = 0,
                            end = 0
                        },
                        result = GameState.NOT_STARTED
                    });
                }
            }

            _eventDelegate.SaveCheckPointsForLevel(results.ToArray());
        }
    }
}