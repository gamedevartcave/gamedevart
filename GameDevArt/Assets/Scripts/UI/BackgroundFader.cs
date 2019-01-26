using UnityEngine;

namespace CityBashers
{
	public class BackgroundFader : MonoBehaviour
	{
		public static BackgroundFader Instance { get; private set; }
		[HideInInspector] public Animator fader;

		private void Awake()
		{
			fader = GetComponent<Animator>();
		}

		/// <summary>
		/// Fades out the main loader UI.
		/// </summary>
		public void SceneLoadUIDisappear()
		{
			SceneLoader.Instance.SceneLoadUIDisappear();
		}

		/// <summary>
		/// Unfades the background.
		/// </summary>
		public void Reveal()
		{
			fader.SetBool("Active", false);
		}

		/// <summary>
		/// Activates the scene from the asynchronous operation.
		/// </summary>
		public void ActivateScene()
		{
			SceneLoader.Instance.OnLoadThisScene();
		}
	}
}
