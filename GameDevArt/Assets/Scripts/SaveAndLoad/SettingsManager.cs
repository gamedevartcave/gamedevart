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
		public Toggle FullscreenToggle;
		public Toggle LimitFramerateToggle;

		[Header ("Audio Settings")]
		public Slider MasterVolSlider;
		public TextMeshProUGUI MasterVolumeValueText;
		[Space (10)]
		public AudioMixer SoundtrackVolMix;
		private float curSoundtrackVol;
		public Slider SoundtrackVolSlider;
		public TextMeshProUGUI SoundtrackVolumeValueText;
		[Space (10)]
		public AudioMixer EffectsVolMix;
		private float curEffectsVol;
		public Slider EffectsVolSlider;
		public TextMeshProUGUI EffectsVolumeValueText;

		[Header ("Gameplay Settings")]
		public Toggle invertToggle;
		public Slider MouseSensitivitySlider;

		void Awake ()
		{
			Instance = this;
			this.enabled = false;
		}

		void Start ()
		{
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
			switch (SaveAndLoadScript.Instance.QualitySettingsIndex)
			{
			case 0:
				QualitySettings.SetQualityLevel (0);
				break;
			case 1:
				QualitySettings.SetQualityLevel (1);
				break;
			case 2:
				QualitySettings.SetQualityLevel (2);
				break;
			default:
				QualitySettings.SetQualityLevel (2);
				break;
			}
		}

		public void ToggleFullScreen ()
		{
			SaveAndLoadScript.Instance.isFullscreen = FullscreenToggle.isOn;
			Screen.fullScreen = SaveAndLoadScript.Instance.isFullscreen;
			Debug.Log ("Fullscreen " + Screen.fullScreen);
		}

		public void RefreshFullscreenToggle ()
		{
			FullscreenToggle.isOn = Screen.fullScreen;
		}

		public void ToggleLimitFramerate ()
		{
			SaveAndLoadScript.Instance.limitFramerate = LimitFramerateToggle.isOn;

			if (SaveAndLoadScript.Instance.limitFramerate == true)
			{
				SaveAndLoadScript.Instance.targetFrameRate = Screen.currentResolution.refreshRate;
			} 

			else
			
			{
				SaveAndLoadScript.Instance.targetFrameRate = -1;
			}

			Application.targetFrameRate = SaveAndLoadScript.Instance.targetFrameRate;
			Debug.Log ("Target framerate: " + Application.targetFrameRate);
		}

		public void RefreshLimitFramerateToggle ()
		{
			LimitFramerateToggle.isOn = SaveAndLoadScript.Instance.limitFramerate;
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

		public void MasterVolumeOnValueChanged ()
		{
			SaveAndLoadScript.Instance.MasterVolume = MasterVolSlider.value;
			AudioListener.volume = SaveAndLoadScript.Instance.MasterVolume;
			UpdateVolumeTextValues ();
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

		public void SoundtrackVolumeOnValueChanged ()
		{
			SaveAndLoadScript.Instance.SoundtrackVolume = SoundtrackVolSlider.value;
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

		public void EffectsVolumeOnValueChanged ()
		{
			SaveAndLoadScript.Instance.EffectsVolume = EffectsVolSlider.value;
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
				MasterVolumeValueText.text = 
					System.Math.Round (
						Mathf.InverseLerp (
							MasterVolSlider.minValue, 
							MasterVolSlider.maxValue, 
							MasterVolSlider.value
						) * 100, 
					0) 
				+ " %";
			}

			if (SoundtrackVolumeValueText != null)
			{
				SoundtrackVolumeValueText.text = 
					System.Math.Round (
						Mathf.InverseLerp (
							SoundtrackVolSlider.minValue, 
							SoundtrackVolSlider.maxValue,
							SoundtrackVolSlider.value
						) * 100, 
					0) 
				+ " %";
			}

			if (EffectsVolumeValueText != null)
			{
				EffectsVolumeValueText.text = 
					System.Math.Round (
						Mathf.InverseLerp (
							EffectsVolSlider.minValue, 
							EffectsVolSlider.maxValue, 
							EffectsVolSlider.value
						) * 100, 
					0) 
				+ " %";
			}
		}

		public void LoadAudioVolumes ()
		{
			MasterVolSlider.value = AudioListener.volume;
			SoundtrackVolMix.SetFloat ("SoundtrackVolume", SaveAndLoadScript.Instance.SoundtrackVolume);
			SoundtrackVolSlider.value = GetSoundtrackVolumeValue ();
			EffectsVolMix.SetFloat ("EffectsVolume", SaveAndLoadScript.Instance.EffectsVolume);
			EffectsVolSlider.value = GetEffectsVolumeValue ();
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

		public void SetMouseSensSliderValue ()
		{
			MouseSensitivitySlider.value = SaveAndLoadScript.Instance.MouseSensitivityMultplier;
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