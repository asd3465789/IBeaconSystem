using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

namespace OMobile.EstimoteUnity
{
	public partial class EstimoteUnity : MonoBehaviour
	{

		#if UNITY_IOS

		#region Native Methods

		[DllImport ("__Internal")]
		private static extern void IOS_Initialize (string unityGameObjectName, string[] beaconRegions, int beaconRegionsLength);

		[DllImport ("__Internal")]
		private static extern void IOS_InitializeEstimoteCloud (string appId, string appToken);

		[DllImport ("__Internal")]
		private static extern void IOS_StartEstimoteScanning ();

		[DllImport ("__Internal")]
		private static extern void IOS_StopEstimoteScanning ();

		[DllImport ("__Internal")]
		private static extern int IOS_CheckDeviceSupportsBeacons ();

		[DllImport ("__Internal")]
		private static extern int IOS_GetBeaconCloudDetails (string beaconUUID, int major, int minor);

		#endregion

		#region Public Static Methods

		public bool CheckIOSDeviceSupported ()
		{
			return IOS_CheckDeviceSupportsBeacons () == 1;
		}

		#endregion

		#region Private Methods

		private void InitializeIOS (List<string> beaconRegions)
		{
			IOS_Initialize (this.gameObject.name, beaconRegions.ToArray (), beaconRegions.Count);

			if (EnableEstimoteCloud) {
				IOS_InitializeEstimoteCloud (EstimoteCloudAppId, EstimoteCloudAppToken);
			}
		}

		private void StartIOSScanning ()
		{
			if (!CheckIOSDeviceSupported ()) {
				return;
			}
			IOS_StartEstimoteScanning ();
		}

		private void StopIOSScanning ()
		{
			IOS_StopEstimoteScanning ();
		}

		private void GetBeaconCloudDetailsIOS (string beaconUUID, int major, int minor)
		{
			IOS_GetBeaconCloudDetails (beaconUUID, major, minor);
		}

		#endregion

		#endif

	}
}