using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;

namespace OMobile.EstimoteUnity
{
	[CustomEditor (typeof(EstimoteUnity))]
	public class EstimoteUnityInspector : Editor
	{

		#region Private Variables

		// GUI
		private GUIStyle mTitleStyle = null;
		private GUIStyle mSubTitleStyle = null;
		private ReorderableList mBeaconRegionsList;

		// Expansions
		private bool mExpandEstimoteCloud = false;
		private bool mExpandAndroidProperties = false;
		private bool mExpandEvents = false;
		private bool mExpandWarnings = false;
		private bool mExpandHelp = false;

		// Properties
		private SerializedProperty mBeaconRegionsProperty;
		private SerializedProperty mEstimoteCloudEnabledProperty;
		private SerializedProperty mEstimoteCloudAppIdProperty;
		private SerializedProperty mEstimoteCloudAppTokenProperty;
		private SerializedProperty mForegroundScanConfigProperty;
		private SerializedProperty mBackgroundScanConfigProperty;
		private SerializedProperty mOnInitEstimoteCompleteEventProperty;
		private SerializedProperty mOnInitEstimoteCloudCompleteEventProperty;
		private SerializedProperty mOnStartedScanningEventProperty;
		private SerializedProperty mOnStoppedScanningEventProperty;
		private SerializedProperty mOnDidRangeBeaconsEventProperty;
		private SerializedProperty mOnFetchedBeaconCloudDetailsSuccessEventProperty;
		private SerializedProperty mOnFetchedBeaconCloudDetailsErrorEventProperty;

		#endregion

		#region Unity Methods

		void OnEnable ()
		{
			LoadEditorPrefs ();

			// Beacon Regions
			mBeaconRegionsProperty = this.serializedObject.FindProperty ("BeaconRegions");

			// Estimote Cloud
			mEstimoteCloudEnabledProperty = this.serializedObject.FindProperty ("EnableEstimoteCloud");
			mEstimoteCloudAppIdProperty = this.serializedObject.FindProperty ("EstimoteCloudAppId");
			mEstimoteCloudAppTokenProperty = this.serializedObject.FindProperty ("EstimoteCloudAppToken");

			// Android Config
			mForegroundScanConfigProperty = this.serializedObject.FindProperty ("ForegroundScanConfig");
			mBackgroundScanConfigProperty = this.serializedObject.FindProperty ("BackgroundScanConfig");

			// Events
			mOnInitEstimoteCompleteEventProperty = this.serializedObject.FindProperty ("OnInitEstimoteComplete");
			mOnInitEstimoteCloudCompleteEventProperty = this.serializedObject.FindProperty ("OnInitEstimoteCloudComplete");
			mOnStartedScanningEventProperty = this.serializedObject.FindProperty ("OnStartedScanning");
			mOnStoppedScanningEventProperty = this.serializedObject.FindProperty ("OnStoppedScanning");
			mOnDidRangeBeaconsEventProperty = this.serializedObject.FindProperty ("OnDidRangeBeacons");
			mOnFetchedBeaconCloudDetailsSuccessEventProperty = this.serializedObject.FindProperty ("OnFetchedBeaconCloudDetailsSuccess");
			mOnFetchedBeaconCloudDetailsErrorEventProperty = this.serializedObject.FindProperty ("OnFetchedBeaconCloudDetailsError");

			// List
			SetupBeaconRegionsList ();
		}

		void OnDisable ()
		{
			SaveEditorPrefs ();
		}

		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();

			EditorGUI.BeginChangeCheck ();

			EditorGUILayout.Space ();

			DrawHeading ();

			CheckIfPrefab ();

			EditorGUILayout.Space ();

			DrawGeneralProperties ();

			EditorGUILayout.Space ();

			DrawEstimoteCloud ();

			EditorGUILayout.Space ();

			DrawAndroidProperties ();

			EditorGUILayout.Space ();

			DrawEvents ();

			EditorGUILayout.Space ();

			DrawWarnings ();

			EditorGUILayout.Space ();

			DrawHelp ();

			EditorGUILayout.Space ();

			serializedObject.ApplyModifiedProperties ();

			if (EditorGUI.EndChangeCheck ()) {
				EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
			}
		}

		#endregion

		#region Private Methods

		private void DrawHeading ()
		{
			GUILayout.BeginVertical ("box");
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			GUILayout.Label ("Estimote Unity", GetTitleStyle ());
			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			GUILayout.Label ("by Oakley Mobile Ltd.", GetSubTitleStyle ());
			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			GUILayout.Label ("v1.2.0", GetSubTitleStyle ());
			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();
			GUILayout.EndVertical ();
		}

		private void CheckIfPrefab ()
		{
			if (PrefabUtility.GetPrefabParent (((EstimoteUnity)target).gameObject) != null && PrefabUtility.GetPrefabObject (((EstimoteUnity)target).gameObject) != null) {
				EditorGUILayout.Space ();
				GUI.color = Color.red;
				EditorGUILayout.HelpBox ("This GameObject cannot be a prefab. Please disconnect the prefab instance or some of your settings will not be saved properly.", MessageType.Error);
				GUI.color = Color.white;
			}
		}

		private void DrawGeneralProperties ()
		{
			GUILayout.BeginVertical ("box");
			EditorGUILayout.LabelField ("General Properties", EditorStyles.boldLabel);

			EditorGUILayout.HelpBox ("Add your beacon regions (UUID) below. All regions added here will be monitored.", MessageType.Info);

			EditorGUI.indentLevel++;
//			EditorGUILayout.PropertyField (mBeaconRegionsProperty, true);
			mBeaconRegionsList.DoLayoutList ();
			EditorGUI.indentLevel--;

			EditorGUILayout.Space ();

			CheckCanAddDefauiltEstimoteUUID ();

			GUILayout.EndVertical ();
		}

		private void DrawEstimoteCloud ()
		{
			if (GUILayout.Button ("Estimote Cloud", EditorStyles.toolbarButton)) {
				mExpandEstimoteCloud = !mExpandEstimoteCloud;
			}

			if (mExpandEstimoteCloud) {
				GUILayout.BeginVertical ("box");

				EditorGUILayout.PropertyField (mEstimoteCloudEnabledProperty, new GUIContent ("Enable Estimote Cloud Integration"));
				EditorGUI.BeginDisabledGroup (!mEstimoteCloudEnabledProperty.boolValue);
				EditorGUILayout.PropertyField (mEstimoteCloudAppIdProperty, new GUIContent ("App ID"));
				EditorGUILayout.PropertyField (mEstimoteCloudAppTokenProperty, new GUIContent ("App Token"));

				EditorGUILayout.Space ();

				if (GUILayout.Button ("Visit Estimote Cloud App Portal")) {
					Application.OpenURL ("https://cloud.estimote.com/#/apps/add");
				}

				EditorGUI.EndDisabledGroup ();

				GUILayout.EndVertical ();
			}
		}

		private void DrawAndroidProperties ()
		{
			if (GUILayout.Button ("Android Settings", EditorStyles.toolbarButton)) {
				mExpandAndroidProperties = !mExpandAndroidProperties;
			}

			if (mExpandAndroidProperties) {
				GUILayout.BeginVertical ("box");
				EditorGUILayout.HelpBox ("These values change the scan period and wait times (in millis) for scanning BLE Beacons. (Android only)", MessageType.Info);

				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField (mForegroundScanConfigProperty, new GUIContent ("Foreground Scan Config"), true);
				ClampScanConfigValues (mForegroundScanConfigProperty);

				EditorGUILayout.PropertyField (mBackgroundScanConfigProperty, new GUIContent ("Background Scan Config"), true);
				ClampScanConfigValues (mBackgroundScanConfigProperty);
				EditorGUI.indentLevel--;
				GUILayout.EndVertical ();
			}

			EditorUtility.SetDirty ((EstimoteUnity)target);
		}

		private void DrawEvents ()
		{
			if (GUILayout.Button ("Events", EditorStyles.toolbarButton)) {
				mExpandEvents = !mExpandEvents;
			}

			if (mExpandEvents) {
				GUILayout.BeginVertical ("box");
				EditorGUILayout.PropertyField (mOnInitEstimoteCompleteEventProperty, new GUIContent ("OnEstimoteInitComplete"), true);
				EditorGUILayout.PropertyField (mOnInitEstimoteCloudCompleteEventProperty, new GUIContent ("OnInitEstimoteCloudComplete"), true);
				EditorGUILayout.PropertyField (mOnStartedScanningEventProperty, new GUIContent ("OnStartedScanning"), true);
				EditorGUILayout.PropertyField (mOnStoppedScanningEventProperty, new GUIContent ("OnStoppedScanning"), true);
				EditorGUILayout.PropertyField (mOnDidRangeBeaconsEventProperty, new GUIContent ("OnDidRangeBeacons"), true);
				EditorGUILayout.PropertyField (mOnFetchedBeaconCloudDetailsSuccessEventProperty, new GUIContent ("OnFetchedBeaconCloudDetailsSuccess"), true);
				EditorGUILayout.PropertyField (mOnFetchedBeaconCloudDetailsErrorEventProperty, new GUIContent ("OnFetchedBeaconCloudDetailsError"), true);
				GUILayout.EndVertical ();
			}
		}

		private void DrawWarnings ()
		{
			bool hasWarning = !EstimoteUnityEditorUtils.CheckAndroidStatus () || !EstimoteUnityEditorUtils.CheckIOSStatus ();

			if (hasWarning) {
				GUI.color = Color.red;
			}
			if (GUILayout.Button ("Warnings", EditorStyles.toolbarButton)) {
				mExpandWarnings = !mExpandWarnings;
			}
			GUI.color = Color.white;

			if (mExpandWarnings) {
				GUILayout.BeginVertical ("box");

				if (!hasWarning) {
					EditorGUILayout.HelpBox ("No warnings to show.", MessageType.Info);
				}

				// Check Android Status
				if (!EstimoteUnityEditorUtils.CheckAndroidStatus ()) {
					EditorGUILayout.Space ();
					GUI.color = Color.red;
					EditorGUILayout.HelpBox ("Android setup is not complete!" +
					" To complete the setup go to Window/O-Mobile/Estimote Unity/Setup and follow the instructions.", MessageType.Info);
					GUI.color = Color.white;
				}

				// Check iOS Status
				if (!EstimoteUnityEditorUtils.CheckIOSStatus ()) {
					EditorGUILayout.Space ();
					GUI.color = Color.red;
					EditorGUILayout.HelpBox ("iOS setup is not complete!" +
					" To complete the setup go to Window/O-Mobile/Estimote Unity/Setup and follow the instructions.", MessageType.Info);
					GUI.color = Color.white;
				}

				// Show Button
				if (!EstimoteUnityEditorUtils.CheckAndroidStatus () || !EstimoteUnityEditorUtils.CheckIOSStatus ()) {
					EditorGUILayout.Space ();
					if (GUILayout.Button ("Open Estimote Unity Setup")) {
						EstimoteUnityEditorSetup.OpenWindow ();
					}
				}

				GUILayout.EndVertical ();
			}
		}

		private void DrawHelp ()
		{
			if (GUILayout.Button ("Help", EditorStyles.toolbarButton)) {
				mExpandHelp = !mExpandHelp;
			}

			if (mExpandHelp) {
				GUILayout.BeginVertical ("box");
				EditorGUILayout.Space ();

				EditorGUILayout.HelpBox ("Plugin Updates - If you do not want to wait for the Asset Store approvals you can contact us directly using the following email address and we can provide the latest version for you.\nSupport Email: support@o-mobile.co.uk", MessageType.Info);

				EditorGUILayout.Space ();

				// Estimotes Developer Portal
				if (GUILayout.Button ("Visit Estimote Developer Portal")) {
					Application.OpenURL ("http://developer.estimote.com/");
				}

				EditorGUILayout.Space ();

				// O-Mobile's website
				if (GUILayout.Button ("Visit Oakley Mobile's Website")) {
					Application.OpenURL ("http://www.o-mobile.co.uk/");
				}

				GUILayout.EndVertical ();
			}
		}

		private GUIStyle GetTitleStyle ()
		{
			if (mTitleStyle == null) {
				mTitleStyle = new GUIStyle (EditorStyles.largeLabel);
				mTitleStyle.fontStyle = FontStyle.Bold;
				mTitleStyle.fontSize = 20;
			}
			return mTitleStyle;
		}

		private GUIStyle GetSubTitleStyle ()
		{
			if (mSubTitleStyle == null) {
				mSubTitleStyle = new GUIStyle (EditorStyles.largeLabel);
			}
			return mSubTitleStyle;
		}

		private void SetupBeaconRegionsList ()
		{
			mBeaconRegionsList = new ReorderableList (this.serializedObject, mBeaconRegionsProperty, true, true, true, true);
			mBeaconRegionsList.drawHeaderCallback = (Rect rect) => {
				var newRect = new Rect (rect.x + 10, rect.y, rect.width - 10, rect.height);
				EditorGUI.LabelField (newRect, mBeaconRegionsProperty.displayName);
			};
			mBeaconRegionsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
				var element = mBeaconRegionsList.serializedProperty.GetArrayElementAtIndex (index);
				rect.y += 2;
				EditorGUI.PropertyField (new Rect (rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
			};
			mBeaconRegionsList.onAddCallback = (ReorderableList list) => {
				var index = mBeaconRegionsList.serializedProperty.arraySize;
				mBeaconRegionsList.serializedProperty.arraySize++;
				mBeaconRegionsList.index = index;

				var element = mBeaconRegionsList.serializedProperty.GetArrayElementAtIndex (index);
				element.stringValue = "";
			};
		}

		private void ClampScanConfigValues (SerializedProperty prop)
		{
			SerializedProperty scanPeriodMillisProp = prop.FindPropertyRelative ("ScanPeriodMillis");
			if (scanPeriodMillisProp != null && scanPeriodMillisProp.longValue < 200) {
				scanPeriodMillisProp.longValue = 200;
			}

			SerializedProperty waitTimeMillisProp = prop.FindPropertyRelative ("WaitTimeMillis");
			if (waitTimeMillisProp != null && waitTimeMillisProp.longValue < 0) {
				waitTimeMillisProp.longValue = 0;
			}
		}

		private void LoadEditorPrefs ()
		{
			mExpandEstimoteCloud = EditorPrefs.GetBool (EstimoteUnityEditorUtils.ESTIMOTE_UNITY_EDITOR_SETTINGS_PREFIX + "ExpandEstimoteCloud", false);
			mExpandAndroidProperties = EditorPrefs.GetBool (EstimoteUnityEditorUtils.ESTIMOTE_UNITY_EDITOR_SETTINGS_PREFIX + "ExpandAndroidProperties", false);
			mExpandEvents = EditorPrefs.GetBool (EstimoteUnityEditorUtils.ESTIMOTE_UNITY_EDITOR_SETTINGS_PREFIX + "ExpandEvents", false);
			mExpandWarnings = EditorPrefs.GetBool (EstimoteUnityEditorUtils.ESTIMOTE_UNITY_EDITOR_SETTINGS_PREFIX + "ExpandWarnings", false);
			mExpandHelp = EditorPrefs.GetBool (EstimoteUnityEditorUtils.ESTIMOTE_UNITY_EDITOR_SETTINGS_PREFIX + "ExpandHelp", false);
		}

		private void SaveEditorPrefs ()
		{
			EditorPrefs.SetBool (EstimoteUnityEditorUtils.ESTIMOTE_UNITY_EDITOR_SETTINGS_PREFIX + "ExpandEstimoteCloud", mExpandEstimoteCloud);
			EditorPrefs.SetBool (EstimoteUnityEditorUtils.ESTIMOTE_UNITY_EDITOR_SETTINGS_PREFIX + "ExpandAndroidProperties", mExpandAndroidProperties);
			EditorPrefs.SetBool (EstimoteUnityEditorUtils.ESTIMOTE_UNITY_EDITOR_SETTINGS_PREFIX + "ExpandEvents", mExpandEvents);
			EditorPrefs.SetBool (EstimoteUnityEditorUtils.ESTIMOTE_UNITY_EDITOR_SETTINGS_PREFIX + "ExpandWarnings", mExpandWarnings);
			EditorPrefs.SetBool (EstimoteUnityEditorUtils.ESTIMOTE_UNITY_EDITOR_SETTINGS_PREFIX + "ExpandHelp", mExpandHelp);
		}

		private void CheckCanAddDefauiltEstimoteUUID ()
		{
			bool shouldAdd = true;
			for (int i = 0; i < mBeaconRegionsProperty.arraySize; i++) {
				if (mBeaconRegionsProperty.GetArrayElementAtIndex (i).stringValue.Equals (EstimoteUnityEditorUtils.DEFAULT_ESTIMOTE_UUID)) {
					shouldAdd = false;
					break;
				}
			}

			if (shouldAdd) {
				if (GUILayout.Button ("Add default Estimote UUID")) {
					mBeaconRegionsProperty.InsertArrayElementAtIndex (0);
					mBeaconRegionsProperty.GetArrayElementAtIndex (0).stringValue = EstimoteUnityEditorUtils.DEFAULT_ESTIMOTE_UUID;
				}
			}
		}

		#endregion

	}
}