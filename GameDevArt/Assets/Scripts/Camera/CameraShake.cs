using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
	// Transform of the camera to shake. Grabs the gameObject's transform if null.
	public Transform camTransform;

	[Header ("Parameters")]
	// How long the object should shake for.
	public float shakeDuration = 0f;
	public float shakeTimeRemaining;
	
	// Amplitude of the shake. A larger value shakes the camera harder.
	public float shakeAmount = 0.7f;
	public float decreaseFactor = 1.0f;
	
	Vector3 originalPos; // Returns to this position once camera shake has finished.
	Vector3 originalRot;
	public Vector3 Offset; // Offset for shaking.

	public int Priority; // Higher priorities allow their shake to override the current shake parameters.
	public bool useSmoothing;
	public float smoothAmount = 10;

	public bool shakeRotation = true;

	[Header ("Synchronization")]
	public bool SyncWithShaker; // Should this shaker sync with another shaker?
	public CameraShake SyncShaker; // Shaker to synchronize with.
	public float SyncMultiplier = 1; // Strength of the sync.
	
	void Awake ()
	{
		if (camTransform == null)
		{
			camTransform = GetComponent<Transform> ();
		}
	}
	
	void OnEnable ()
	{
		originalPos = camTransform.localPosition;
	}

	IEnumerator ShakeTimeOn ()
	{
		originalRot = camTransform.eulerAngles;

		while (shakeTimeRemaining > 0)
		{
			// Dont use smoothing, simply give a random position.
			if (useSmoothing == false) 
			{
				camTransform.localPosition = originalPos + (Random.insideUnitSphere * shakeAmount) + Offset;

				if (shakeRotation == true) 
				{
					originalRot = camTransform.eulerAngles;
					camTransform.transform.eulerAngles = originalRot + new Vector3 (
						originalRot.x + Random.Range (0.25f * shakeAmount, 0.25f * shakeAmount) + Offset.x,
						originalRot.y + Random.Range (0.25f * shakeAmount, 0.25f * shakeAmount) + Offset.y, 
						originalRot.z + Random.Range (-1f * shakeAmount, 1f * shakeAmount) + Offset.z
					);
				}
			}

			// Use smoothing, lerp to new points.
			if (useSmoothing == true) 
			{
				camTransform.localPosition = Vector3.Lerp (
					camTransform.localPosition, 
					originalPos + (Random.insideUnitSphere * shakeAmount) + Offset, 
					smoothAmount * Time.smoothDeltaTime
				);

				if (shakeRotation == true) 
				{
					originalRot = camTransform.eulerAngles;
					camTransform.transform.eulerAngles = originalRot + new Vector3 (
						originalRot.x + Random.Range (0.25f * shakeAmount, 0.25f * shakeAmount) + Offset.x,
						originalRot.y + Random.Range (0.25f * shakeAmount, 0.25f * shakeAmount) + Offset.y, 
						originalRot.z + Random.Range (-1f * shakeAmount, 1f * shakeAmount) + Offset.z
					);
				}
			}

			shakeTimeRemaining -= Time.deltaTime * decreaseFactor; // Decrease shake time remaining.

			yield return null;
		}

		StartCoroutine (ShakeTimeOff ());
	}

	IEnumerator ShakeTimeOff ()
	{
		Priority = 0; // Reset priority.
		shakeTimeRemaining = 0f; // Reset shake time.

		// To do when theres no shake time.
		while (shakeTimeRemaining <= 0)
		{
			camTransform.localPosition = Vector3.Lerp (
				camTransform.localPosition, 
				originalPos + Offset, 
				Time.deltaTime * smoothAmount
			);
				
			yield return null;
		}
	}

	void Update ()
	{
		// Syncing with another shaker.
		if (SyncWithShaker == true) 
		{
			// Match shake parameters.
			this.shakeDuration = SyncShaker.shakeDuration;
			this.shakeTimeRemaining = SyncShaker.shakeTimeRemaining;
			this.shakeAmount = SyncShaker.shakeAmount * SyncMultiplier;
		}
	}

	// Reset shake time to shake duration.
	public void Shake ()
	{
		shakeTimeRemaining = shakeDuration;
		StartCoroutine (ShakeTimeOn ());
	}

	// Called by other scripts to shake this.
	public void ShakeCam (float strength, float time, int priority)
	{
		// Priority must be same or higher to affect this camera shake.
		if (priority >= Priority)
		{
			shakeAmount = strength;
			shakeDuration = time;
			shakeTimeRemaining = time;
			Priority = priority;
		}
	}

	// Just overwrite the duration if needed.
	public void OverwriteCamShakeDuration (float duration)
	{
		shakeDuration = duration;
	}
}