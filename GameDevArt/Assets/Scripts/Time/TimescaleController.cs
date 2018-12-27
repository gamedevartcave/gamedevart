using UnityEngine;

namespace CityBashers
{
	public class TimescaleController : MonoBehaviour 
	{
		public static TimescaleController instance { get; private set; }

		// This is just to show in the inspector rather than having to navigate to TimeManager.
		[SerializeField] [ReadOnlyAttribute] private float currentTimeScale;

		// Set by other scripts so Time.timeScale can smoothly interpolate to this value.
		[Range (0, 100)]
		public float targetTimeScale = 1;

		/// <summary>
		/// How much smoothing is applied to the interpolation to the target value.
		/// </summary>
		public float timeScaleSmoothing = 10;

		/// <summary>
		/// Time step resolution, smaller value = a more precise FixedUpdate ().
		/// </summary>
		public float timeStep = 0.01f;

		void Awake ()
		{
			instance = this;
		}

		void Start ()
		{
			Time.timeScale = targetTimeScale;
			currentTimeScale = Time.timeScale;
		}

		void Update ()
		{
			SetTargetTimeScale ();
		}

		void SetTargetTimeScale ()
		{
			if (GameController.instance != null)
			{
				if (GameController.instance.isPaused == false)
				{
					currentTimeScale = Mathf.Lerp (
						currentTimeScale, 
						targetTimeScale, 
						timeScaleSmoothing * Time.unscaledDeltaTime
					);

					Time.timeScale = currentTimeScale;
				}
			}
		}

		public void LateUpdate ()
		{
			SetFixedUpdateOnTimeScale ();
		}

		void SetFixedUpdateOnTimeScale ()
		{
			if (Time.fixedDeltaTime != Time.timeScale * timeStep)
			{
				Time.fixedDeltaTime = Time.timeScale * timeStep;
			}
		}
	}
}
