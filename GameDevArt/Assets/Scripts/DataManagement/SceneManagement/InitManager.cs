using UnityEngine;
using TMPro;
using System;

namespace CityBashers
{
	public class InitManager : MonoBehaviour 
	{
		public static InitManager Instance { get; private set; }

		public TextMeshProUGUI LoadingMissionText;

		void Awake ()
		{
			Instance = this;
			//DontDestroyOnLoad (gameObject);
		}

		void Start ()
		{
			//LoadingMissionText.text = "";
			//LoadingMissionText.gameObject.SetActive (false);
		}

		void OnApplicationQuit ()
		{
			Debug.Log ("Application ended after " + Time.unscaledTime + " seconds.");
		}
	}
}