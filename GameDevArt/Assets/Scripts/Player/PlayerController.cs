using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityStandardAssets.Characters.ThirdPerson;

public class PlayerController : MonoBehaviour
{
	public static PlayerController instance { get; private set; }

	public CameraShake camShakeScript;
	public Collider playerCol;

	public Collider DeathBarrier;

	[Header ("Health")]
	public int health;
	public int StartHealth = 100;
	public int MaximumHealth = 100;
	public Slider HealthSlider;
	public Slider HealthSlider_Smoothed;
	public float healthSliderSmoothing;

	[Header ("Aiming")]
	public float normalFov = 65;
	public float aimFov = 35;
	public float aimSmoothing = 10;
	public GameObject CrosshairObject;
	public Transform AimingOrigin;
	public Vector3 AimDir;
	public Vector3 AimNoPitchDir;

	[Header ("Shooting")]
	public float currentFireRate;
	[HideInInspector] public float nextFire;
	public UnityEvent OnShoot;

	[Header ("Footsteps")]
	public AudioSource footstepAudioSource;
	public AudioClip[] footstepClips;
	private int footstepSoundIndex;
	public float footStepRate = 0.25f;
	private float nextFootstepTime;
	public UnityEvent OnFootstep;

	[Header ("Jumping")]
	public AudioSource jumpAudioSource;
	public AudioClip[] jumpClips;
	private int jumpSoundIndex;
	public UnityEvent OnJump;

	[Header ("Double jumping")]
	public AudioSource doubleJumpAudioSource;
	public AudioClip[] doubleJumpClips;
	private int doubleJumpSoundIndex;
	public UnityEvent OnDoubleJump;

	[Header ("Landing")]
	public AudioSource landingAudioSource;
	public AudioClip[] landingClips;
	private int landingSoundIndex;
	public UnityEvent OnLand;


	public UnityEvent OnUse;



	void Awake ()
	{
		instance = this;
	}

	void Start ()
	{
		health = StartHealth;

		OnFootstep.AddListener (OnFootStep);
		OnJump.AddListener (OnJumpBegan);
		OnDoubleJump.AddListener (OnDoubleJumpBegan);
		OnLand.AddListener (OnLanded);

		//camShakeScript.Shake ();
	}

	void LateUpdate ()
	{
		CheckHealthSliders ();
	}

	void CheckHealthSliders ()
	{
		HealthSlider.value = health;
		HealthSlider_Smoothed.value = Mathf.Lerp (
			HealthSlider_Smoothed.value, 
			health, 
			healthSliderSmoothing * Time.deltaTime
		);
	}

	public void Shoot ()
	{
		RaycastHit hit;
		Ray ray = ThirdPersonUserControl.instance.m_Cam.ScreenPointToRay (Input.mousePosition);
		AimDir = ray.direction;
		AimNoPitchDir = new Vector3 (ray.direction.x, 0, ray.direction.z);


		if (Physics.Raycast (AimingOrigin.position, ray.direction, out hit, Mathf.Infinity))
		{
			Debug.DrawRay (AimingOrigin.position, ray.direction * 10, Color.cyan, 0.2f);
			Debug.DrawRay (AimingOrigin.position, AimNoPitchDir * 10, Color.green, 0.2f);
		}

		OnShoot.Invoke ();
		Debug.Log ("Shoot");
	}

	#region Footsteps
	public void GetFootStepSound ()
	{
		if (Time.time > nextFootstepTime)
		{
			footstepSoundIndex = Random.Range (0, footstepClips.Length);
			footstepAudioSource.clip = footstepClips [footstepSoundIndex];
			footstepAudioSource.Play ();
			nextFootstepTime = Time.time + footStepRate;
		}
	}

	void OnFootStep ()
	{
		GetFootStepSound ();
	}
	#endregion

	#region Jumping
	public void GetJumpSound ()
	{
		jumpSoundIndex = Random.Range (0, jumpClips.Length);
		jumpAudioSource.clip = jumpClips [jumpSoundIndex];
		jumpAudioSource.Play ();
	}

	void OnJumpBegan ()
	{
		GetJumpSound ();
	}

	public void GetDoubleJumpSound ()
	{
		doubleJumpSoundIndex = Random.Range (0, doubleJumpClips.Length);
		doubleJumpAudioSource.clip = doubleJumpClips [doubleJumpSoundIndex];
		doubleJumpAudioSource.Play ();
	}

	void OnDoubleJumpBegan ()
	{
		GetDoubleJumpSound ();
	}

	public void GetLandingSound ()
	{
		landingSoundIndex = Random.Range (0, landingClips.Length);
		landingAudioSource.clip = landingClips [landingSoundIndex];
		landingAudioSource.Play ();
	}

	void OnLanded ()
	{
		GetLandingSound ();
	}
	#endregion

	void OnTriggerEnter (Collider other)
	{
		if (other == DeathBarrier)
		{
			transform.position = Vector3.zero;
		}
	}
}
