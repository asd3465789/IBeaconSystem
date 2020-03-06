using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace OMobile.EstimoteUnity
{
	public partial class EstimoteUnity : MonoBehaviour
	{
		#if UNITY_ANDROID

		#region Private Variables

		private AndroidJavaObject mActivityContext;
		private AndroidJavaObject mEstimoteUnityAndroidPlugin;

		#endregion

		#region Private Methods

		private void InitializeAndroid (List<string> beaconRegions)
		{
			using (AndroidJavaClass unityPlayerClass = new AndroidJavaClass ("com.unity3d.player.UnityPlayer")) {
				mActivityContext = unityPlayerClass.GetStatic<AndroidJavaObject> ("currentActivity");
			}
			using (AndroidJavaClass estimoteUnityAndroidPluginClass = new AndroidJavaClass ("uk.co.omobile.estimoteunityandroidplugin.EstimoteUnityAndroidPlugin")) {
				mEstimoteUnityAndroidPlugin = estimoteUnityAndroidPluginClass.CallStatic<AndroidJavaObject> ("Instance");
				mEstimoteUnityAndroidPlugin.Call ("InitEstimote", this.gameObject.name, mActivityContext, beaconRegions.ToArray (), ForegroundScanConfig.ScanPeriodMillis, ForegroundScanConfig.WaitTimeMillis, BackgroundScanConfig.ScanPeriodMillis, BackgroundScanConfig.WaitTimeMillis);

				if (EnableEstimoteCloud) {
					mEstimoteUnityAndroidPlugin.Call ("InitEstimoteCloud", EstimoteCloudAppId.Trim (), EstimoteCloudAppToken.Trim ());
				}
			}
		}

		private void StartAndroidScanning ()
		{
			mEstimoteUnityAndroidPlugin.Call ("StartScanning");
		}

		private void StopAndroidScanning ()
		{
			if (!mHasInitialized) {
				return;
			}
			mEstimoteUnityAndroidPlugin.Call ("StopScanning");
		}

		private void GetBeaconCloudDetailsAndroid (string beaconUUID, int major, int minor)
		{
			mEstimoteUnityAndroidPlugin.Call ("GetBeaconCloudDetails", beaconUUID, major, minor);
		}

		#endregion

		#endif
	}
}