using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

namespace CityBashers
{
	public class AudioSettingsManager : MonoBehaviour 
	{
		public static AudioSettingsManager instance { get; private set; }

		public Slider MasterVolumeSlider;
		[Space (10)]
		public AudioMixerGroup SoundtrackMixer;
		public Slider SoundtrackVolumeSlider;
		private float SoundtrackVol;
		[Space (10)]
		public AudioMixerGroup EffectsMixer;
		public Slider EffectsVolumeSlider;
		private float EffectsVol;

		void Awake ()
		{
			instance = this;
			SaveAndLoadScript.Instance.LoadSettingsData ();
		}

		public void GetAudioSettings ()
		{
			MasterVolumeSlider.value = SaveAndLoadScript.Instance.MasterVolume;
			AudioListener.volume = MasterVolumeSlider.value;

			SoundtrackVolumeSlider.value = SaveAndLoadScript.Instance.SoundtrackVolume;
			SoundtrackMixer.audioMixer.SetFloat ("MasterVol", SoundtrackVol);

			EffectsVolumeSlider.value = SaveAndLoadScript.Instance.EffectsVolume;
			EffectsMixer.audioMixer.SetFloat ("MasterVol", EffectsVol);
		}

		public void MasterVolumeOnValueChanged ()
		{
			float MasterVol = MasterVolumeSlider.value;
			SaveAndLoadScript.Instance.MasterVolume = MasterVol;
			AudioListener.volume = MasterVol;
		}

		public void SoundtrackVolumeOnValueChanged ()
		{
			SoundtrackVol = SoundtrackVolumeSlider.value;
			SaveAndLoadScript.Instance.SoundtrackVolume = SoundtrackVol;
			SoundtrackMixer.audioMixer.SetFloat ("MasterVol", SoundtrackVol);
		}

		public void EffectsVolumeOnValueChanged ()
		{
			EffectsVol = EffectsVolumeSlider.value;
			SaveAndLoadScript.Instance.EffectsVolume = EffectsVol;
			EffectsMixer.audioMixer.SetFloat ("MasterVol", EffectsVol);
		}

		public void SaveSettings ()
		{
			SaveAndLoadScript.Instance.SaveSettingsData ();
		}
	}
}