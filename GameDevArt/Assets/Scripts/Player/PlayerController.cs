using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityEngine.Rendering.PostProcessing;

namespace CityBashers
{
	public class PlayerController : MonoBehaviour
	{
		public static PlayerController instance { get; private set; }

		[Header ("General")]
		public Collider playerCol;
		public Rigidbody playerRb;
		public Animator PlayerUI;
		public Transform startingPoint;

		[Header ("Health")]
		[ReadOnlyAttribute] public bool lostAllHealth;
		public int health;
		public int StartHealth = 100;
		public int MaximumHealth = 100;
		public Slider HealthSlider;
		public Slider HealthSlider_Smoothed;
		public float healthSliderSmoothing;
		public UnityEvent OnLostAllHealth;
		public PostProcessVolume postProcessUIVolume;

		[Header ("Magic")]
		public int magic;
		public int StartingMagic = 100;
		public int MaximumMagic = 100;
		public Slider MagicSlider;
		public Slider MagicSlider_Smoothed;
		public float magicSliderSmoothing;

		[Header ("Aiming")]
		public float normalFov = 65;
		public float aimFov = 35;
		public float aimSmoothing = 10;
		public GameObject CrosshairObject;
		public Transform AimingOrigin;
		[HideInInspector] public Vector3 AimDir;
		[HideInInspector] public Vector3 AimNoPitchDir;

		[Header ("Shooting")]
		public float currentFireRate;
		[HideInInspector] public float nextFire;
		public UnityEvent OnShoot;

		[Header ("Weapons")]
		public int currentWeaponIndex;
		public GameObject[] Weapons;

		public RawImage DisplayedWeaponImage;
		public TextMeshProUGUI DisplayedWeaponAmmoText;

		[Header ("WeaponWheel")]
		private bool weaponTexturesAssigned;
		public TextMeshProUGUI currentSelectedWeaponAmmoText;
		public RawImage CurrentSelectedWeaponImage;
		public Texture2D[] weaponTextures;

		[Space (10)]
		public RawImage[] WeaponSelectionBackgrounds;
		public RawImage[] WeaponSelectionImages;

		public Color WeaponAvailableColor;
		public Color WeaponAmmoOutColor;
		[Space (10)]
		public TextMeshProUGUI[] weaponWheelAmmoTexts;
		public int[] currentAmmoAmounts;
		public int[] maxAmmoAmounts;

		[Header ("Using")]
		public UnityEvent OnUse;

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

		[Header ("Hit stun")]
		[ReadOnlyAttribute] public bool isInHitStun;
		public Renderer[] skinnedMeshes;
		[ReadOnlyAttribute] [SerializeField] private float hitStunCurrentTime;
		public float hitStunDuration = 2;
		private WaitForSeconds hitStunYield;
		public float HitStunRenderToggleWait = 0.07f;
		public UnityEvent OnHitStunBegin;
		public UnityEvent OnHitStunEnded;

		void Awake ()
		{
			instance = this;

			for (int i = 0; i < skinnedMeshes.Length; i++)
			{
				skinnedMeshes [i].enabled = false;
			}

			this.enabled = false;
		}

		void Start ()
		{
			health = StartHealth;
			HealthSlider.value = health;
			HealthSlider_Smoothed.value = health;

			magic = StartingMagic;
			MagicSlider.value = magic;
			MagicSlider_Smoothed.value = magic;

			OnFootstep.AddListener (OnFootStep);
			OnJump.AddListener (OnJumpBegan);
			OnDoubleJump.AddListener (OnDoubleJumpBegan);
			OnLand.AddListener (OnLanded);

			hitStunYield = new WaitForSeconds (HitStunRenderToggleWait);
		}

		void LateUpdate ()
		{
			CheckHealthSliders ();
			CheckMagicSliders ();

			if (health < 25 || magic < 25)
			{
				if (PlayerUI.GetBool ("Low") == false)
				{
					PlayerUI.SetBool ("Low", true);
				}
			} 

			else
			
			{
				if (PlayerUI.GetBool ("Low") == true)
				{
					PlayerUI.SetBool ("Low", false);
				}
			}
		}

		#region Health
		void CheckHealthSliders ()
		{
			HealthSlider.value = Mathf.Clamp (health, 0, MaximumHealth);
			HealthSlider_Smoothed.value = Mathf.Lerp (
				HealthSlider_Smoothed.value, 
				health, 
				healthSliderSmoothing * Time.deltaTime
			);

			postProcessUIVolume.profile.GetSetting <Vignette> ().intensity.value = -0.005f * HealthSlider.value + 0.5f;
			postProcessUIVolume.profile.GetSetting <MotionBlur> ().shutterAngle.value = -3.6f * HealthSlider.value + 360;
			SaveAndLoadScript.Instance.postProcessVolume.profile.GetSetting<ColorGrading> ().saturation.value = 
				HealthSlider.value - 100;

			if (health <= 0)
			{
				if (lostAllHealth == false)
				{
					lostAllHealth = true;
					OnLostAllHealth.Invoke ();
					Debug.Log ("Player died.");
				}
			}
		}
		#endregion

		#region Magic
		void CheckMagicSliders ()
		{
			MagicSlider.value = Mathf.Clamp (magic, 0, MaximumMagic);
			MagicSlider_Smoothed.value = Mathf.Lerp (
				MagicSlider_Smoothed.value, 
				magic, 
				magicSliderSmoothing * Time.deltaTime
			);
		}
		#endregion

		#region Shooting
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
		}
		#endregion

		#region Weapons
		public void SetWeaponIndex (int index, bool reverse)
		{
			bool runout = true;

			for (int i = 0; i < Weapons.Length; i++)
			{
				// If there is ammo in any weapon.
				if (currentAmmoAmounts [i] != 0)
				{
					runout = false;
				}

				// If there is no ammo in any weapon.
				if (runout == true)
				{
					return;
				}
			}

			// Scrolling down.
			if (reverse == true)
			{
				// Check for ammo has run out.
				// Then restart loop.
				if (currentAmmoAmounts [index] <= 0)
				{
					currentWeaponIndex++;

					if (currentWeaponIndex >= Weapons.Length)
					{
						currentWeaponIndex = 0;
					}

					SetWeaponIndex (currentWeaponIndex, true);
					return;
				}
			}

			else // Scrolling up

			{
				// Check for ammo has run out.
				// Then restart loop.
				if (currentAmmoAmounts [index] <= 0)
				{
					currentWeaponIndex--;

					if (currentWeaponIndex < 0)
					{
						currentWeaponIndex = Weapons.Length - 1;
					}

					SetWeaponIndex (currentWeaponIndex, false);
					return;
				}
			}

			// Update all weapon selections.
			for (int i = 0; i < Weapons.Length; i++)
			{	
				Weapons [i].SetActive ((index == i) ? true : false);
				weaponWheelAmmoTexts [i].text = currentAmmoAmounts [i] + " / " + maxAmmoAmounts [i];
				WeaponSelectionBackgrounds [i].enabled = ((index == i) ? true : false);

				// Sets all weapons without ammo to be not available.
				if (currentAmmoAmounts [i] <= 0)
				{
					WeaponSelectionImages [i].color = WeaponAmmoOutColor;
				} 

				else // Weapon is available.
				
				{
					WeaponSelectionImages [i].color = WeaponAvailableColor;
				}

				// Only called once to assign weapon textures.
				if (weaponTexturesAssigned == false)
				{
					WeaponSelectionImages [i].texture = weaponTextures [i];
				}
			}

			// Update displayed weapon.
			DisplayedWeaponImage.texture = weaponTextures [index];
			DisplayedWeaponAmmoText.text = currentAmmoAmounts [index] + " / " + maxAmmoAmounts [index];
				
			// Update current ammo selection for center of wheel.
			CurrentSelectedWeaponImage.texture = weaponTextures [index];
			currentSelectedWeaponAmmoText.text = currentAmmoAmounts [index] + " / " + maxAmmoAmounts [index];

			weaponTexturesAssigned = true; // Prevents assigning of weapon textures the next time.
		}
		#endregion

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

		#region Physics

		// When player hits death barrier.
		// Hard reset on positioning and movement.
		public void DeathBarrier ()
		{
			transform.position = startingPoint.position;
			playerRb.velocity = Vector3.zero;
		}
		#endregion

		#region Override
		public void OverridePlayerPosition (Transform newPos)
		{
			transform.position = newPos.position;
		}

		public void OverridePosition (Vector3 pos)
		{
			this.transform.position = pos;
		}
		#endregion

		#region Hit stun
		public void DoHitStun ()
		{	
			if (isInHitStun == false)
			{
				StartCoroutine (Hitstun ());
			}
		}

		public IEnumerator Hitstun ()
		{
			// Get current time.
			hitStunCurrentTime = Time.time;
			isInHitStun = true;
			OnHitStunBegin.Invoke ();

			while (Time.time < hitStunCurrentTime + hitStunDuration)
			{
				for (int i = 0; i < skinnedMeshes.Length; i++)
				{
					skinnedMeshes [i].enabled = !skinnedMeshes [i].enabled;
				}

				yield return hitStunYield;
			}

			for (int i = 0; i < skinnedMeshes.Length; i++)
			{
				// Always end with skinned mesh renderers enabled.
				skinnedMeshes [i].enabled = true;
			}
				
			OnHitStunEnded.Invoke ();
			isInHitStun = false;
		}
		#endregion

		public void EnableSkinnedMeshes ()
		{
			for (int i = 0; i < skinnedMeshes.Length; i++)
			{
				skinnedMeshes [i].enabled = true;
			}
		}
	}
}
