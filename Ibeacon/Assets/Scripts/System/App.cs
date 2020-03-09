using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class App : MonoBehaviour
{
    float timer = 0;
    public  static int  frameCount=0;
    private void Start()
    {
        AppManager.Instance.Init();
    }

    private void Update()
    {
        AppManager.Instance.AppManagerUpdate();
        timer += Time.deltaTime;
        AppManager.Instance._UIManager.timer.text = timer.ToString("0.00");
        if (timer >= 1)
        {
            timer = 0;
            AppManager.Instance._UIManager.fps.text = frameCount.ToString();
            frameCount = 0;
        }
    }
}
