using System;
using UnityEngine;

namespace UnityUXTesting.EndregasWarriors.Common.Model
{
    [Serializable]
    public class CheckPointResult
    {
        //set by user
        public string name { get; set; }

        //computed
        public GameState result { get; set; }
        public TimeInterval actualPlayInterval { get; set; }
        
    }
}