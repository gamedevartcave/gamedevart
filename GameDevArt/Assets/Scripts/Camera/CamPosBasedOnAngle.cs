using UnityEngine;

namespace CityBashers
{
	//[ExecuteInEditMode]
	public class CamPosBasedOnAngle : MonoBehaviour
	{
		public AnimationCurve yPos;
		public AnimationCurve zPos;
		private Transform Cam;
		private Transform Checker;
		private Vector2 Eval;
			
		void Start ()
		{
			Cam = transform;
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
			Cam.transform.localPosition = new Vector3 (
				Cam.transform.localPosition.x,
				Eval.x,
				-Eval.y
			);
		}
	}
}
