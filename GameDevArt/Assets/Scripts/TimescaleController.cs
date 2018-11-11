using UnityEngine;

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
	}

	void Update ()
	{
		SetTargetTimeScale ();
	}

	void SetTargetTimeScale ()
	{
		if (GameController.instance.isPaused == false)
		{
			Time.timeScale = Mathf.Lerp (Time.timeScale, targetTimeScale, timeScaleSmoothing * Time.unscaledDeltaTime);
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
