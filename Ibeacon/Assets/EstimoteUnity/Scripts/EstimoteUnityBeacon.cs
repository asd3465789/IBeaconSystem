﻿using UnityEngine;
using System;
using System.Collections;

namespace OMobile.EstimoteUnity
{
	public enum EstimoteUnityBeaconRange
	{
		UNKNOWN,
		FAR,
		NEAR,
		IMMEDIATE
	}

	public class EstimoteUnityBeacon : IEquatable<EstimoteUnityBeacon>
	{
		#region Public Variables

		/// <summary>
		/// Beacons proximity identifer
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
		/// The beacons range
		/// </summary>
		public EstimoteUnityBeaconRange BeaconRange;

		/// <summary>
		/// The signal strength of the beacon, measured in decibels
		/// </summary>
		public int RSSI;

		/// <summary>
		/// The accuracy of the proximity value, measured in meters from the beacon
		/// </summary>
		public double Accuracy;

		/// <summary>
		/// When the beacon was last seen
		/// </summary>
		public DateTime LastSeen;

		#endregion

		#region Constructor

		public EstimoteUnityBeacon (string uuid, int major, int minor, int range, int strength, double accuracy)
		{
			UUID = uuid;
			Major = major;
			Minor = minor;
			BeaconRange = (EstimoteUnityBeaconRange)range;
			RSSI = strength;
			Accuracy = accuracy;
			LastSeen = DateTime.Now;
		}

		#endregion

		#region Public Methods

		public override string ToString ()
		{
			return "" + this.Major + ":" + this.Minor + " - " + this.Accuracy.ToString () + "m";
		}

		public override bool Equals (object obj)
		{
			EstimoteUnityBeacon eub = obj as EstimoteUnityBeacon;
			if (eub != null) {
				return Equals (eub);
			} else {
				return false;
			}
		}

		public bool Equals (EstimoteUnityBeacon other)
		{
			if (other == null) {
				return false;
			}
			return this.UUID.Equals (other.UUID) && this.Major.Equals (other.Major) && this.Minor.Equals (other.Minor);
		}

		public override int GetHashCode ()
		{
			return UUID.GetHashCode () ^ Major.GetHashCode () & Minor.GetHashCode ();
		}

		#endregion
	}
}