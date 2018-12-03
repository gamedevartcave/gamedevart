using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

namespace CityBashers
{
	public class SettingsManager : MonoBehaviour 
	{
		public static SettingsManager Instance { get; private set; }

		[Header ("Visual Settings")]
		public Camera cam;

		public Button HighEndQualityButton;
		public Button MediumQualityButton;
		public Button LowEndQualityButton;

		[Header ("Audio Settings")]
		public Button MasterVolumeButtonUp;
		public Button MasterVolumeButtonDown;
		public TextMeshProUGUI MasterVolumeValueText;

		public AudioMixer SoundtrackVolMix;
		private float curSoundtrackVol;
		public Button SoundtrackVolumeButtonUp;
		public Button SoundtrackVolumeButtonDown;
		public TextMeshProUGUI SoundtrackVolumeValueText;

		public AudioMixer EffectsVolMix;
		private float curEffectsVol;
		public Button EffectsVolumeButtonUp;
		public Button EffectsVolumeButtonDown;
		public TextMeshProUGUI EffectsVolumeValueText;

		[Header ("Gameplay Settings")]
		public bool invertMouse;
		public Toggle invertToggle;
		public Slider MouseSensitivitySlider;

		void Awake ()
		{
			Instance = this;
			this.enabled = false;
		}

		void Start ()
		{
			SaveAndLoadScript.Instance.cam = cam;

			UpdateVisuals ();
			UpdateVolumeTextValues ();
			UpdateGameplayValues ();

			InvokeRepeating ("GetSoundtrackVolumeValue", 0, 1);
			InvokeRepeating ("GetEffectsVolumeValue", 0, 1);
		}


		#region Video
		public void OnQualitySettingsButtonClick (int QualityIndex)
		{
			SaveAndLoadScript.Instance.QualitySettingsIndex = QualityIndex;
			Debug.Log ("Quality Settings index set to: " + SaveAndLoadScript.Instance.QualitySettingsIndex);
		}

		public void UpdateVisuals ()
		{
			// Low visual quality settings.
			if (SaveAndLoadScript.Instance.QualitySettingsIndex == 0) 
			{
				QualitySettings.SetQualityLevel (0);

				SaveAndLoadScript.Instance.ParticleEmissionMultiplier = 0.25f;
			}

			// Medium visual quality settings.
			if (SaveAndLoadScript.Instance.QualitySettingsIndex == 1) 
			{
				QualitySettings.SetQualityLevel (1);

				SaveAndLoadScript.Instance.ParticleEmissionMultiplier = 1f;
			}

			// High visual quality settings.
			if (SaveAndLoadScript.Instance.QualitySettingsIndex == 2) 
			{
				QualitySettings.SetQualityLevel (2);

				SaveAndLoadScript.Instance.ParticleEmissionMultiplier = 1f;
			}
		}
		#endregion

		#region Audio
		public void MasterVolumeUpOnClick ()
		{
			if (AudioListener.volume < 1) 
			{
				SaveAndLoadScript.Instance.MasterVolume += 0.1f;
				AudioListener.volume = SaveAndLoadScript.Instance.MasterVolume;
				UpdateVolumeTextValues ();
			}
		}

		public void MasterVolumeDownOnClick ()
		{
			if (AudioListener.volume > 0) 
			{
				SaveAndLoadScript.Instance.MasterVolume -= 0.1f;
				AudioListener.volume = SaveAndLoadScript.Instance.MasterVolume;
				UpdateVolumeTextValues ();
			}
		}
			
		public void SoundtrackVolumeUpOnClick ()
		{
			SaveAndLoadScript.Instance.SoundtrackVolume += 8f;
			UpdateSoundtrackVol ();
		}

		public void SoundtrackVolumeDownOnClick ()
		{
			SaveAndLoadScript.Instance.SoundtrackVolume -= 8f;
			UpdateSoundtrackVol ();
		}

		void UpdateSoundtrackVol ()
		{
			SaveAndLoadScript.Instance.SoundtrackVolume = Mathf.Clamp (SaveAndLoadScript.Instance.SoundtrackVolume, -80, 0);
			curSoundtrackVol = SaveAndLoadScript.Instance.SoundtrackVolume;
			SoundtrackVolMix.SetFloat ("SoundtrackVolume", curSoundtrackVol);
			UpdateVolumeTextValues ();
		}

		public void EffectsVolumeUpOnClick ()
		{
			SaveAndLoadScript.Instance.EffectsVolume += 8f;
			UpdateEffectsVol ();
		}

		public void EffectsVolumeDownOnClick ()
		{
			SaveAndLoadScript.Instance.EffectsVolume -= 8f;
			UpdateEffectsVol ();
		}

		void UpdateEffectsVol ()
		{
			SaveAndLoadScript.Instance.EffectsVolume = Mathf.Clamp (SaveAndLoadScript.Instance.EffectsVolume, -80, 0);
			curEffectsVol = SaveAndLoadScript.Instance.EffectsVolume;
			EffectsVolMix.SetFloat ("EffectsVolume", curEffectsVol);
			UpdateVolumeTextValues ();
		}

		// Gets current soundtrack volume from mixer.
		public float GetSoundtrackVolumeValue ()
		{
			if (SoundtrackVolMix != null)
			{
				bool curVolResult = SoundtrackVolMix.GetFloat ("SoundtrackVolume", out curSoundtrackVol);

				if (curVolResult)
				{
					return curSoundtrackVol;
				} 

				else
				
				{
					return 0f;
				}
			} 

			else
			
			{
				return 0f;
			}
		}

		// Gets current effects volume from mixer.
		public float GetEffectsVolumeValue ()
		{
			if (EffectsVolMix != null)
			{
				bool curVolResult = EffectsVolMix.GetFloat ("EffectsVolume", out curEffectsVol);

				if (curVolResult)
				{
					return curEffectsVol;
				} 

				else
				
				{
					return 0f;
				}
			} 

			else
			
			{
				return 0f;
			}
		}

		void UpdateVolumeTextValues ()
		{
			if (MasterVolumeValueText != null)
			{
				MasterVolumeValueText.text = System.Math.Round (
					SaveAndLoadScript.Instance.MasterVolume, 1).ToString ();
			}

			if (SoundtrackVolumeValueText != null)
			{
				SoundtrackVolumeValueText.text = (1 +
				System.Math.Round ((0.0125f * SaveAndLoadScript.Instance.SoundtrackVolume), 1)
				).ToString ();
			}

			if (EffectsVolumeValueText != null)
			{
				EffectsVolumeValueText.text = (1 +
				System.Math.Round ((0.0125f * SaveAndLoadScript.Instance.EffectsVolume), 1)
				).ToString ();
			}
		}
		#endregion

		#region Gameplay
		void UpdateGameplayValues ()
		{
			invertToggle.isOn = SaveAndLoadScript.Instance.invertYAxis;
		}

		public void SetInvertMouse ()
		{
			SaveAndLoadScript.Instance.invertYAxis = invertToggle.isOn;
		}

		public void SetMouseSens ()
		{

		}
		#endregion
			
		#region Saving and applying settings
		public void SaveSettings ()
		{
			SaveAndLoadScript.Instance.SaveSettingsData ();
		}

		public void LoadSettings ()
		{
			SaveAndLoadScript.Instance.LoadSettingsData ();
		}

		public void ApplySettings ()
		{
			SaveAndLoadScript.Instance.SaveSettingsData ();
			SaveAndLoadScript.Instance.LoadSettingsData ();
		}

		public void RevertSettings ()
		{
			SaveAndLoadScript.Instance.LoadSettingsData ();
			RefreshSettings ();
		}

		public void RefreshSettings ()
		{
			UpdateVolumeTextValues ();
		}
		#endregion
	}
}