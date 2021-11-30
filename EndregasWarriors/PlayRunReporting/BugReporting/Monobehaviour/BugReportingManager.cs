using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityUXTesting.EndregasWarriors.Common;
using UnityUXTesting.EndregasWarriors.Common.Model;

namespace UnityUXTesting.EndregasWarriors.BugReporting.Monobehaviour
{
    public class BugReportingManager : BugReportingManagerBase
    {
        public TMP_InputField bugName;
        public TMP_InputField description;
        public TMP_InputField numberOfSec;

        public GameObject reportMenu;
        public GameObject warningLabel;

        public static BugReportingManager _instance;

        [NonSerialized] public List<Bug> bugs = new List<Bug>();

        protected override void Awake()
        {
            DontDestroyOnLoad(this);
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(this);
                return;
            }

            base.Awake();
        }

        protected override void Start()
        {
            bugName.onSelect.AddListener((arg0 => warningLabel.SetActive(false)));
            description.onSelect.AddListener((arg0 => warningLabel.SetActive(false)));
            numberOfSec.onSelect.AddListener((arg0 => warningLabel.SetActive(false)));
            base.Start();
        }


        public void AddBug()
        {
            if (bugName.text.Equals("") || description.text.Equals(""))
            {
                warningLabel.SetActive(true);
            }
            else
            {
                Bug newBug = new Bug()
                {
                    bugDescription = description.text,
                    bugName = bugName.text,
                    timeVideoReference = numberOfSec.text.Equals("")
                        ? Utils.MathUtils.CreateTimeInterval(0, countedTime)
                        : Utils.MathUtils.CreateTimeInterval(int.Parse(numberOfSec.text), countedTime)
                };

                bugs.Add(newBug);
                reportMenu.SetActive(false);
                warningLabel.SetActive(false);
                ResetInputValue();
            }
        }

        public void ResetInputValue()
        {
            description.text = "";
            bugName.text = "";
            numberOfSec.text = "";
        }
    }
}