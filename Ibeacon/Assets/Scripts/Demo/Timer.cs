using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using System;
public class Timer : MonoBehaviour
{
    public Text TimerText;
    public int currentTime;
    public bool continueTime=true;
   
    public IEnumerator timer()
    {
        while (continueTime)
        {
            currentTime++;
            TimerText.text = "StayTime:" + currentTime;
            yield return new WaitForSeconds(1f);
        }
    }
   
}
