using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoggerUser : MonoBehaviour
{
    private LoggerScriptObj _obj;

    public TheOtherLoggerUser OtherLoggerUser;
    // Start is called before the first frame update
    void Start()
    {
        _obj = ScriptableObject.CreateInstance<LoggerScriptObj>();
        StartCoroutine(DestroyMe());
    }

    private IEnumerator DestroyMe()
    {
        yield return new WaitForSeconds(10);
        OtherLoggerUser._loggerScriptObj = _obj;
        Destroy(this);
    }

    // Update is called once per frame
    void Update()
    {
        _obj.Log();
    }
}
