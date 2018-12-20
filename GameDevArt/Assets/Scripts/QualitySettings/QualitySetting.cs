using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace CityBashers
{
	[CreateAssetMenu (fileName = "New Quality Setting", menuName = "Quality Setting")]
	public class QualitySetting : ScriptableObject
	{
		public bool sunShafts;
		public bool volumetricLighting;
		public PostProcessLayer.Antialiasing AntiAliasingMode;
		public bool fog;
		public bool lensDistortion;
		public bool motionBlur;
		public bool autoExposure;
		public bool vignette;
		public bool screenSpaceReflections;
		public bool ambientOcclusion;
		public bool chromaticAbberation;
		public bool depthOfField;
		public bool grain;
		public bool bloom;
		public bool colorGrading;
	}
}
