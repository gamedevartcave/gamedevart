using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityStandardAssets.ImageEffects;

namespace CityBashers
{
	public class SaveAndLoadScript : MonoBehaviour 
	{
		public static SaveAndLoadScript Instance { get; private set; }

		public bool AllowLoading = true;
		public bool AllowSaving = true;

		[Header ("Player Data")]
		public string Username = "default"; // For multiple profiles.
		public string defaultUsername = "default";

		[Header ("Effects components")]
		public Camera cam; // Camera to use to change settings.
		public SunShafts sunShafts;
		public EdgeDetection edgeDetection;
		public PostProcessProfile postProcessProfile;
		public PostProcessLayer postProcessLayer;
		public PostProcessVolume postProcessVolume;
		public QualitySetting[] qualitySettings;

		[Header ("Visual settings")]
		public int QualitySettingsIndex;
		public int targetResolutionWidth = 1920; // Screen resolution will be independent of quality settings.
		public int targetResolutionHeight = 1080;
		public bool isFullscreen;
		public bool limitFramerate = true;
		public int targetFrameRate;
		public float averageFpsTimer;

		[Header ("Audio settings")]
		public float MasterVolume = 1; // Using volume multiplier applied to AudioListener.
		public float SoundtrackVolume = 0; // Using dB scale (-80, 0)
		public float EffectsVolume = 0;  // Using dB scale (-80, 0)

		[Header ("Gameplay settings")]
		public bool invertYAxis;
		public float MouseSensitivityMultplier = 1;

		[HideInInspector] public playerData PlayerData;
		[HideInInspector] public settingsData SettingsData;

		#region Singleton
		void Awake ()
		{
			Instance = this;
		}
		#endregion

		/// <summary>
		/// Initializes the app.
		/// </summary>
		public void InitializeLoad ()
		{
			SaveAndLoadScript.Instance.LoadPlayerData ();

			cam = Camera.main;
			sunShafts = cam.GetComponent<SunShafts> ();
			edgeDetection = cam.GetComponent<EdgeDetection> ();

			SaveAndLoadScript.Instance.LoadSettingsData ();

			LocalSceneLoader.Instance.FadeOutLoaderUI ();
		}

		/// <summary>
		/// Raises the application quit event.
		/// </summary>
		void OnApplicationQuit ()
		{
			SaveSettingsData ();
		}

		/// <summary>
		/// Checks the username from data.
		/// </summary>
		void CheckUsername ()
		{
			// Checks if username field is null or empty.
			// If so sets default value.
			if (string.IsNullOrEmpty (Username) == false) 
			{
				Username = defaultUsername;
			}

			// When username is default username.
			if (Username == defaultUsername) 
			{
				/*
				Debug.Log (
					"Username is " 
					+ Username + 
					". Consider changing your username in the menu. " +
					"You may not have created a local profile yet."
				);
				*/
			}
		}
			
		/// <summary>
		/// Gets variables from this script = variables in other scripts.
		/// </summary>
		void GetPlayerData ()
		{
			#if !UNITY_EDITOR
			if (File.Exists (Application.persistentDataPath + "/" + Username + "_PlayerConfig.dat") == true) 
			{

			}

			if (File.Exists (Application.persistentDataPath + "/" + Username + "_PlayerConfig.dat") == false) 
			{

			}
			#endif

			#if UNITY_EDITOR
			if (File.Exists (Application.persistentDataPath + "/" + Username + "_PlayerConfig_Editor.dat") == true) 
			{
				
			}
				
			if (File.Exists (Application.persistentDataPath + "/" + Username + "_PlayerConfig_Editor.dat") == false) 
			{
				
			}
			#endif
		}
			
		/// <summary>
		/// Save PlayerData Main.
		/// </summary>
		public void SavePlayerData ()
		{
			if (AllowSaving == true) 
			{
				// Refer to GetPlayerData.
				GetPlayerData ();

				// Creates new save file.
				BinaryFormatter bf = new BinaryFormatter ();

				#if !UNITY_EDITOR
				FileStream file = File.Create (Application.persistentDataPath + "/" + Username + "_PlayerConfig.dat");

				Debug.Log ("<color=cyan>Saved player data</color>\n" +
					Application.persistentDataPath + "/" + Username + "_PlayerConfig.dat"); 
				#endif

				#if UNITY_EDITOR
				FileStream file = File.Create (Application.persistentDataPath + "/" + Username + "_PlayerConfig_Editor.dat");

				Debug.Log ("<color=cyan>Saved player data</color>\n" +
					Application.persistentDataPath + "/" + Username + "_PlayerConfig_Editor.dat"); 
				#endif

				// Does the saving.
				playerData data = new playerData ();

				SetPlayerData (data);

				// Serializes and closes the file.
				bf.Serialize (file, data);
				file.Close ();
			}
		}
			
		/// <summary>
		/// Sets data.[variable] = [variable] from this script.
		/// </summary>
		/// <param name="data">Data.</param>
		void SetPlayerData (playerData data)
		{
			data.Username = Username;
		}

		/// <summary>
		/// Deletes the editor player data.
		/// </summary>
		public void DeletePlayerDataEditor ()
		{
			if (File.Exists (Application.persistentDataPath + "/" + Username + "_PlayerConfig_Editor.dat") == true)
			{
				File.Delete (Application.persistentDataPath + "/" + Username + "_PlayerConfig_Editor.dat");

				Debug.Log ("<color=red>Deleted player data</color>\n" +
					Application.persistentDataPath + "/" + Username + "_PlayerConfig_Editor.dat"
				);
			}
		}

		/// <summary>
		/// Deletes the main player data.
		/// </summary>
		public void DeletePlayerDataMain ()
		{
			if (File.Exists (Application.persistentDataPath + "/" + Username + "_PlayerConfig.dat") == true) 
			{
				File.Delete (Application.persistentDataPath + "/" + Username + "_PlayerConfig.dat");

				Debug.Log ("<color=red>Deleted player data</color>\n" +
					Application.persistentDataPath + "/" + Username + "_PlayerConfig.dat"
				);
			}
		}

		/// <summary>
		/// Deletes the editor settings data.
		/// </summary>
		public void DeleteSettingsDataEditor ()
		{
			if (File.Exists (Application.persistentDataPath + "/" + Username + "_SettingsConfig_Editor.dat") == true) 
			{
				File.Delete (Application.persistentDataPath + "/" + Username + "_SettingsConfig_Editor.dat");

				Debug.Log ("<color=red>Deleted settings data</color>\n" +
					Application.persistentDataPath + "/" + Username + "_SettingsConfig_Editor.dat"
				);
			}
		}

		/// <summary>
		/// Deletes the main settings data.
		/// </summary>
		public void DeleteSettingsDataMain ()
		{
			if (File.Exists (Application.persistentDataPath + "/" + Username + "_SettingsConfig.dat") == true)
			{
				File.Delete (Application.persistentDataPath + "/" + Username + "_SettingsConfig.dat");
				Debug.Log ("<color=red>Deleted settings data</color>\n" +
					Application.persistentDataPath + "/" + Username + "_SettingsConfig.dat"
				);
			}
		}
			
		/// <summary>
		/// Load PlayerData main.
		/// </summary>
		public void LoadPlayerData ()
		{
			#if !UNITY_EDITOR
			if (AllowLoading == true)
			{
				if (File.Exists (Application.persistentDataPath + "/" + Username + "_PlayerConfig.dat") == true) 
				{
					// Opens the save data.
					BinaryFormatter bf = new BinaryFormatter ();
					FileStream file = File.Open (
						Application.persistentDataPath + "/" + Username + "_PlayerConfig.dat", 
						FileMode.Open);

					// Processes the save data into memory.
					playerData data = (playerData)bf.Deserialize (file);
					file.Close ();

					LoadPlayerDataContents (data);
					StorePlayerDataInGame ();

					Debug.Log ("<color=cyan>Loaded player data</color>\n" +
						Application.persistentDataPath + "/" + Username + "_PlayerConfig.dat");
				}

				else
				
				{
					SavePlayerData ();
					LoadPlayerData ();
				}
			}
			#endif

			#if UNITY_EDITOR
			if (AllowLoading == true)
			{
				if (File.Exists (Application.persistentDataPath + "/" + Username + "_PlayerConfig_Editor.dat") == true) 
				{
					// Opens the save data.
					BinaryFormatter bf = new BinaryFormatter ();

					FileStream file = File.Open (
						Application.persistentDataPath + "/" + Username + "_PlayerConfig_Editor.dat", 
						FileMode.Open);

					// Processes the save data into memory.
					playerData data = (playerData)bf.Deserialize (file);
					file.Close ();

					LoadPlayerDataContents (data);
					StorePlayerDataInGame ();

					Debug.Log ("<color=#00FF00>Loaded player data</color>\n" +
						Application.persistentDataPath + "/" + Username + "_PlayerConfig_Editor.dat");
				}

				else
				
				{
					SavePlayerData ();
					LoadPlayerData ();
				}
			}
			#endif
		}

		/// <summary>
		/// Sets variables in this script by getting data from save file.
		/// </summary>
		/// <param name="data">Data.</param>
		void LoadPlayerDataContents (playerData data)
		{
			Username = data.Username;
		}
			
		/// <summary>
		/// Stores the player data in game.
		/// </summary>
		public void StorePlayerDataInGame ()
		{
			if (SceneManager.GetActiveScene ().name != "menu")
			{

			}
		}
			
		/// <summary>
		/// Gets variables from this script = variables in other scripts.
		/// </summary>
		void GetSettingsData ()
		{
			if (File.Exists (Application.persistentDataPath + "/" + Username + "_SettingsConfig.dat") == true
			 || File.Exists (Application.persistentDataPath + "/" + Username + "_SettingsConfig_Editor.dat") == true) 
			{
				// Visual settings.
				QualitySettings.SetQualityLevel (QualitySettingsIndex);
				targetResolutionWidth = Screen.currentResolution.width; 
				targetResolutionHeight = Screen.currentResolution.height;

				// Audio settings.
				targetFrameRate = Application.targetFrameRate;
			}
		}
			
		/// <summary>
		/// Save Settings Main.
		/// </summary>
		public void SaveSettingsData ()
		{
			if (AllowSaving == true) 
			{
				// Refer to GetSettingsData.
				GetSettingsData ();

				// Creates new save file.
				BinaryFormatter bf = new BinaryFormatter ();

				#if !UNITY_EDITOR
				FileStream file = File.Create (Application.persistentDataPath + "/" + Username + "_SettingsConfig.dat");
				
				Debug.Log ("<color=cyan>Saved settings data</color>\n" +
					Application.persistentDataPath + "/" + Username + "_SettingsConfig.dat"); 
				#endif

				#if UNITY_EDITOR
				FileStream file = File.Create (Application.persistentDataPath + "/" + Username + "_SettingsConfig_Editor.dat");

				Debug.Log ("<color=cyan>Saved settings data</color>\n" +
					Application.persistentDataPath + "/" + Username + "_SettingsConfig_Editor.dat"); 
				#endif

				// Does the saving
				settingsData data = new settingsData ();

				SetSettingsData (data);

				// Serializes and closes the file.
				bf.Serialize (file, data);
				file.Close ();
			}
		}
			
		/// <summary>
		/// Sets data.[variable] = [variable] from this script.
		/// </summary>
		/// <param name="data">Data.</param>
		void SetSettingsData (settingsData data)
		{
			// Visual settings.
			data.QualitySettingsIndex 	= QualitySettingsIndex;
			data.targetResolutionWidth  = targetResolutionWidth;
			data.targetResolutionHeight = targetResolutionHeight;
			data.isFullscreen 			= isFullscreen;
			data.limitFramerate 		= limitFramerate;
			data.targetFrameRate  		= targetFrameRate;

			// Audio settings.
			data.MasterVolume 	  = Mathf.Clamp (MasterVolume, 	     0, 1);
			data.SoundtrackVolume = Mathf.Clamp (SoundtrackVolume, -80, 0);
			data.EffectsVolume 	  = Mathf.Clamp (EffectsVolume,    -80, 0);

			// Gameplay settings.
			data.invertYAxis = invertYAxis;
			data.MouseSensitivityMultplier = MouseSensitivityMultplier;
		}

		/// <summary>
		/// Loads the settings data.
		/// </summary>
		public void LoadSettingsData ()
		{
			if (AllowLoading == true)
			{
				#if !UNITY_EDITOR
				if (File.Exists (Application.persistentDataPath + "/" + Username + "_SettingsConfig.dat") == true) 
				{
					// Opens the save data.
					BinaryFormatter bf = new BinaryFormatter ();

					FileStream file = File.Open (Application.persistentDataPath + "/" + Username + "_SettingsConfig.dat", FileMode.Open);

					Debug.Log ("<color=#00FF00>Saved settings data</color>\n" +
						Application.persistentDataPath + "/" + Username + "_SettingsConfig.dat");

					// Processes the save data into memory.
					settingsData data = (settingsData)bf.Deserialize (file);
					
					file.Close ();

					LoadSettingsDataContents (data);
					StoreSettingsDataInGame ();
				}

				else

				{
					SaveSettingsData ();
					LoadSettingsData ();
				}
				#endif

				#if UNITY_EDITOR
				if (File.Exists (Application.persistentDataPath + "/" + Username + "_SettingsConfig_Editor.dat") == true) 
				{
					// Opens the save data.
					BinaryFormatter bf = new BinaryFormatter ();

					FileStream file = File.Open (Application.persistentDataPath + "/" + Username + "_SettingsConfig_Editor.dat", FileMode.Open);

					Debug.Log ("<color=#00FF00>Loaded settings data</color>\n" +
						Application.persistentDataPath + "/" + Username + "_SettingsConfig_Editor.dat");

					// Processes the save data into memory.
					settingsData data = (settingsData)bf.Deserialize (file);

					file.Close ();

					LoadSettingsDataContents (data);
					StoreSettingsDataInGame ();
				}

				else
				
				{
					SaveSettingsData ();
					LoadSettingsData ();
				}
				#endif
			}
		}
			
		/// <summary>
		/// Sets variables in this script by getting data from save file. 
		/// </summary>
		/// <param name="data">Data.</param>
		void LoadSettingsDataContents (settingsData data)
		{
			// Visual settings.
			QualitySettingsIndex 	= data.QualitySettingsIndex;
			targetResolutionWidth 	= data.targetResolutionWidth;
			targetResolutionHeight  = data.targetResolutionHeight;
			isFullscreen 			= data.isFullscreen;
			limitFramerate 			= data.limitFramerate;
			targetFrameRate 		= data.targetFrameRate;

			// Audio settings.
			MasterVolume 	 = data.MasterVolume;
			SoundtrackVolume = data.SoundtrackVolume;
			EffectsVolume 	 = data.EffectsVolume;

			// Gameplay settings.
			invertYAxis = data.invertYAxis;
			MouseSensitivityMultplier = data.MouseSensitivityMultplier;
		}
			
		/// <summary>
		/// Puts new data into relevant scripts.
		/// </summary>
		void StoreSettingsDataInGame ()
		{
			if (postProcessLayer == null)
			{
				postProcessLayer = cam.GetComponent<PostProcessLayer> ();
			}

			QualitySettings.SetQualityLevel (QualitySettingsIndex);

			sunShafts.enabled = qualitySettings [QualitySettingsIndex].sunShafts;
			//edgeDetection.enabled = qualitySettings [QualitySettingsIndex].edgeDetection;

			postProcessLayer.antialiasingMode = qualitySettings [QualitySettingsIndex].AntiAliasingMode;
			postProcessLayer.fog.enabled = qualitySettings [QualitySettingsIndex].fog;

			postProcessVolume.profile.GetSetting <LensDistortion> ().enabled.value = 
				qualitySettings [QualitySettingsIndex].lensDistortion;
			
			postProcessVolume.profile.GetSetting <UnityEngine.Rendering.PostProcessing.MotionBlur> ().enabled.value = 
				qualitySettings [QualitySettingsIndex].motionBlur;
			
			postProcessVolume.profile.GetSetting <AutoExposure> ().enabled.value = 
				qualitySettings [QualitySettingsIndex].autoExposure;
			
			postProcessVolume.profile.GetSetting <Vignette> ().enabled.value = 
				qualitySettings [QualitySettingsIndex].vignette;
			
			postProcessVolume.profile.GetSetting <ScreenSpaceReflections> ().enabled.value = 
				qualitySettings [QualitySettingsIndex].screenSpaceReflections;
			
			postProcessVolume.profile.GetSetting <AmbientOcclusion> ().enabled.value = 
				qualitySettings [QualitySettingsIndex].ambientOcclusion;
			
			postProcessVolume.profile.GetSetting <ChromaticAberration> ().enabled.value = 
				qualitySettings [QualitySettingsIndex].chromaticAbberation;
			
			postProcessVolume.profile.GetSetting <UnityEngine.Rendering.PostProcessing.DepthOfField> ().enabled.value = 
				qualitySettings [QualitySettingsIndex].depthOfField;
			
			postProcessVolume.profile.GetSetting <Grain> ().enabled.value = 
				qualitySettings [QualitySettingsIndex].grain;
			
			postProcessVolume.profile.GetSetting <UnityEngine.Rendering.PostProcessing.Bloom> ().enabled.value = 
				qualitySettings [QualitySettingsIndex].bloom;
			
			postProcessVolume.profile.GetSetting <ColorGrading> ().enabled.value = 
				qualitySettings [QualitySettingsIndex].colorGrading;

			AudioListener.volume = Mathf.Clamp (MasterVolume, 0, 1);
			SettingsManager.Instance.LoadAudioVolumes ();

			SetTargetFramerate ();
		}

		/// <summary>
		/// Sets the target framerate.
		/// </summary>
		void SetTargetFramerate ()
		{
			if (limitFramerate == true)
			{
				if (targetFrameRate < 30 && targetFrameRate >= 0)
				{
					targetFrameRate = Screen.currentResolution.refreshRate;
				} 

				else

				{
					TargetFPS.Instance.SetTargetFramerate (targetFrameRate);
				}
			} 

			else

			{
				targetFrameRate = -1;
			}

			//Debug.Log ("Application.targetFrameRate = " + Application.targetFrameRate);
		}

		// Variables stored in data files.
		[Serializable]
		public class playerData
		{
			public string Username;
		}

		[Serializable]
		public class settingsData
		{
			// VISUAL SETTINGS
			public int QualitySettingsIndex;
			public int targetResolutionWidth = 1920;
			public int targetResolutionHeight = 1080;
			public bool isFullscreen;
			public bool limitFramerate;
			public int targetFrameRate = 60;

			// AUDIO SETTINGS
			public float MasterVolume = 1;
			public float SoundtrackVolume = 0;
			public float EffectsVolume = 0;

			// GAMEPLAY SETTINGS
			public bool invertYAxis;
			public float MouseSensitivityMultplier = 1;
		}
	}
}