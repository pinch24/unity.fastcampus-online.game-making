using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainControl : MonoBehaviour
{
    static public int Score = 0;
    static public int Life = 3;

    public GUISkin mySkin = null;

    private void OnGUI()
    {
        GUI.skin = mySkin;

        Rect labelRect1 = new Rect(10.0f, 10.0f, 400.0f, 100.0f);
        GUI.Label(labelRect1, "SCORE: " + MainControl.Score);
        
        Rect labelRect2 = new Rect(10.0f, 104.0f, 400.0f, 100.0f);
        GUI.Label(labelRect2, "Life: " + MainControl.Life);
    }

    void Update()
    {
        
    }
}
