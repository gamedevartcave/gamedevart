using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveAndLoadScript : MonoBehaviour 
{
	public static SaveAndLoadScript Instance { get; private set; }

	public bool AllowLoading = true;
	public bool AllowSaving = true;
	public bool TestMode;

	[Header ("Player Data")]
	public string Username = "default";
	public string myName;
	public int LevelId;

	[Header ("Settings Data")]
	public Camera cam;
	[Space (10)]
	public int QualitySettingsIndex;
	[Space (10)]
	public int targetframerate;
	public float averageFpsTimer;
	[Space (10)]
	public float ParticleEmissionMultiplier = 1;
	[Space (10)]
	public float MasterVolume;
	public float SoundtrackVolume;
	public float EffectsVolume;

	public playerData PlayerData;
	public levelData LevelData;
	public settingsData SettingsData;

	#region Singleton
	void Awake ()
	{
		Instance = this;
	}
	#endregion

	void Start ()
	{
		SaveAndLoadScript.Instance.LoadPlayerData ();
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

	public void DeleteLevelData ()
	{
		string levelDirectory = Application.persistentDataPath + "/" + Username + "/";

		if (Directory.Exists (levelDirectory) == true)
		{
			Directory.Delete (levelDirectory, true);
		}
	}

	public void ResetAllLeaderboards ()
	{
		Debug.Log ("Leaderboards have been reset.");
	}
		
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

			if (QualitySettingsIndex == 0) 
			{
				ParticleEmissionMultiplier = 0.25f;
			}

			if (QualitySettingsIndex == 1) 
			{
				ParticleEmissionMultiplier = 1.0f;
			}

			MasterVolume = Mathf.Clamp (AudioListener.volume, 0, 1);
			targetframerate = Application.targetFrameRate;
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
		data.ParticleEmissionMultiplier = ParticleEmissionMultiplier;
		data.targetframerate  = targetframerate;
		data.MasterVolume 	  = Mathf.Clamp (MasterVolume, 	     0, 1);
		data.SoundtrackVolume = Mathf.Clamp (SoundtrackVolume, -80, 0);
		data.EffectsVolume 	  = Mathf.Clamp (EffectsVolume,    -80, 0);
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
			#endif
		}
	}

	// Sets variables in this script by getting data from save file. 
	void LoadSettingsDataContents (settingsData data)
	{
		QualitySettingsIndex = data.QualitySettingsIndex;
		ParticleEmissionMultiplier = data.ParticleEmissionMultiplier;
		targetframerate = data.targetframerate;
		MasterVolume = data.MasterVolume;
		SoundtrackVolume = data.SoundtrackVolume;
		EffectsVolume = data.EffectsVolume;
	}

	// Puts new data into relevant scripts.
	void StoreSettingsDataInGame ()
	{
		QualitySettings.SetQualityLevel (QualitySettingsIndex);
		AudioListener.volume = Mathf.Clamp (MasterVolume, 0, 1);

		if (AudioSettingsManager.instance != null)
		{
			AudioSettingsManager.instance.GetAudioSettings ();
		}

		if (targetframerate < 30 && targetframerate >= 0) 
		{
			targetframerate = -1;
		}

		if (targetframerate >= 30 || targetframerate <= -1) 
		{
			TargetFPS.Instance.SetTargetFramerate (targetframerate);
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

	[System.Serializable]
	public class levelData
	{
	}

	[System.Serializable]
	public class settingsData
	{
		public int QualitySettingsIndex;
		[Range (0, 2)]
		public float ParticleEmissionMultiplier = 1;
		public int targetframerate;
		public float MasterVolume;
		public float SoundtrackVolume;
		public float EffectsVolume;
	}
}