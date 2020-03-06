using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OMobile.EstimoteUnity;

public abstract class IBeaconHandle
{
    /// <summary>
    /// 欲處理iBeacon的Major
    /// </summary>
    public int Major;

    /// <summary>
    /// 欲處理iBeacon的Minor
    /// </summary>
    public int Minor;

    /// <summary>
    /// 使用的Beacon物件
    /// </summary>
    public EstimoteUnityBeacon beacon;

    public IBeaconHandle(EstimoteUnityBeacon _beacon)
    {
        beacon = _beacon;
        Major = _beacon.Major;
        Minor = _beacon.Minor;

    }

    public void Enter() { }
    public void Exit() { }
    public void Update() { }

    public bool Trigger(EstimoteUnityBeacon _beacon)
    {
        return _beacon.Major == Major && _beacon.Minor == Minor;
    }

}
