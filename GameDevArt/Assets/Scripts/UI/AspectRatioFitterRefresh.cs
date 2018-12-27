using UnityEngine;
using UnityEngine.UI;

namespace CityBashers
{
	[ExecuteInEditMode]
	[RequireComponent (typeof (AspectRatioFitter))]
	[RequireComponent (typeof (Graphic))]
	public class AspectRatioFitterRefresh : MonoBehaviour 
	{
		private AspectRatioFitter aspectFitter;
		private Graphic graphic;
		private float ratio;

		void OnEnable ()
		{
			if (aspectFitter == null) aspectFitter = GetComponent<AspectRatioFitter> ();
			if (graphic == null) graphic = GetComponent<Graphic> ();
			
			RefreshAspectRatio (graphic);
		}

		/// <summary>
		/// Refreshs the aspect ratio for the graphic.
		/// </summary>
		public void RefreshAspectRatio (Graphic _graphic)
		{
			if (_graphic.mainTexture != null)
			{
				// Get width and height ratio, set aspect ratio in fitter.
				ratio = _graphic.mainTexture.width / _graphic.mainTexture.height;
				aspectFitter.aspectRatio = ratio;
				//Debug.Log ("New aspect ratio: " + aspectFitter.aspectRatio);
			}
		}
	}
}
