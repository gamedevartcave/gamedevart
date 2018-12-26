using UnityEngine;

namespace CityBashers
{
	[CreateAssetMenu(fileName = "Ranged weapon", menuName = "Player/Weapons/Ranged weapon", order = 1)]
	public class RangedWeapon : ScriptableObject
	{
		public float fireRate = 0.15f;
		public int maxAmmo = 100;
		public GameObject projectile;
	}
}
