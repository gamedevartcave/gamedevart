﻿using UnityEngine;

namespace CityBashers
{
	//[ExecuteInEditMode]
	public class CamPosBasedOnAngle : MonoBehaviour
	{
		public static CamPosBasedOnAngle Instance { get; private set; }

		public Vector2 offset;
		public float offsetMult = 0.1f;
		public AnimationCurve yPos;
		public AnimationCurve zPos;
		private Transform Checker;
		[ReadOnly] public Vector2 Eval;
			
		void Awake ()
		{
			Instance = this;
		}

		void Start ()
		{
			Checker = transform.parent;
		}

		void Update ()
		{
			SetPosByRotation ();
		}

		void SetPosByRotation ()
		{
			// Evaluate Y and Z
			Eval = new Vector2 (
				yPos.Evaluate (Checker.transform.localEulerAngles.x), 
				zPos.Evaluate (Checker.transform.localEulerAngles.x)
			);

			// Set new pos.
			transform.localPosition = new Vector3 (
				transform.localPosition.x,
				Eval.x + offset.x,
				-Eval.y + offset.y
			);
		}
	}
}
