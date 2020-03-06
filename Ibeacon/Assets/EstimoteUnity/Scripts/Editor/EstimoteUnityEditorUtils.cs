using UnityEngine;
using System.IO;

namespace OMobile.EstimoteUnity
{
	public class EstimoteUnityEditorUtils
	{

		#if UNITY_EDITOR

		#region Public Static Variables

		public static string IOS_ESTIMOTE_FRAMEWORK_URL = "https://github.com/Estimote/iOS-SDK/archive/4.26.3.zip";
    public static string ANDROID_ESTIMOTE_AAR_URL = "http://github.com/Estimote/Android-SDK/blob/d97660b6b34780013a540d9ea6c1c90fa397c769/EstimoteSDK/estimote-sdk.aar?raw=true";

    public static string ESTIMOTE_UNITY_EDITOR_SETTINGS_PREFIX = "EstimoteUnityEditorSettings-";

    public static string DEFAULT_ESTIMOTE_UUID = "B9407F30-F5F8-466E-AFF9-25556B57FE6D";

		#endregion

		#region Private Static Variables

		private static string IOS_ESTIMOTE_FRAMEWORK_PATH = "/EstimoteUnity/Plugins/iOS/EstimoteSDK.framework/";
		private static string ANDROID_ESTIMOTE_AAR_PATH = "/EstimoteUnity/Plugins/Android/estimote-sdk.aar";

		#endregion

		#region Public Static Methods

		public static string GetIOSEstimoteFrameworkPath ()
		{
			return Application.dataPath + IOS_ESTIMOTE_FRAMEWORK_PATH;
		}

		public static string GetAndroidEstimoteFrameworkPath ()
		{
			return Application.dataPath + ANDROID_ESTIMOTE_AAR_PATH;
		}

		public static bool CheckIOSStatus ()
		{
			bool status = false;
			if (Directory.Exists (GetIOSEstimoteFrameworkPath ())) {
				status = true;
			}
			return status;
		}

		public static bool CheckAndroidStatus ()
		{
			bool status = false;
			if (File.Exists (GetAndroidEstimoteFrameworkPath ())) {
				status = true;
			}
			return status;
		}

		#endregion

		#endif

	}
}
