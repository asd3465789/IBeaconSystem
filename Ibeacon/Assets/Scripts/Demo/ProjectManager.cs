using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectManager : MonoBehaviour
{
    //private bool pauseState = false;
    //private bool focusState = false;
    private Database database;

    public bool exitApp = false;
    public bool exitUpdateData=false;
    private int pressCount = 0;

    private void Start()
    {
        database = FindObjectOfType<Database>();
        exitApp = false;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)&&pressCount<1)
        {
            pressCount++;
            exitUpdateData = true;
        }
       

        if (database.updateFinish && exitApp)
        {
            database.status_Text.text = "Staut:"+database.updateFinish+"離開";
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            //Application.Quit();
        }

    }
    
}
