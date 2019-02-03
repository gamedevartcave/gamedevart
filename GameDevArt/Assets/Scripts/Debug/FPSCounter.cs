using System.Collections;
using UnityEngine;
using TMPro;

namespace CityBashers
{
	public class FPSCounter : MonoBehaviour 
	{
		public static FPSCounter Instance { get; private set; }

		[Tooltip ("How often to update calculating fps.")]
		public float frequency = 0.5f;
		public TextMeshProUGUI FPSText;
		public bool showPrefix;
		public int FramesPerSec { get; protected set; }
		public float FramesPerSecB { get; protected set; }
		public bool toggle;
		public bool showFps;

		private int qty;
		public float averageFps;

		void Awake ()
		{
			Instance = this;
		}

		private void Start()
		{
			StartCoroutine (FPS ());

			if (showFps == true)
			{
				FPSText.enabled = true;
			}

			if (showFps == false)
			{
				FPSText.enabled = false;
			}
		}

		float UpdateCumulativeMovingAverageFPS (float newFPS)
		{
			qty++;
			averageFps += (newFPS - averageFps) / qty;
			return averageFps;
		}

		public void ResetAverageFps()
		{
			qty = 0;
			averageFps = 0;
		}

		private IEnumerator FPS () 
		{
			while (true)
			{
				// Capture current framerate.
				int lastFrameCount = Time.frameCount;

				float lastTime = Time.realtimeSinceStartup; // Get time since startup.

				// Sets how fast this updates.
				yield return new WaitForSecondsRealtime (frequency);

				// Get time span and frame count.
				float timeSpan = Time.realtimeSinceStartup - lastTime;
				int frameCount = Time.frameCount - lastFrameCount;
				
				// Display it.
				FramesPerSec = Mathf.RoundToInt(frameCount / timeSpan);
				FramesPerSecB = frameCount / timeSpan;
				FPSText.text = "CUR FPS: " + FramesPerSec.ToString() + " \nAVG FPS: " + Mathf.RoundToInt (averageFps);

				averageFps = UpdateCumulativeMovingAverageFPS (FramesPerSecB);

				if (showPrefix == true)
				{
					FPSText.text = "FPS: " + FramesPerSec.ToString() + " (" + (System.Math.Round ((1.0f/(FramesPerSecB)) * 1000.0f, 2)) + " ms)";
				}
			}
		}
	}
}