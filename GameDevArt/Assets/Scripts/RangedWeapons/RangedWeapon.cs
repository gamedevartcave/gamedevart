using UnityEngine;

[CreateAssetMenu(fileName = "Ranged weapon", menuName = "Weapons/Ranged weapon", order = 1)]
public class RangedWeapon : ScriptableObject
{
	public float fireRate = 0.15f;
	public int maxAmmo = 100;
	public GameObject projectile;
}
