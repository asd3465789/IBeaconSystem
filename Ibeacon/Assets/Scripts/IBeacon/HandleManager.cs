using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleManager 
{
    public List<IHandle> Handles;
    public IHandle CurrentHandle;

    public void Init()
    {
        Handles = new List<IHandle>();
        Handles.Add(new test3());
       // Handles.Add(new TestHandle());
      //  Handles.Add(new Test2Handle());
        CurrentHandle = new NoneHandle();
    }

    public void HandleUpdate()
    {
        foreach (IHandle value in Handles)
        {
            if (value.Trigger() && value != CurrentHandle)
            {
                SwitchHandle(value);
                break;
            }
        }
        CurrentHandle.Update();
    }


    public void SwitchHandle(IHandle Nexthadle)
    {
        CurrentHandle.Exit();
        CurrentHandle = Nexthadle;
        Nexthadle.Enter();
    }

}
