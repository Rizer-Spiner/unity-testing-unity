using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityUXTesting.EndregasWarriors.Common;
using UnityUXTesting.EndregasWarriors.Common.Model;

namespace UnityUXTesting.EndregasWarriors.BugReporting.Monobehaviour
{
    public class BugReportingManager : BugReportingManagerBase
    {
        public TMP_InputField name;
        public TMP_InputField description;
        public TMP_InputField numberOfSec;


        public List<Bug> bugs;


        public void AddBug()
        {
            Bug newBug = new Bug()
            {
                bugDescription = description.text,
                bugName = name.text,
                timeVideoReference = Utils.MathUtils.CreateTimeInterval(int.Parse(numberOfSec.text), countedTime)
            };

            bugs.Add(newBug);

            description.text = "";
            name.text = "";
            numberOfSec.text = "";
            
            Debug.Log(newBug.timeVideoReference.start);
        }
        
    }
}