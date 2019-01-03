using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using UnityStandardAssets.Characters.ThirdPerson;

namespace CityBashers
{
	public class GameController : MonoBehaviour 
	{
		public static GameController instance { get; private set; }

		public CameraShake camShakeScript;

		[Header ("Scoring")]
		[ReadOnlyAttribute] public float displayScore;
		[SerializeField] private float targetScore;
		public float score 
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

		//[ReadOnlyAttribute] private bool isCountingScore;
		public TextMeshProUGUI scoreText;
		public float scoreSmoothing;
		public UnityEvent OnScoreCountStarted;
		public UnityEvent OnScoreCountComplete;
		[Space (10)]

		[ReadOnlyAttribute] public float displayComboScore;
		[SerializeField] private float targetComboScore;
		public float comboScore 
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

		[ReadOnlyAttribute] private bool isCountingCombo;
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
		[ReadOnlyAttribute] public bool isPaused;
		public MenuNavigation activeMenu;
		public MenuNavigation firstActiveMenu;
		public UnityEvent OnPause;
		public UnityEvent OnUnpause;

		[Header ("Post processing")]
		public float targetDofDistance;
		public float dofSmoothingIn = 5.0f;
		public float dofSmoothingOut = 5.0f;
		public float maxDofDistance = 1000;

		private PlayerActions playerActions;

		void Awake ()
		{
			instance = this;
			this.enabled = false;
		}

		void Start ()
		{
			playerActions = InControlActions.instance.playerActions;
			//PlayerController.instance.OnWeaponChange.AddListener (OnWeaponChange);
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
			if (playerActions.Pause.WasPressed)
			{
				if (PlayerController.instance.lostAllHealth == false)
				{
					CheckPause ();
				}
			}

			if (isPaused == false)
			{
				GetDepthOfField ();
				GetScore ();
				GetCombo ();
			}
		}

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

		void GetDepthOfField ()
		{
			RaycastHit hit;

			if (Physics.Raycast (Camera.main.transform.position, Camera.main.transform.forward, out hit, maxDofDistance))
			{
				// While moving.
				if (playerActions.CamRot.Value.magnitude > 0.05f || playerActions.Move.Value.magnitude > 0.25f)
				{
					targetDofDistance = Vector3.Distance (
						Camera.main.transform.position, 
						hit.point);
				} 

				else // Idling.
				
				{
					targetDofDistance = Vector3.Distance (
						Camera.main.transform.position, 
						PlayerController.instance.transform.position);
				}
			}

			else // Idling.

			{
				targetDofDistance = Vector3.Distance (
					Camera.main.transform.position, 
					PlayerController.instance.transform.position);
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

		void CheckPause ()
		{
			if (activeMenu == firstActiveMenu)
			{
				isPaused = !isPaused;
			}

			if (isPaused && activeMenu == firstActiveMenu)
			{
				DoPause ();
			}

			if (!isPaused && activeMenu == firstActiveMenu)
			{
				DoUnpause ();
			}
		}

		public void SetActiveMenu (MenuNavigation newActiveMenu)
		{
			activeMenu = newActiveMenu;
		}

		public void DoPause ()
		{
			isPaused = true;
			Time.timeScale = 0;
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			OnPause.Invoke ();
		}

		public void DoUnpause ()
		{
			Time.timeScale = TimescaleController.instance.targetTimeScale;
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
			isPaused = false;
			OnUnpause.Invoke ();
		}

		public void OnWeaponChange ()
		{
			
		}
	}
}
