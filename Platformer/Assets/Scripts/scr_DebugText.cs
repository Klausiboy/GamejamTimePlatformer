using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scr_DebugText : MonoBehaviour
{
    public scr_GameMan gMan;
    public Text txt_Time, txt_Score;

    // Update is called once per frame
    void Update()
    {
        txt_Time.text = gMan.timeLeft.ToString("0");
        txt_Score.text = gMan.score.ToString("0");
    }
}
