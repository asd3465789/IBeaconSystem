using UnityEngine;
using System.Collections;

namespace OMobile.EstimoteUnity
{
	public class EstimoteUnityBeaconCloudInfo
	{

		#region Public Variables

		/// <summary>
		/// Beacons proximity identifier
		/// </summary>
		public string UUID;

		/// <summary>
		/// Beacons Major ID
		/// </summary>
		public int Major;

		/// <summary>
		/// Beacons Minor ID
		/// </summary>
		public int Minor;

		/// <summary>
		/// Beacons name
		/// </summary>
		public string BeaconName;

		/// <summary>
		/// Beacons color
		/// </summary>
		public string Color;

		/// <summary>
		/// Beacons battery life in days
		/// </summary>
		public double BatteryLifeDays;

		/// <summary>
		/// Beacons mac address
		/// </summary>
		public string MacAddress;

		#endregion

		#region Constructor

		public EstimoteUnityBeaconCloudInfo (string uuid, int major, int minor, string beaconName, string beaconColor, double batteryLife, string macAddress)
		{
			UUID = uuid;
			Major = major;
			Minor = minor;
			BeaconName = beaconName;
			Color = beaconColor;
			BatteryLifeDays = batteryLife;
			MacAddress = macAddress;
		}

		#endregion

		#region Public Methods

		public override string ToString ()
		{
			return string.Format ("[EstimoteUnityBeaconCloudInfo] {0}, {1}, {2}, {3}, {4}, {5}, {6}", UUID, Major, Minor, BeaconName, Color, BatteryLifeDays, MacAddress);
		}

		#endregion

	}
}