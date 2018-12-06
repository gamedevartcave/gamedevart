using UnityEngine;
using UnityEngine.UI;

namespace CityBashers
{
	[ExecuteInEditMode]
	public class AspectRatioFitterRefresh : MonoBehaviour 
	{
		private AspectRatioFitter aspectFitter;
		private Graphic graphic;

		void Awake ()
		{
			aspectFitter = GetComponent<AspectRatioFitter> ();
			graphic = GetComponent<Graphic> ();
		}

		void OnEnable ()
		{
			RefreshAspectRatio ();
		}

		public void RefreshAspectRatio ()
		{
			if (graphic.mainTexture != null)
			{
				aspectFitter.aspectRatio = graphic.mainTexture.width / graphic.mainTexture.height;
				//Debug.Log ("New aspect ratio: " + aspectFitter.aspectRatio);
			}
		}
	}
}
