using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.SceneManagement;
using UnityStandardAssets.ImageEffects;

namespace CityBashers
{
	public class SaveAndLoadScript : MonoBehaviour 
	{
		public static SaveAndLoadScript Instance { get; private set; }

		public bool AllowLoading = true;
		public bool AllowSaving = true;
		public bool TestMode;

		[Header ("Player Data")]
		public string Username = "default"; // For multiple profiles.
		public string myName; // For multiple playthroughs.
		public int LevelId; // This is the maximum level number the player can access, 0 = Level 1, 1 = level 2, etc.

		[Header ("Settings Data")]
		public Camera cam; // Camera to use to change settings.
		public PostProcessingProfile main_postProcessing;
		public PostProcessingProfile camera_UI_PostProcessing;
		public VolumetricLightRenderer volLightRend;
		public SunShafts sunShafts;
		// Screen resolution will be independent of quality settings.
		public int targetResolutionWidth = 1920;
		public int targetResolutionHeight = 1080;

		[Space (10)]
		public int QualitySettingsIndex;
		[Space (10)]
		public int targetFrameRate;
		public float averageFpsTimer;
		[Space (10)]
		[Range (0, 2)]
		public float ParticleEmissionMultiplier = 1;
		[Space (10)]
		public float MasterVolume = 1; // Using volume multiplier applied to AudioListener.
		public float SoundtrackVolume = 0; // Using dB scale (-80, 0)
		public float EffectsVolume = 0;  // Using dB scale (-80, 0)
		public bool invertYAxis;
		//public float MouseSensitivity;

		[HideInInspector]
		public playerData PlayerData;
		//public levelData LevelData;
		[HideInInspector]
		public settingsData SettingsData;

		#region Singleton
		void Awake ()
		{
			Instance = this;
		}
		#endregion

		void Start ()
		{
			
		}

		public void InitializeLoad ()
		{
			SaveAndLoadScript.Instance.LoadPlayerData ();

			cam = Camera.main;
			volLightRend = cam.GetComponent<VolumetricLightRenderer> ();
			sunShafts = cam.GetComponent<SunShafts> ();

			SaveAndLoadScript.Instance.LoadSettingsData ();
		}

		void CheckUsername ()
		{
			if (Username == null || Username == "") 
			{
				Username = "default";
			}

			if (Username == "default") 
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
			
		// Gets variables from this script = variables in other scripts.
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

		// Save PlayerData Main.
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

					Debug.Log (
						"Successfully saved to " +
						Application.persistentDataPath + "/" + Username + "_PlayerConfig.dat"
					); 
				#endif

				#if UNITY_EDITOR
					FileStream file = File.Create (Application.persistentDataPath + "/" + Username + "_PlayerConfig_Editor.dat");

					Debug.Log (
						"Successfully saved to " +
						Application.persistentDataPath + "/" + Username + "_PlayerConfig_Editor.dat"
					); 
				#endif

				// Does the saving
				playerData data = new playerData ();
				SetPlayerData (data);

				// Serializes and closes the file.
				bf.Serialize (file, data);
				file.Close ();
			}
		}

		// Sets data.[variable] = [variable] from this script.
		void SetPlayerData (playerData data)
		{
			data.Username = Username;
			data.myName = myName;

			//SaveLevelData ();
		}

		/*
		public void SaveLevelData ()
		{
			if (SceneManager.GetActiveScene ().name != "menu")
			{
				BinaryFormatter bf = new BinaryFormatter ();
				FileStream file = File.Create (Application.persistentDataPath + "/" + Username + "/" + "Level_" + LevelId + "_data.dat");
				levelData LevDat = new levelData ();


				bf.Serialize (file, LevDat);
				file.Close ();
			}
		}
		*/

		public void DeletePlayerDataEditor ()
		{
			if (File.Exists (Application.persistentDataPath + "/" + Username + "_PlayerConfig_Editor.dat") == true)
			{
				File.Delete (Application.persistentDataPath + "/" + Username + "_PlayerConfig_Editor.dat");

				Debug.Log (
					"Successfully deleted file " +
					Application.persistentDataPath + "/" + Username + "_PlayerConfig_Editor.dat"
				);
			}

			myName = string.Empty;
		}

		public void DeletePlayerDataMain ()
		{
			if (File.Exists (Application.persistentDataPath + "/" + Username + "_PlayerConfig.dat") == true) 
			{
				File.Delete (Application.persistentDataPath + "/" + Username + "_PlayerConfig.dat");

				Debug.Log (
					"Successfully deleted file " +
					Application.persistentDataPath + "/" + Username + "_PlayerConfig.dat"
				);
			}

			myName = string.Empty;
		}

		public void DeleteSettingsDataEditor ()
		{
			if (File.Exists (Application.persistentDataPath + "/" + Username + "_SettingsConfig_Editor.dat") == true) 
			{
				File.Delete (Application.persistentDataPath + "/" + Username + "_SettingsConfig_Editor.dat");

				Debug.Log (
					"Successfully deleted file " +
					Application.persistentDataPath + "/" + Username + "_SettingsConfig_Editor.dat"
				);
			}
		}

		public void DeleteSettingsDataMain ()
		{
			if (File.Exists (Application.persistentDataPath + "/" + Username + "_SettingsConfig.dat") == true)
			{
				File.Delete (Application.persistentDataPath + "/" + Username + "_SettingsConfig.dat");
				Debug.Log (
					"Successfully deleted file " +
					Application.persistentDataPath + "/" + Username + "_SettingsConfig.dat"
				);
			}
		}

		/*
		public void DeleteLevelData ()
		{
			string levelDirectory = Application.persistentDataPath + "/" + Username + "/";

			if (Directory.Exists (levelDirectory) == true)
			{
				Directory.Delete (levelDirectory, true);
			}
		}
		*/

		/*
		public void ResetAllLeaderboards ()
		{
			Debug.Log ("Leaderboards have been reset.");
		}
		*/
			
		// Load PlayerData main.
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
						FileMode.Open
					);

					// Processes the save data into memory.
					playerData data = (playerData)bf.Deserialize (file);
					file.Close ();

					LoadPlayerDataContents (data);
					StorePlayerDataInGame ();

					Debug.Log ("Successfully loaded from " +
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
						FileMode.Open
					);

					// Processes the save data into memory.
					playerData data = (playerData)bf.Deserialize (file);
					file.Close ();

					LoadPlayerDataContents (data);
					StorePlayerDataInGame ();

					Debug.Log ("Successfully loaded from " +
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

		// Sets variables in this script by getting data from save file. 
		void LoadPlayerDataContents (playerData data)
		{
			Username = data.Username;
			myName = data.myName;
		}

		/*
		public void LoadLevelData ()
		{
			if (Directory.Exists (Application.persistentDataPath + "/" + Username + "/") == true)
			{
				if (File.Exists (Application.persistentDataPath + "/" + Username + "/" + "Level_" + LevelId + "_data.dat") == true)
				{
					// Opens the save data.
					BinaryFormatter bf = new BinaryFormatter ();
					FileStream file = File.Open (
						Application.persistentDataPath + "/" + Username + "/" + "Level_" + LevelId + "_data.dat", 
						FileMode.Open
					);

					// Processes the save data into memory.
					levelData levdat = (levelData)bf.Deserialize (file);
					file.Close ();

					Debug.Log ("Successfully loaded from " +
					Application.persistentDataPath + "/" + Username + "/" + "Level_" + LevelId + "_data.dat");
				} 

				else
				
				{
					Directory.CreateDirectory (Application.persistentDataPath + "/" + Username + "/");
					SaveLevelData ();
				}
			} 

			else
			
			{
				Directory.CreateDirectory (Application.persistentDataPath + "/" + Username + "/");
				LoadLevelData ();
			}
		}
		*/
			
		public void StorePlayerDataInGame ()
		{
			if (SceneManager.GetActiveScene ().name != "menu")
			{

			}
		}

		// Gets variables from this script = variables in other scripts.
		void GetSettingsData ()
		{
			if (File.Exists (Application.persistentDataPath + "/" + Username + "_SettingsConfig.dat") == true
			 || File.Exists (Application.persistentDataPath + "/" + Username + "_SettingsConfig_Editor.dat") == true) 
			{
				QualitySettings.SetQualityLevel (QualitySettingsIndex);
				targetResolutionWidth = Screen.currentResolution.width; 
				targetResolutionHeight = Screen.currentResolution.height;

				if (QualitySettingsIndex == 0) 
				{
					ParticleEmissionMultiplier = 0.25f;
				}

				if (QualitySettingsIndex == 1) 
				{
					ParticleEmissionMultiplier = 1.0f;
				}

				MasterVolume = Mathf.Clamp (AudioListener.volume, 0, 1);
				targetFrameRate = Application.targetFrameRate;
			}
		}

		// Save Settings Main.
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
					
					Debug.Log (
						"Successfully saved to " +
						Application.persistentDataPath + "/" + Username + "_SettingsConfig.dat"
					); 
				#endif

				#if UNITY_EDITOR
					FileStream file = File.Create (Application.persistentDataPath + "/" + Username + "_SettingsConfig_Editor.dat");

					Debug.Log (
						"Successfully saved to " +
						Application.persistentDataPath + "/" + Username + "_SettingsConfig_Editor.dat"
					); 
				#endif

				// Does the saving
				settingsData data = new settingsData ();
				SetSettingsData (data);

				// Serializes and closes the file.
				bf.Serialize (file, data);
				file.Close ();
			}
		}

		// Sets data.[variable] = [variable] from this script.
		void SetSettingsData (settingsData data)
		{
			data.QualitySettingsIndex = QualitySettingsIndex;
			data.targetResolutionWidth = targetResolutionWidth;
			data.targetResolutionHeight = targetResolutionHeight;
			data.ParticleEmissionMultiplier = ParticleEmissionMultiplier;
			data.targetFrameRate  = targetFrameRate;
			data.MasterVolume 	  = Mathf.Clamp (MasterVolume, 	     0, 1);
			data.SoundtrackVolume = Mathf.Clamp (SoundtrackVolume, -80, 0);
			data.EffectsVolume 	  = Mathf.Clamp (EffectsVolume,    -80, 0);
			data.invertYAxis = invertYAxis;
		}

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

					Debug.Log ("Successfully loaded from " +
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

					Debug.Log ("Successfully loaded from " +
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

		// Sets variables in this script by getting data from save file. 
		void LoadSettingsDataContents (settingsData data)
		{
			QualitySettingsIndex = data.QualitySettingsIndex;
			targetResolutionWidth = data.targetResolutionWidth;
			targetResolutionHeight = data.targetResolutionHeight;
			ParticleEmissionMultiplier = data.ParticleEmissionMultiplier;
			targetFrameRate = data.targetFrameRate;
			MasterVolume = data.MasterVolume;
			SoundtrackVolume = data.SoundtrackVolume;
			EffectsVolume = data.EffectsVolume;
			invertYAxis = data.invertYAxis;
		}

		// Puts new data into relevant scripts.
		void StoreSettingsDataInGame ()
		{
			QualitySettings.SetQualityLevel (QualitySettingsIndex);

			switch (QualitySettingsIndex)
			{
			case 0: // Low setting. In the dire hopes it works on integrated graphics.
				main_postProcessing.ambientOcclusion.enabled = false;
				main_postProcessing.antialiasing.enabled = false;
				main_postProcessing.bloom.enabled = false;
				main_postProcessing.chromaticAberration.enabled = false;
				main_postProcessing.colorGrading.enabled = false;
				main_postProcessing.depthOfField.enabled = false;
				main_postProcessing.eyeAdaptation.enabled = false;
				main_postProcessing.fog.enabled = false;
				main_postProcessing.grain.enabled = false;
				main_postProcessing.motionBlur.enabled = false;
				main_postProcessing.screenSpaceReflection.enabled = false;
				main_postProcessing.vignette.enabled = false;
				volLightRend.enabled = false;
				sunShafts.enabled = false;
				camera_UI_PostProcessing.bloom.enabled = false;
				break;
			case 1: // Medium setting. Good quality but some optimizations.
				main_postProcessing.ambientOcclusion.enabled = true;
				main_postProcessing.antialiasing.enabled = false;
				main_postProcessing.bloom.enabled = true;
				main_postProcessing.chromaticAberration.enabled = false;
				main_postProcessing.colorGrading.enabled = true;
				main_postProcessing.depthOfField.enabled = true;
				main_postProcessing.eyeAdaptation.enabled = true;
				main_postProcessing.fog.enabled = true;
				main_postProcessing.grain.enabled = true;
				main_postProcessing.motionBlur.enabled = true;
				main_postProcessing.screenSpaceReflection.enabled = false;
				main_postProcessing.vignette.enabled = false;
				volLightRend.enabled = false;
				sunShafts.enabled = true;
				camera_UI_PostProcessing.bloom.enabled = false;
				break;
			case 2: // High setting. Most things are enabled and on high/highest settings.
				main_postProcessing.ambientOcclusion.enabled = true;
				main_postProcessing.antialiasing.enabled = true;
				main_postProcessing.bloom.enabled = true;
				main_postProcessing.chromaticAberration.enabled = true;
				main_postProcessing.colorGrading.enabled = true;
				main_postProcessing.depthOfField.enabled = true;
				main_postProcessing.eyeAdaptation.enabled = true;
				main_postProcessing.fog.enabled = true;
				main_postProcessing.grain.enabled = true;
				main_postProcessing.motionBlur.enabled = true;
				main_postProcessing.screenSpaceReflection.enabled = true;
				main_postProcessing.vignette.enabled = true;
				volLightRend.enabled = true;
				sunShafts.enabled = true;
				camera_UI_PostProcessing.bloom.enabled = true;
				break;
			}

			Screen.SetResolution (targetResolutionWidth, targetResolutionHeight, Screen.fullScreen, 60);

			AudioListener.volume = Mathf.Clamp (MasterVolume, 0, 1);

			if (AudioSettingsManager.instance != null)
			{
				AudioSettingsManager.instance.GetAudioSettings ();
			}

			if (targetFrameRate < 30 && targetFrameRate >= 0) 
			{
				targetFrameRate = -1;
			}

			if (targetFrameRate >= 30 || targetFrameRate <= -1) 
			{
				TargetFPS.Instance.SetTargetFramerate (targetFrameRate);
			}
		}

		// Variables stored in data files.
		[System.Serializable]
		public class playerData
		{
			public string Username;
			public string myName;
			public int ExperiencePoints;
		}

		/*
		[System.Serializable]
		public class levelData
		{
		}
		*/

		[System.Serializable]
		public class settingsData
		{
			public int QualitySettingsIndex;
			public int targetResolutionWidth = 1920;
			public int targetResolutionHeight = 1080;
			[Range (0, 2)]
			public float ParticleEmissionMultiplier = 1;
			public int targetFrameRate = 60;
			public float MasterVolume = 1;
			public float SoundtrackVolume = 0;
			public float EffectsVolume = 0;
			public bool invertYAxis;
		}
	}
}