using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class LoggerScriptObj : ScriptableObject
{
    public int times = 0;


    public void Log()
    {
        Debug.Log("Logged by ScriptableObject "+ times + " times");
        times++;
    }

    public void LogTimes()
    {
        Debug.Log("Times: " + times);
    }
    
}
