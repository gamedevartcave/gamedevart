using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingScreenTextUpdate : MonoBehaviour
{
	public TextMeshProUGUI loadingScreenText;

	void Start ()
	{
		if (loadingScreenText == null) 
		{
			loadingScreenText = InitManager.Instance.LoadingMissionText;
		}
	}

	public void UpdateMissionText (string missionText)
	{
		loadingScreenText.text = missionText;
	}

	public void UpdateLoadingTextInLevel ()
	{
		loadingScreenText.text = "Level " + (SaveAndLoadScript.Instance.LevelId + 1).ToString ();
	}
}