using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;
using System;

namespace OMobile.EstimoteUnity.Demo
{
    public class Ibeacon : MonoBehaviour
    {

        #region Public Variables

        [Header("Setup")]
        public EstimoteUnity _EstimoteUnity;

        //[Header("UI")]
        //public GameObject _StartScanInfoPanel;
        //public GameObject _BeaconRowPrefab;
        //public RectTransform _BeaconScrollRectContent;

        [Header("Beacon Info UI")]
        //public GameObject _BeaconInfoPanel;
        //public GameObject _BeaconInfoPanelContent;
        //public GameObject _BeaconInfoPanelStatus;
        //public Text _BeaconInfoPanelName;
        public Text _BeaconUUID;
        public Text _BeaconMajorMinor;
        public Text _BeaconDis;
        public Text _BeaconRSSI;
        public Text LastSeen_Text;
        //public Text _BeaconInfoPanelBeaconColor;
        //public Text _BeaconInfoPanelBatteryLife;

        [Header("Beacon Config")]
        public float _CheckOutOfRangeTimer = 2.0f;
        public float _LastSeenSeconds = 10f;

        #endregion

        #region Private Variables

        private List<EstimoteUnityBeacon> mBeacons = new List<EstimoteUnityBeacon>();
        private float mOutOfRangeTimer = 0f;

        #endregion

        #region TimeControl
        private Timer timer;
        private bool firstCount = true;
        private int userStayTime;

        #endregion

        #region Database
        private int previousMinor = 0;
        public Text previousMinor_Text;
        //private int ibeaconSort = 0;
        private string nowDate;
        private Database dataBase;
        #endregion

        #region ProjectManager
        private ProjectManager projectManager;
        private bool stopApp;
        #endregion

        #region Unity Methods

        private void Awake()
        {
            dataBase = FindObjectOfType<Database>();
        }
        private void Start()
        {
            projectManager = FindObjectOfType<ProjectManager>();
            timer = FindObjectOfType<Timer>();
            _EstimoteUnity = FindObjectOfType<EstimoteUnity>();
            _EstimoteUnity.OnDidRangeBeacons.AddListener(HandleDidRangeBeacons);
            _EstimoteUnity.OnFetchedBeaconCloudDetailsSuccess.AddListener(HandleFetchedBeaconCloudDetailsSuccess);
            _EstimoteUnity.OnFetchedBeaconCloudDetailsError.AddListener(HandleFetchedBeaconCloudDetailsError);


            _EstimoteUnity.Initialize();

        }

        private void OnApplicationPause(bool isPause)
        {

            if (isPause)
            {
                stopApp = true;
                SendDataToDatabase(mBeacons[0].Minor, nowDate);
                Debug.Log("縮到主頁");
            }
            else
            {
                dataBase.ibeaconSort--;
                stopApp = false;
                Debug.Log("回到app");
            }
        }

        private void Update()
        {
            if (mOutOfRangeTimer < 0f)
            {
                RemoveOutOfRangeBeacons();
                mOutOfRangeTimer = _CheckOutOfRangeTimer;
            }
            mOutOfRangeTimer -= Time.deltaTime;


        }

        #endregion

        #region Public Methods

        public void StartScanning()
        {
            _EstimoteUnity.StartScanning();
            
        }

        public void StopScanning()
        {
            _EstimoteUnity.StopScanning();

            timer.continueTime = false;
            timer.currentTime = 0;
            firstCount = true;
        }

        #endregion

        #region Private Methods

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

            // Sort the beacons by distance
            mBeacons.Sort((EstimoteUnityBeacon x, EstimoteUnityBeacon y) => x.Accuracy.CompareTo(y.Accuracy));

            LastSeen_Text.text = " LastSeen:" + mBeacons[0].LastSeen;
            _BeaconUUID.text = "BeaconUUID: " + mBeacons[0].UUID;
            _BeaconMajorMinor.text = "BeaconMajor/Minor:" + mBeacons[0].Major + "/" + mBeacons[0].Minor;
            _BeaconDis.text = "BeaconDis:" + mBeacons[0].Accuracy;
            _BeaconRSSI.text = "BeaconRSSI" + mBeacons[0].RSSI;
            dataBase.status_Text.text = "Staus:搜尋Beacons";
            HandleBeaconsData();
        }

        private void SendDataToDatabase(int beaconMinor, string date)
        {

            if (projectManager.exitUpdateData)
            {
                projectManager.exitUpdateData = false;
                projectManager.exitApp = true;
            }

            userStayTime = timer.currentTime;
            dataBase.status_Text.text = "Staus:上傳資料中";
            dataBase.UpdateData(beaconMinor, userStayTime, date);
        }


        private void HandleBeaconsData()
        {

            if (projectManager.exitUpdateData)
            {
                SendDataToDatabase(mBeacons[0].Minor, nowDate);
            }

            //紀錄目前的Minor，如果Minor改變表示移位置，就把上一個Minor資料update到資料庫
            if (previousMinor == mBeacons[0].Minor)
            {
                return;
            }

            if (firstCount)
            {
                nowDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                previousMinor = mBeacons[0].Minor;
                timer.continueTime = true;
                timer.StartCoroutine("timer");
                previousMinor_Text.text = "PreviousMinor:" + mBeacons[0].Minor;
                firstCount = false;
            }
            else if (previousMinor != mBeacons[0].Minor && previousMinor != 0)
            {
                previousMinor_Text.text = "PreviousMinor:" + previousMinor;
                dataBase.status_Text.text = "Staus:上傳資料中";
                SendDataToDatabase(previousMinor, nowDate);
                previousMinor = mBeacons[0].Minor;
                timer.currentTime = 0;
                nowDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
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



        private void RemoveOutOfRangeBeacons()
        {
            for (int i = 0; i < mBeacons.Count; i++)
            {
                EstimoteUnityBeacon beacon = mBeacons[i];
                if (beacon != null && beacon.LastSeen.AddSeconds(_LastSeenSeconds) < System.DateTime.Now)
                {
                    if (i == ( mBeacons.Count - 1 ))
                    {
                        SendDataToDatabase(mBeacons[0].Minor, nowDate);
                        timer.currentTime = 0;
                        firstCount = true;
                        previousMinor = 0;
                        timer.continueTime = false;
                        dataBase.status_Text.text = "Staus:沒有Beacon";
                        LastSeen_Text.text = " LastSeen:";
                        _BeaconUUID.text = "BeaconUUID:";
                        _BeaconMajorMinor.text = "BeaconMajor/Minor:";
                        _BeaconDis.text = "BeaconDis:";
                        _BeaconRSSI.text = "BeaconRSSI";
                    }
                    mBeacons.RemoveAt(i);
                }
            }
        }


        #endregion

    }
}