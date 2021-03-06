﻿using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace CityBashers
{
	public class WorldToScreenPoint : MonoBehaviour 
	{
		[Header ("World")]
		public Transform WorldObject;
		public Renderer worldMeshRend;
		public Vector3 worldOffset;
		public bool updateWhenOffScreen;
		public float coneAngle = 60;
		[ReadOnly] public bool IsInView;
		public float viewWaitTime = 0.5f;
		private WaitForSeconds viewWait;

		private Vector2 ViewportPosition;
		private Vector2 WorldObject_ScreenPosition;

		[Header ("Distance")]
		public bool showDistance;
		public Vector2 distanceRange = new Vector2 (1, 10000);
		public Transform distanceReferencePos;
		[ReadOnly] public float currentDistance;
		public TextMeshProUGUI distanceText;
		public WaitForSeconds DistanceUpdate;
		public float distanceUpdateTime = 0.5f;
		[Range (0, 2)] public int decimalPlaces;
		public bool useSmoothing;
		public float smoothAmount = 10;

		[Header ("UI")]
		public RectTransform CanvasRect;
		public RawImage UI_Element_Image;
		public RectTransform UI_Element;
		public Vector2 screenOffset;

		[Header ("Screen bounds")]
		public Vector2 xBounds = new Vector2 (-960, 960);
		public Vector2 yBounds = new Vector2 (-540, 540);

		public UnityEvent OnBecameVisible;
		public UnityEvent OnBecameInvisible;

		void Start ()
		{
			DistanceUpdate = new WaitForSeconds (distanceUpdateTime);

			if (distanceReferencePos != null)
			{
				//currentDistance = Vector3.Distance (distanceReferencePos.position, WorldObject.position);
				currentDistance = GetDistanceOptimized.GetDistance(distanceReferencePos.position, WorldObject.position);
			}

			viewWait = new WaitForSeconds(viewWaitTime);
			StartCoroutine(EvaluateView ());
		}

		void GetDistance()
		{
		}

		void OnEnable ()
		{
			StartCoroutine (UpdateDistance ());
		}

		IEnumerator EvaluateView()
		{
			while (true)
			{
				bool lastViewState = IsInView; // Get last visible state.
				IsInView = ViewTester.Instance.TestCone(WorldObject.position, coneAngle);

				// A change in Visible state.
				if (lastViewState != IsInView)
				{
					// Became visible.
					if (IsInView == true)
					{
						OnBecameVisible.Invoke();
						//Debug.Log("Became visible");
					}

					else // Became invisible.

					{
						OnBecameInvisible.Invoke();
						//Debug.Log("Became invisible");
					}
				}

				yield return viewWait;
			}
		}
			
		IEnumerator UpdateDistance ()
		{
			//while (showDistance == true)
			while (true)
			{
				DoUpdateDistance ();
				yield return DistanceUpdate;
			}
		}

		void Update ()
		{
			// Note: Checks any camera (including Editor camera).
			if ((IsInView || updateWhenOffScreen) && 
				GameController.Instance.isPaused == false &&
				Camera.main != null)
			{
				// Calculate the position of the UI element
				// 0,0 for the canvas is at the center of the screen, 
				// whereas WorldToViewPortPoint treats the lower left corner as 0,0. 
				// Because of this, you need to subtract the height / width of the canvas * 0.5f to get the correct position.

				//Debug.Log(IsInView);
				ViewportPosition = Camera.main.WorldToViewportPoint (WorldObject.position + worldOffset);

				WorldObject_ScreenPosition = new Vector2 (
					Mathf.Clamp (
						((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)), 
						xBounds.x, 
						xBounds.y
					),

					Mathf.Clamp (
						((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)), 
						yBounds.x, 
						yBounds.y
					)
				);

				// Set the position of the UI element.
				UI_Element.anchoredPosition = useSmoothing ? 
					Vector2.Lerp (
						UI_Element.anchoredPosition, 
						WorldObject_ScreenPosition + screenOffset, 
						Time.deltaTime * smoothAmount
					) : 
					WorldObject_ScreenPosition + screenOffset;

				if (showDistance == false) // Showing distance.
				{
					if (distanceReferencePos != null)
					{
						// Update current distance.
						currentDistance = Vector3.Distance (distanceReferencePos.position, WorldObject.position);
					}
				} 

				if (currentDistance > distanceRange.x && currentDistance < distanceRange.y)
				{
					if (UI_Element_Image != null)
					{
						if (UI_Element_Image.enabled == false)
						{
							UI_Element_Image.enabled = true;
						}
					}
				} 

				else // Outside range, hide.

				{
					if (UI_Element_Image != null)
					{
						if (UI_Element_Image.enabled == true)
						{
							UI_Element_Image.enabled = false;
						}
					}
				}
			} 

			else // Object is not visible on camera.
			
			{
				if (UI_Element_Image != null)
				{
					if (UI_Element_Image.enabled == true)
					{
						UI_Element_Image.enabled = false;
					}
				}

				if (showDistance == true)
				{
					
				} 

				else
				
				{
					if (distanceText != null)
					{
						distanceText.text = "";
					}
				}
			}
		}

		void DoUpdateDistance ()
		{
			if (showDistance == true) // Allow updating distance.
			{
				if (worldMeshRend.isVisible == true) // Object is rendered on the screen.
				{
					// Update current distance.
					currentDistance = Vector3.Distance (distanceReferencePos.position, WorldObject.position);

					// If current distance is out of distance range.
					if (currentDistance < distanceRange.x || currentDistance > distanceRange.y)
					{
						if (distanceText != null)
						{
							distanceText.text = "";
						}
							
						if (UI_Element_Image != null)
						{
							UI_Element_Image.enabled = false;
						}
					} 

					else // If current distance is greater than 1m.
					
					{
						if (currentDistance < 1000) // Use m units.
						{
							distanceText.text =  Math.Round (currentDistance, decimalPlaces).ToString () + " m";
						} 

						else // Use km units.
						
						{
							distanceText.text = Math.Round (currentDistance * 0.001f, decimalPlaces).ToString () + " km";
						}
					}
				} 

				else // Object is not visible by a camera, hide distance text and UI element.
				
				{
					if (distanceText != null)
					{
						distanceText.text = "";
					}
						
					if (UI_Element_Image != null)
					{
						UI_Element_Image.enabled = false;
					}
				}
			}

			else // Use empty string for distance text.
			
			{
				if (distanceText != null)
				{
					distanceText.text = "";
				}
			}
		}
	}
}
