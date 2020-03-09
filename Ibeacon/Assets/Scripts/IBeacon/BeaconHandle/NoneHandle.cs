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
        return AppManager.Instance.mBeacons.Count==0;
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
public class Positioning : IHandle
{
    public Positioning()
    {
        name = "Positioning";
    }

    public override void Enter()
    {

    }
    public override void Exit()
    {

    }
    public override void Update()
    {
        AppManager.Instance._UIManager.testText.text = AppManager.Instance.mBeacons[0].Accuracy.ToString("0.00");
        AppManager.Instance._UIManager.testText2.text = AppManager.Instance.mBeacons[1].Accuracy.ToString("0.00");
    }

    public override bool Trigger()
    {
        return AppManager.Instance.mBeacons.Count == 2;
    }
}

public class test3 : IHandle
{
    public test3()
    {
        name = "test3";
    }

    public override void Enter()
    {

    }
    public override void Exit()
    {

    }
    public override void Update()
    {
        AppManager.Instance._UIManager.beacon_count.text = AppManager.Instance.mBeacons.Count.ToString();
        AppManager.Instance._UIManager.testText.text = AppManager.Instance.mBeacons[0].Minor + " " + AppManager.Instance.mBeacons[0].Accuracy.ToString("0.00");
        if(AppManager.Instance.mBeacons.Count>1)
        AppManager.Instance._UIManager.testText2.text = AppManager.Instance.mBeacons[1].Minor + " " + AppManager.Instance.mBeacons[1].Accuracy.ToString("0.00");
    }

    public override bool Trigger()
    {
        return AppManager.Instance.mBeacons.Count >0;
    }
}