using UnityEngine;

namespace CityBashers
{
	public class GetDistanceOptimized : MonoBehaviour
	{
		/// <summary>
		/// Just concerned about distance without heading.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		public static float GetDistance(Vector3 a, Vector3 b)
		{
			Vector3 heading;
			float distance;
			float distanceSquared;

			heading.x = a.x - b.x;
			heading.y = a.y - b.y;
			heading.z = a.z - b.z;

			distanceSquared = heading.x * heading.x + heading.y * heading.y + heading.z * heading.z;
			distance = Mathf.Sqrt(distanceSquared);
			return distance;
		}
	}
}
