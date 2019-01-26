using UnityEngine;

namespace CityBashers
{
	public class MainMenuManager : MonoBehaviour 
	{
		void Start () 
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			SceneLoader.Instance.backgroundFader.fader.SetTrigger("FadeOut");
		}
	}
}
