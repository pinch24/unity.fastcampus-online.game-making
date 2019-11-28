using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryControl : MonoBehaviour
{
    private void OnGUI()
    {
        float CenterX = Screen.width * 0.5f;
        float CenterY = Screen.height * 0.5f;
        Rect ButtonRect = new Rect(CenterX - 100.0f, CenterY - 100.0f, 200.0f, 200.0f);

        if (GUI.Button(ButtonRect, "You Win!") == true)
        {
            SceneManager.LoadScene("Level1");
        }
    }

    void Update()
    {
        if (MainControl.Score > 500)
        {
            MainControl.Score = 0;

            UnityEngine.SceneManagement.SceneManager.LoadScene("Victory");
        }
    }
}
