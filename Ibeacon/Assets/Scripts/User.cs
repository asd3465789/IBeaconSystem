using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OMobile.EstimoteUnity;

public class User : MonoBehaviour
{
    public IBeaconHandle CurrentHandle;
    private void Start()
    {
        CurrentHandle = new NoneHandle(new EstimoteUnityBeacon(" ", 0, 0, 0, 0, 0));
        CurrentHandle.Enter();
    }

    private void Update()
    {
        CurrentHandle.Update();
    }

    private void ChangeHandle(IBeaconHandle Nexthadle)
    {
        CurrentHandle.Exit();
        CurrentHandle = Nexthadle;
        CurrentHandle.Enter();
    }
}
