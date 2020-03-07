using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OMobile.EstimoteUnity;
using PugTools;
using UnityEngine.UI;

public class AppManager : Singleton<AppManager>
{
    private HandleManager beaconManager;
    public EstimoteUnity _EstimoteUnity;
    public UIManager _UIManager;
    public List<EstimoteUnityBeacon> mBeacons = new List<EstimoteUnityBeacon>();

    public void Init()
    {
        beaconManager = new HandleManager();
        beaconManager.Init();
        _EstimoteUnity = FindObjectOfType<EstimoteUnity>();
        _UIManager = FindObjectOfType<UIManager>();

        _EstimoteUnity.OnDidRangeBeacons.AddListener(HandleDidRangeBeacons);
        _EstimoteUnity.OnFetchedBeaconCloudDetailsSuccess.AddListener(HandleFetchedBeaconCloudDetailsSuccess);
        _EstimoteUnity.OnFetchedBeaconCloudDetailsError.AddListener(HandleFetchedBeaconCloudDetailsError);
        _EstimoteUnity.Initialize();
        StartScanning();
    }

    public void AppManagerUpdate()
    {
        if (mBeacons.Count > 0)
            beaconManager.HandleUpdate();


    }

    private void HandleDidRangeBeacons(List<EstimoteUnityBeacon> beacons)
    {
        // Cache the beacons
        foreach (EstimoteUnityBeacon beacon in beacons)
        {
            int beaconIndex = mBeacons.IndexOf(beacon);
            if (beaconIndex == -1)
            {
                mBeacons.Add(beacon);
            }
            else
            {
                mBeacons[beaconIndex] = beacon;
            }
        }

       ///排序Beacons
        mBeacons.Sort((EstimoteUnityBeacon x, EstimoteUnityBeacon y) => x.Accuracy.CompareTo(y.Accuracy));

    }
    private void HandleFetchedBeaconCloudDetailsSuccess(EstimoteUnityBeaconCloudInfo beaconInfo)
    {
        Debug.Log(beaconInfo.ToString());

        //_BeaconInfoPanelName.text = beaconInfo.BeaconName;
        //_BeaconInfoPanelUUID.text = "UUID: " + beaconInfo.UUID;
        //_BeaconInfoPanelMajorMinor.text = "Major / Minor: " + beaconInfo.Major + ":" + beaconInfo.Minor;
        //_BeaconInfoPanelBeaconColor.text = "Beacon Color: " + beaconInfo.Color;
        //_BeaconInfoPanelBatteryLife.text = "Battery Life (Days): " + beaconInfo.BatteryLifeDays.ToString();

        //_BeaconInfoPanel.SetActive(true);
        //_BeaconInfoPanelStatus.SetActive(false);
        //_BeaconInfoPanelContent.SetActive(true);
    }

    private void HandleFetchedBeaconCloudDetailsError(string errorBody)
    {
        Debug.Log("HandleFetchedBeaconCloudDetailsError: " + errorBody);
    }

    public void StartScanning()
    {
        _EstimoteUnity.StartScanning();

    }

    public void StopScanning()
    {
        _EstimoteUnity.StopScanning();
    }
}
