using UnityEngine;

namespace CityBashers
{
	public class TimescaleController : MonoBehaviour 
	{
		public static TimescaleController instance { get; private set; }

		[SerializeField] [ReadOnlyAttribute] private float currentTimeScale;
		public float targetTimeScale = 1;
		public float timeScaleSmoothing = 10;

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
			Time.fixedDeltaTime = Time.timeScale * timeStep;
		}
	}
}
