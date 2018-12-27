using UnityEngine;

namespace CityBashers
{
	public class LoadingUI : MonoBehaviour 
	{
		public void StartLoadSequence ()
		{
			SceneLoader.Instance.StartLoadSequence ();
		}
	}
}
