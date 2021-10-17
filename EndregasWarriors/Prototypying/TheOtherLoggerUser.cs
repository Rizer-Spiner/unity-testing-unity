using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheOtherLoggerUser : MonoBehaviour
{
    public LoggerUser user;
    public LoggerScriptObj _loggerScriptObj;
    private bool locker;
    
    

    // Update is called once per frame
    void Update()
    {
        if (user == null && !locker)
        {
            _loggerScriptObj.LogTimes();
            locker = true;

        }
    }
}
