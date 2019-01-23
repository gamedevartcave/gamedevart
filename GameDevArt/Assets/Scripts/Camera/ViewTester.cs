using UnityEngine;

namespace CityBashers
{
	public class ViewTester : MonoBehaviour
	{
		public static ViewTester Instance { get; private set; }

		private void Awake()
		{
			Instance = this;
		}

		public bool TestCone(Vector3 inputPoint, float cutoffAngle)
		{
			// 1 if paralel
			// 0 if perpendicular
			// -1 if reverse parallel
			float cosAngle = Vector3.Dot(
				(inputPoint - transform.position).normalized,
				transform.forward);
			float angle = Mathf.Acos(cosAngle) * Mathf.Rad2Deg;
			//Debug.Log(angle);
			return angle < cutoffAngle;
		}
	}
}
