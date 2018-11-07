using UnityEngine;
//using UnityEngine.PostProcessing;
using TMPro;
using System;

public class InitManager : MonoBehaviour 
{
	public static InitManager Instance { get; private set; }

	public TextMeshProUGUI LoadingMissionText;

	void Awake ()
	{
		Instance = this;
		DontDestroyOnLoad (gameObject);
	}

	void Start ()
	{
		Shader.WarmupAllShaders ();
		GC.Collect ();
		LoadingMissionText.text = "";
		LoadingMissionText.gameObject.SetActive (false);

		//SaveAndLoadScript.Instance.LoadSettingsData ();
		//SaveAndLoadScript.Instance.SaveSettingsData ();
		//SaveAndLoadScript.Instance.LoadPlayerData ();

		CheckPostProcessQuality ();
	}

	void CheckPostProcessQuality ()
	{
		int qualLevel = QualitySettings.GetQualityLevel ();

		if (qualLevel == 0) 
		{
		}

		if (qualLevel == 1) 
		{
		}
	}


	void OnApplicationQuit ()
	{
		Debug.Log ("Application ended after " + Time.unscaledTime + " seconds.");
	}
}