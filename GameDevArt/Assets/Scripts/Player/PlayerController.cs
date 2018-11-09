using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public static PlayerController instance { get; private set; }

	public int health;
	public int StartHealth = 100;
	public int MaximumHealth = 100;

	public Collider DeathBarrier;

	void Awake ()
	{
		instance = this;
	}

	void Start ()
	{
		health = StartHealth;
	}

	void OnTriggerEnter (Collider other)
	{
		if (other == DeathBarrier)
		{
			transform.position = Vector3.zero;
		}
	}
}
