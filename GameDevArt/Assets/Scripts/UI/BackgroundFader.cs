using UnityEngine;

namespace CityBashers
{
	public class BackgroundFader : MonoBehaviour
	{
		public static BackgroundFader Instance { get; private set; }

		public Animator fader;

		public void SceneLoadUIDisappear()
		{
			SceneLoader.Instance.SceneLoadUIDisappear();
		}

		public void Reveal()
		{
			fader.SetBool("Active", false);
		}

		public void ActivateScene()
		{
			SceneLoader.Instance.OnLoadThisScene();
			//Debug.Log("Activates scene");
		}
	}
}
