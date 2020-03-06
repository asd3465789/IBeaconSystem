using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OMobile.EstimoteUnity.Demo
{
	public class EstimoteUnityBeaconUIController : MonoBehaviour
	{

		#region Public Variables

		public Text _UUIDText;
		public Text _MajorMinorText;
		public Text _DistanceText;

		#endregion

		#region Private Variables

		private EstimoteUnityBeacon mEstimoteUnityBeacon;
		private EstimoteUnity mEstimoteUnity;

		#endregion

		#region Properties

		public EstimoteUnityBeacon EstimoteBeacon {
			get {
				return mEstimoteUnityBeacon;
			}
		}

		#endregion

		#region Public Methods

		public void Setup (EstimoteUnityBeacon eb, EstimoteUnity estimoteUnity)
		{
			mEstimoteUnityBeacon = eb;
			mEstimoteUnity = estimoteUnity;

			_UUIDText.text = "UUID: " + mEstimoteUnityBeacon.UUID;
			_MajorMinorText.text = "Major / Minor: " + mEstimoteUnityBeacon.Major + ":" + mEstimoteUnityBeacon.Minor;
			_DistanceText.text = "Distance: " + mEstimoteUnityBeacon.Accuracy;

			GetComponent<Button> ().onClick.AddListener (delegate() {
				mEstimoteUnity.GetBeaconCloudDetails (mEstimoteUnityBeacon);
			});
		}

		#endregion

	}
}