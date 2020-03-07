using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OMobile.EstimoteUnity;

public class NoneHandle : IHandle
{  
    public NoneHandle()
    {
        name = "None";
    }

    public override void Enter()
    {
      
    }
    public override void Exit()
    {

    }
    public override void Update()
    {

    }

    public override bool Trigger()
    {
        return false;
    }
}


public class TestHandle : IHandle
{
    public int Major = 1;
    public int Minor = 550;
    public TestHandle()
    {
        name = "test";
    }

    public override void Enter() 
    {
        AppManager.Instance._UIManager.testText.text = "Major:1 Minor:500";
    }
    public override void Exit()
    {

    }
    public override void Update()
    {

    }

    public override bool Trigger()
    {
        return AppManager.Instance.mBeacons[0].Major == Major && AppManager.Instance.mBeacons[0].Minor == Minor;
    }
}
public class Test2Handle : IHandle
{
    public int Major = 1;
    public int Minor = 547;
    public Test2Handle()
    {
        name = "test2";
    }

    public override void Enter()
    {
        AppManager.Instance._UIManager.testText.text = "Major:1 Minor:547";
    }
    public override void Exit()
    {

    }
    public override void Update()
    {

    }

    public override bool Trigger()
    {
        return AppManager.Instance.mBeacons[0].Major == Major && AppManager.Instance.mBeacons[0].Minor == Minor;
    }
}
