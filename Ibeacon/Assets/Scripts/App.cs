using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class App : MonoBehaviour
{
    public IBeaconHandle CurrentHandle;
    private void Start()
    {
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
