using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OMobile.EstimoteUnity;

public abstract class IHandle
{
    public HandleTarget target;
    public string name; 


    public IHandle()
    {     

    }

    public abstract void Enter();
    public abstract void Exit();
    public abstract void Update();

    public abstract bool Trigger();
}

public enum HandleTarget
{

}
