using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

namespace OMobile.EstimoteUnity.Demo
{
	public class EstimoteUnityDemo : MonoBehaviour
	{

		#region Public Variables

		[Header ("Setup")]
		public EstimoteUnity _EstimoteUnity;

		[Header ("UI")]
		public GameObject _StartScanInfoPanel;
		public GameObject _BeaconRowPrefab;
		public RectTransform _BeaconScrollRectContent;

		[Header ("Beacon Info UI")]
		public GameObject _BeaconInfoPanel;
		public GameObject _BeaconInfoPanelContent;
		public GameObject _BeaconInfoPanelStatus;
		public Text _BeaconInfoPanelName;
		public Text _BeaconInfoPanelUUID;
		public Text _BeaconInfoPanelMajorMinor;
		public Text _BeaconInfoPanelBeaconColor;
		public Text _BeaconInfoPanelBatteryLife;

		[Header ("Beacon Config")]
		public float _CheckOutOfRangeTimer = 2.0f;
		public float _LastSeenSeconds = 10f;

		#endregion

		#region Private Variables

		private List<EstimoteUnityBeacon> mBeacons = new List<EstimoteUnityBeacon> ();
		private float mOutOfRangeTimer = 0f;

		#endregion

		#region Unity Methods

		private void Start ()
		{
			_EstimoteUnity.OnDidRangeBeacons.AddListener (HandleDidRangeBeacons);
			_EstimoteUnity.OnFetchedBeaconCloudDetailsSuccess.AddListener (HandleFetchedBeaconCloudDetailsSuccess);
			_EstimoteUnity.OnFetchedBeaconCloudDetailsError.AddListener (HandleFetchedBeaconCloudDetailsError);
			_EstimoteUnity.OnStoppedScanning.AddListener (HandleStoppedScanning);

			_EstimoteUnity.Initialize ();

			_StartScanInfoPanel.SetActive (true);
			_BeaconInfoPanel.SetActive (false);
			ClearBeaconUIList ();
		}

		private void Update ()
		{
			if (mOutOfRangeTimer < 0f) {
				RemoveOutOfRangeBeacons ();
				mOutOfRangeTimer = _CheckOutOfRangeTimer;
			}
			mOutOfRangeTimer -= Time.deltaTime;
		}

		#endregion

		#region Public Methods

		public void StartScanning ()
		{
			_EstimoteUnity.StartScanning ();
			_StartScanInfoPanel.SetActive (false);		

		}

		public void StopScanning ()
		{
			_EstimoteUnity.StopScanning ();
		}

		#endregion

		#region Private Methods

		private void HandleDidRangeBeacons (List<EstimoteUnityBeacon> beacons)
		{
			// Cache the beacons
			foreach (EstimoteUnityBeacon beacon in beacons) {
				int beaconIndex = mBeacons.IndexOf (beacon);
				if (beaconIndex == -1) {
					mBeacons.Add (beacon);
				} else {
					mBeacons [beaconIndex] = beacon;
				}
			}

			// Sort the beacons by distance
			mBeacons.Sort ((EstimoteUnityBeacon x, EstimoteUnityBeacon y) => x.Accuracy.CompareTo (y.Accuracy));

			// Clean the list
			ClearBeaconUIList ();

			// Add rows to the list
			if (mBeacons != null && mBeacons.Count > 0) {
				foreach (EstimoteUnityBeacon beacon in mBeacons) {
					GameObject go = Instantiate (_BeaconRowPrefab, _BeaconScrollRectContent);
					go.transform.localScale = Vector3.one;
					go.GetComponent<EstimoteUnityBeaconUIController> ().Setup (beacon, _EstimoteUnity);
				}
			}
		}

		private void HandleFetchedBeaconCloudDetailsSuccess (EstimoteUnityBeaconCloudInfo beaconInfo)
		{
			Debug.Log (beaconInfo.ToString ());

			_BeaconInfoPanelName.text = beaconInfo.BeaconName;
			_BeaconInfoPanelUUID.text = "UUID: " + beaconInfo.UUID;
			_BeaconInfoPanelMajorMinor.text = "Major / Minor: " + beaconInfo.Major + ":" + beaconInfo.Minor;
			_BeaconInfoPanelBeaconColor.text = "Beacon Color: " + beaconInfo.Color;
			_BeaconInfoPanelBatteryLife.text = "Battery Life (Days): " + beaconInfo.BatteryLifeDays.ToString ();

			_BeaconInfoPanel.SetActive (true);
			_BeaconInfoPanelStatus.SetActive (false);
			_BeaconInfoPanelContent.SetActive (true);
		}

		private void HandleFetchedBeaconCloudDetailsError (string errorBody)
		{
			Debug.Log ("HandleFetchedBeaconCloudDetailsError: " + errorBody);
		}

		private void HandleStoppedScanning ()
		{
			ClearBeaconUIList ();
			_BeaconInfoPanel.SetActive (false);
			_StartScanInfoPanel.SetActive (true);
		}

		private void RemoveOutOfRangeBeacons ()
		{
			for (int i = 0; i < mBeacons.Count; i++) {
				EstimoteUnityBeacon beacon = mBeacons [i];
				if (beacon != null && beacon.LastSeen.AddSeconds (_LastSeenSeconds) < System.DateTime.Now) {
					mBeacons.RemoveAt (i);
				}
			}
		}

		private void ClearBeaconUIList ()
		{
			for (int i = 0; i < _BeaconScrollRectContent.childCount; i++) {
				Destroy (_BeaconScrollRectContent.GetChild (i).gameObject);
			}
		}

		private void ShowBeaconInfoPanel ()
		{
			_BeaconInfoPanel.SetActive (true);
			_BeaconInfoPanelContent.SetActive (false);
			_BeaconInfoPanelStatus.SetActive (true);
		}

		#endregion

	}
}