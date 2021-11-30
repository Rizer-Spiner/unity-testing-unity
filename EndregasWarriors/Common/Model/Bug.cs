using System;

namespace UnityUXTesting.EndregasWarriors.Common.Model
{
    [Serializable]
    public class Bug
    {
        public string bugName { get; set; }

        public string bugDescription { get; set; }
        
        public TimeInterval timeVideoReference { get; set; }
    }
}