using UnityEngine;
using UnityEngine.Events;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MiniJSON;

namespace OMobile.EstimoteUnity
{

	[System.Serializable]
	public class ForegroundScanConfiguration
	{
		/// <summary>
		/// How long to perform BLE scanning. Cannot be less than 200. Android only.
		/// </summary>
		public long ScanPeriodMillis = 1000;

		/// <summary>
		/// How long to wait until a next scan is started. Cannot be less than 0. Android only.
		/// </summary>
		public long WaitTimeMillis = 0;
	}

	[System.Serializable]
	public class BackgroundScanConfiguration
	{
		/// <summary>
		/// How long to perform BLE scanning. Cannot be less than 200. Android only.
		/// </summary>
		public long ScanPeriodMillis = 1000;

		/// <summary>
		/// How long to wait until a next scan is started. Cannot be less than 0. Android only.
		/// </summary>
		public long WaitTimeMillis = 0;
	}

	[Serializable]
	public partial class EstimoteUnity : MonoBehaviour
	{

		#region Events

		[Serializable]
		public class DidRangeBeaconEvent : UnityEvent<List<EstimoteUnityBeacon>>
		{

		}

		[Serializable]
		public class FetchedBeaconCloudDetailsSuccessEvent : UnityEvent<EstimoteUnityBeaconCloudInfo>
		{

		}

		[Serializable]
		public class FetchedBeaconCloudDetailsErrorEvent : UnityEvent<string>
		{

		}

		#endregion

		#region Public Variables

		/// <summary>
		/// The UUID for the beacons you wish to detect. Defaults to Estimote beacons UUID.
		/// </summary>
		public List<string> BeaconRegions = new List<string> ();

		/// <summary>
		/// Whether Estimote Cloud integration is enabled or not.
		/// </summary>
		public bool EnableEstimoteCloud = false;

		/// <summary>
		/// The Estimote App ID
		/// </summary>
		public string EstimoteCloudAppId;

		/// <summary>
		/// Your Estimote App Token
		/// </summary>
		public string EstimoteCloudAppToken;

		/// <summary>
		/// The foreground scan config information. Android only.
		/// </summary>
		public ForegroundScanConfiguration ForegroundScanConfig;

		/// <summary>
		/// The background scan config information. Android only.
		/// </summary>
		public BackgroundScanConfiguration BackgroundScanConfig;

		/// <summary>
		/// An event called when we have initialised the Estimote SDK.
		/// </summary>
		public UnityEvent OnInitEstimoteComplete;

		/// <summary>
		/// An event called when we have initialised the Estimote cloud SDK.
		/// </summary>
		public UnityEvent OnInitEstimoteCloudComplete;

		/// <summary>
		/// An event called when we have started scanning.
		/// </summary>
		public UnityEvent OnStartedScanning;

		/// <summary>
		/// An event called when we have stopped scanning.
		/// </summary>
		public UnityEvent OnStoppedScanning;

		/// <summary>
		/// An event called when beacons have been detected. Passes through the list of beacons detected.
		/// </summary>
		public DidRangeBeaconEvent OnDidRangeBeacons;

		/// <summary>
		/// An event called when we have fetched cloud details for a beacon.
		/// </summary>
		public FetchedBeaconCloudDetailsSuccessEvent OnFetchedBeaconCloudDetailsSuccess;

		/// <summary>
		/// An event called when we failed to get cloud details for a beacon.
		/// </summary>
		public FetchedBeaconCloudDetailsErrorEvent OnFetchedBeaconCloudDetailsError;

		#endregion

		#region Private Variables

		private bool mHasInitialized = false;

		#endregion

		#region Public Methods

		/// <summary>
		/// Initializes the native plugins ready for scanning beacons
		/// </summary>
		public void Initialize ()
		{
			if (mHasInitialized) {
				return;
			}

			if (ForegroundScanConfig.ScanPeriodMillis < 200) {
				ForegroundScanConfig.ScanPeriodMillis = 200;
			}
			if (ForegroundScanConfig.WaitTimeMillis < 0) {
				ForegroundScanConfig.WaitTimeMillis = 0;
			}

			if (BackgroundScanConfig.ScanPeriodMillis < 200) {
				BackgroundScanConfig.ScanPeriodMillis = 200;
			}
			if (BackgroundScanConfig.WaitTimeMillis < 0) {
				BackgroundScanConfig.WaitTimeMillis = 0;
			}

			// Filter out any invalid beacon regions
			List<string> filteredBeaconRegions = new List<string> ();
			foreach (string beaconRegion in BeaconRegions) {
				if (!string.IsNullOrEmpty (beaconRegion)) {
					filteredBeaconRegions.Add (beaconRegion);
				}
			}
#if !UNITY_EDITOR
#if UNITY_IOS
			InitializeIOS (filteredBeaconRegions);
#elif UNITY_ANDROID
			
            InitializeAndroid(filteredBeaconRegions);			
			
#endif
#endif

			mHasInitialized = true;
		}

		/// <summary>
		/// Starts scanning for beacons.
		/// Also initialises the system if required.
		/// </summary>
		public void StartScanning ()
		{
			if (!mHasInitialized) {
				Debug.LogError ("Please call Initialize before calling StartScanning.");
				return;
			}

#if !UNITY_EDITOR						
#if UNITY_IOS
			StartIOSScanning ();
#elif UNITY_ANDROID
            StartAndroidScanning();
#endif
#endif
		}

		/// <summary>
		/// Stops scanning for beacons.
		/// No more updates will come through once this has been called until StartScanning is called again.
		/// </summary>
		public void StopScanning ()
		{
#if !UNITY_EDITOR
#if UNITY_IOS
			StopIOSScanning ();
#elif UNITY_ANDROID
            StopAndroidScanning();
#endif
#endif
		}

		/// <summary>
		/// Gets cloud beacon details for the specified beacon.
		/// </summary>
		/// <param name="beacon">The beacon</param>
		public void GetBeaconCloudDetails (EstimoteUnityBeacon beacon)
		{
			if (!EnableEstimoteCloud) {
				return;
			}
			GetBeaconCloudDetails (beacon.UUID, beacon.Major, beacon.Minor);
		}

		/// <summary>
		/// Gets cloud beacon details for the specified beacon.
		/// </summary>
		/// <param name="beaconUUID">Beacons UUID</param>
		/// <param name="major">Beacons major</param>
		/// <param name="minor">Beacons minor</param>
		public void GetBeaconCloudDetails (string beaconUUID, int major, int minor)
		{
			if (!EnableEstimoteCloud) {
				return;
			}

			if (string.IsNullOrEmpty (beaconUUID)) {
				Debug.LogError ("Cannot fetch beacon cloud details with an invalid UUID.", this.gameObject);
				return;
			}

#if !UNITY_EDITOR
#if UNITY_IOS
			GetBeaconCloudDetailsIOS (beaconUUID, major, minor);
#elif UNITY_ANDROID
            GetBeaconCloudDetailsAndroid(beaconUUID, major, minor);
#endif
#endif
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Called by the native plugin when initialisation of the Estimote SDK has completed.
		/// </summary>
		private void InitEstimoteCompleteCallback ()
		{
			if (OnInitEstimoteComplete != null) {
				OnInitEstimoteComplete.Invoke ();
			}
		}

		/// <summary>
		/// Called by the native plugin when initialisation of the Estimote Cloud SDK has completed.
		/// </summary>
		private void InitEstimoteCloudCompleteCallback ()
		{
			if (OnInitEstimoteCloudComplete != null) {
				OnInitEstimoteCloudComplete.Invoke ();
			}
		}

		/// <summary>
		/// Called by the native plugin when scanning has started.
		/// </summary>
		private void StartedScanningCallback ()
		{
			if (OnStartedScanning != null) {
				OnStartedScanning.Invoke ();
			}
		}

		/// <summary>
		/// Called by the native plugin when scanning has stopped.
		/// </summary>
		private void StoppedScanningCallback ()
		{
			if (OnStoppedScanning != null) {
				OnStoppedScanning.Invoke ();
			}
		}

		/// <summary>
		/// Called by the native plugin when beacons are detected.
		/// </summary>
		/// <param name="beacons">A string representation of the detected beacons</param>
		private void DidRangeBeaconsCallback (string beacons)
		{
			if (!string.IsNullOrEmpty (beacons)) {
				List<EstimoteUnityBeacon> estimoteBeacons = new List<EstimoteUnityBeacon> ();

				var beaconsJsonList = MiniJSON.Json.Deserialize (beacons) as List<object>;
				foreach (object o in beaconsJsonList) {
					Dictionary<string, object> beaconJson = o as Dictionary<string, object>;

					string uuid = (string)beaconJson ["UUID"];
					int major = (int)((long)beaconJson ["Major"]);
					int minor = (int)((long)beaconJson ["Minor"]);
					int range = (int)((long)beaconJson ["BeaconRange"]);
					int strength = (int)((long)beaconJson ["RSSI"]);
					double accuracy = GetDouble (beaconJson ["Accuracy"]);

					EstimoteUnityBeacon estimoteUnityBeacon = new EstimoteUnityBeacon (uuid, major, minor, range, strength, accuracy);
					estimoteBeacons.Add (estimoteUnityBeacon);
				}

				if (OnDidRangeBeacons != null) {
					OnDidRangeBeacons.Invoke (estimoteBeacons);
				}
			} else {
				if (OnDidRangeBeacons != null) {
					OnDidRangeBeacons.Invoke (null);
				}
			}
		}

		/// <summary>
		/// Called by the native plugin when cloud beacon details have been fetched.
		/// </summary>
		/// <param name="beaconInfoString">A string representation of the cloud beacon details</param>
		private void FetchBeaconDetailsSuccessCallback (string beaconInfoString)
		{
			Dictionary<string, object> beaconJson = MiniJSON.Json.Deserialize (beaconInfoString) as Dictionary<string, object>;

			string uuid = (string)beaconJson ["UUID"];
			int major = (int)((long)beaconJson ["Major"]);
			int minor = (int)((long)beaconJson ["Minor"]);
			string beaconName = (string)beaconJson ["BeaconName"];
			string beaconColor = (string)beaconJson ["BeaconColor"];
			double batteryLifeDays = GetDouble (beaconJson ["BatteryLifeInDays"]);
			string macAddress = (string)beaconJson ["MacAddress"];

			EstimoteUnityBeaconCloudInfo beaconInfo = new EstimoteUnityBeaconCloudInfo (uuid, major, minor, beaconName, beaconColor, batteryLifeDays, macAddress);

			if (OnFetchedBeaconCloudDetailsSuccess != null) {
				OnFetchedBeaconCloudDetailsSuccess.Invoke (beaconInfo);
			}
		}

		/// <summary>
		/// Called by the native plugin when there was an error fetching cloud beacon details.
		/// </summary>
		/// <param name="errorBody">The error response</param>
		private void FetchBeaconDetailsFailureCallback (string errorBody)
		{
			if (OnFetchedBeaconCloudDetailsError != null) {
				OnFetchedBeaconCloudDetailsError.Invoke (errorBody);
			}
		}

		/// <summary>
		/// Gets a double in the correct format.
		/// </summary>
		/// <returns>The double.</returns>
		/// <param name="obj">Object.</param>
		private double GetDouble (object obj)
		{
			double val;
			if (obj.GetType () == typeof(Int64)) {
				val = System.Convert.ToDouble (obj);
			} else {
				val = (double)obj;
			}
			return val;
		}

		#endregion

	}
}