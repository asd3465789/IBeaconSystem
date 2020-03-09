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
        beaconManager.HandleUpdate();
    }

    private void HandleDidRangeBeacons(List<EstimoteUnityBeacon> beacons)
    {
        App.frameCount++;
        /*
        // Cache the beacons
        foreach (EstimoteUnityBeacon beacon in beacons)
        {
            int beaconIndex = mBeacons.IndexOf(beacon);
            //-1代表不再已掃描清單中
            if (beaconIndex == -1)
            {
                //不再清單中就加入清單
                mBeacons.Add(beacon);
            }
            else
            {
                //在清單中則取代原資料
                mBeacons[beaconIndex] = beacon;
            }
        }
        */
        mBeacons = beacons;
       ///排序Beacons
        mBeacons.Sort((EstimoteUnityBeacon x, EstimoteUnityBeacon y) => x.Accuracy.CompareTo(y.Accuracy));
        Caching.ClearCache();
    }
    private void HandleFetchedBeaconCloudDetailsSuccess(EstimoteUnityBeaconCloudInfo beaconInfo)
    {
        Debug.Log(beaconInfo.ToString());
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
