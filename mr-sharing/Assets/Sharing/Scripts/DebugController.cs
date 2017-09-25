using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugController : MonoBehaviour {
    TextMesh _m;

	// Use this for initialization
	void Start () {
        _m = gameObject.GetComponentInChildren<TextMesh>();	
	}

    private void OnEnable()
    {
        Application.logMessageReceived += Application_logMessageReceived;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= Application_logMessageReceived;
    }

    private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
    {
        if (!condition.Contains("MipMap"))
        {
            _m.text += condition + "\n";
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
