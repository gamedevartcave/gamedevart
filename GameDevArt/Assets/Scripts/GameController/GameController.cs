using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.PostProcessing;

namespace CityBashers
{
	public class GameController : MonoBehaviour
	{
		public static GameController Instance { get; private set; }

		public CameraShake camShakeScript;

		[Header ("Scoring")]
		[ReadOnly] public float displayScore;
		[SerializeField] private float targetScore;
		public float Score 
		{
			get 
			{ 
				return targetScore; 
			}

			set 
			{ 
				OnScoreCountStarted.Invoke (); 
				targetScore = value; 
			}
		}

		//[ReadOnly] private bool isCountingScore;
		public TextMeshProUGUI scoreText;
		public float scoreSmoothing;
		public UnityEvent OnScoreCountStarted;
		public UnityEvent OnScoreCountComplete;
		[Space (10)]

		[ReadOnly] public float displayComboScore;
		[SerializeField] private float targetComboScore;
		public float ComboScore 
		{
			get 
			{ 
				return targetComboScore; 
			}

			set 
			{
				OnComboCountStarted.Invoke (); 
				targetComboScore = value; 
			}
		}

		[ReadOnly] private bool isCountingCombo;
		public TextMeshProUGUI comboScoreText;
		public float comboScoreSmoothing;

		public UnityEvent OnComboCountStarted;
		public UnityEvent OnComboCountComplete;

		[Header ("Game timer")]
		public UnityEvent OnTimerTrackingStarted;
		public UnityEvent OnTimerTrackingStopped;
		private bool trackTime;
		private float finalTime;
		private float startTime;

		[Header ("Pausing")]
		[ReadOnly] public bool isPaused;
		public float unpauseCooldown = 0.25f;
		private float nextUnpause;
		public MenuNavigation activeMenu;
		public MenuNavigation firstActiveMenu;
		public UnityEvent OnPause;
		public UnityEvent OnUnpause;
		
		[Header ("Post processing")]
		[ReadOnly] public float targetDofDistance;
		public float dofSmoothingIn = 5.0f;
		public float dofSmoothingOut = 5.0f;
		public float maxDofDistance = 1000;

		Vector3 delta;
		Vector3 lastPos;

		void Awake ()
		{
			Instance = this;
			enabled = false;
		}

		void Start ()
		{
			displayScore = 0;
			targetScore = 0;
			scoreText.text = 0.ToString ();
			displayComboScore = 0;
			targetComboScore = 0;
			comboScoreText.text = string.Empty;

			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		void Update ()
		{
			// Do this while not paused.
			if (isPaused == false)
			{
				GetDepthOfField();
				GetScore();
				GetCombo();
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
			}

			else

			{
				delta = Input.mousePosition - lastPos;

				if (delta.sqrMagnitude > 1)
				{
					Cursor.visible = true;
					Cursor.lockState = CursorLockMode.None;
					//Debug.Log("Unlocked");
				}

				//Debug.Log("delta X : " + delta.x);
				//Debug.Log("delta Y : " + delta.y);
				lastPos = Input.mousePosition;
			}
		}

		/// <summary>
		/// Gets score value.
		/// </summary>
		void GetScore ()
		{
			if (targetScore - displayScore >= 0.5f)
			{
				//isCountingScore = true;
				displayScore = Mathf.Lerp (displayScore, targetScore, scoreSmoothing * Time.deltaTime);
				displayScore = Mathf.Clamp (displayScore, 0, Mathf.Infinity);
				scoreText.text = Mathf.Round (displayScore).ToString ();
			} 

			else
			
			{
				if (isCountingCombo == true)
				{
					OnScoreCountComplete.Invoke ();
					isCountingCombo = false;
				}
			}
		}

		/// <summary>
		/// Gets combo score value.
		/// </summary>
		void GetCombo ()
		{
			if (targetComboScore - displayComboScore >= 0.5f)
			{
				isCountingCombo = true;
				displayComboScore = Mathf.Lerp (displayComboScore, targetComboScore, scoreSmoothing * Time.deltaTime);
				displayComboScore = Mathf.Clamp (displayComboScore, 0, Mathf.Infinity);
				comboScoreText.text = "+ " + Mathf.Round (displayComboScore).ToString ();
			} 

			else
			
			{
				if (isCountingCombo == true)
				{
					OnComboCountComplete.Invoke ();
					isCountingCombo = false;
				}
			}
		}

		/// <summary>
		/// Gets depth of field value to be used on post processing using raycasts.
		/// </summary>
		void GetDepthOfField ()
		{
			if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, maxDofDistance))
			{
				// While moving.
				if (PlayerController.Instance.MoveAxis.sqrMagnitude > 0.1f ||
					MouseLook.Instance.LookAxis.sqrMagnitude > 0.01f)
				{
					targetDofDistance = Vector3.Distance(
						Camera.main.transform.position,
						hit.point);
				}

				else // Idling.

				{
					targetDofDistance = Vector3.Distance(
						Camera.main.transform.position,
						PlayerController.Instance.transform.position);
				}
			}

			else // raycast too far.

			{
				targetDofDistance = Vector3.Distance(
					Camera.main.transform.position,
					PlayerController.Instance.transform.position);
			}

			if (SaveAndLoadScript.Instance.postProcessVolume.profile != null)
			{
				float currentdof = 
					SaveAndLoadScript.Instance.postProcessVolume.profile.GetSetting <DepthOfField> ().focusDistance.value;

				// Distance is decreasing.
				if (currentdof > targetDofDistance)
				{
					SaveAndLoadScript.Instance.postProcessVolume.profile.GetSetting <DepthOfField> ().focusDistance.value = 
					Mathf.SmoothStep (
						currentdof, 
						Mathf.Clamp (targetDofDistance, 0, maxDofDistance), 
						Time.deltaTime * dofSmoothingIn
					);
				}

				else // Distance is increasing.
				
				{
					SaveAndLoadScript.Instance.postProcessVolume.profile.GetSetting <DepthOfField> ().focusDistance.value = 
					Mathf.SmoothStep (
						currentdof, 
						Mathf.Clamp (targetDofDistance, 0, maxDofDistance), 
						Time.deltaTime * dofSmoothingOut
					);
				}
			}

			#if UNITY_EDITOR
			Debug.DrawRay (Camera.main.transform.position, Camera.main.transform.forward, Color.blue);
			//Debug.DrawLine (Camera.main.transform.position, hit.point, Color.gray);
			#endif
		}

		/// <summary>
		/// Starts tracking time.
		/// </summary>
		public void StartTrackingTime ()
		{
			if (trackTime == false)
			{
				startTime = Time.time;
				trackTime = true;
				Debug.Log ("Started tracking time.");
				OnTimerTrackingStarted.Invoke ();
			}
		}

		/// <summary>
		/// Stops tracking time.
		/// </summary>
		public void StopTrackingTime ()
		{
			if (trackTime == true)
			{
				finalTime = Time.time - startTime;
				trackTime = false;

				int hours = Mathf.FloorToInt (finalTime / 3600);
				int minutes = Mathf.FloorToInt (finalTime / 60);
				int seconds = Mathf.FloorToInt (finalTime % 60);
				int milliseconds = 
					Mathf.RoundToInt ((finalTime - Mathf.FloorToInt (finalTime)) * 1000);

				Debug.Log ("Tracked time is: " + 
					hours + " hours, " + 
					minutes + " minutes, " + 
					seconds + " seconds, " + 
					milliseconds + " milliseconds"
				);

				OnTimerTrackingStopped.Invoke ();
			}
		}

		/// <summary>
		/// Toggle pause and unpause state.
		/// </summary>
		public void CheckPause ()
		{
			if (Time.unscaledTime > nextUnpause)
			{
				if (activeMenu == firstActiveMenu)
				{
					isPaused = !isPaused;
				}

				if (isPaused && activeMenu == firstActiveMenu)
				{
					DoPause();
				}

				if (!isPaused && activeMenu == firstActiveMenu)
				{
					DoUnpause();
				}
			}
		}

		/// <summary>
		/// Sets chosen active menu.
		/// </summary>
		/// <param name="newActiveMenu"></param>
		public void SetActiveMenu (MenuNavigation newActiveMenu)
		{
			activeMenu = newActiveMenu;
			MenuNavigation.ActiveMenu = activeMenu;
		}

		/// <summary>
		/// Pauses the game.
		/// </summary>
		public void DoPause ()
		{
			isPaused = true;

			Time.timeScale = 0;

			OnPause.Invoke ();

			Physics.autoSimulation = false;
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
			lastPos = Input.mousePosition;
			SettingsManager.Instance.UpdateGameplayValues();
			SettingsManager.Instance.RefreshFullscreenToggle();
			SettingsManager.Instance.RefreshLimitFramerateToggle();
		}

		/// <summary>
		/// Unpauses the game.
		/// </summary>
		public void DoUnpause ()
		{
			isPaused = false;

			Time.timeScale = TimescaleController.Instance.targetTimeScale;
			nextUnpause = Time.unscaledTime + unpauseCooldown;

			OnUnpause.Invoke ();

			Physics.autoSimulation = true;
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}
	}
}
