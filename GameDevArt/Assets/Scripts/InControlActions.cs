using UnityEngine;

public class InControlActions : MonoBehaviour 
{
	public static InControlActions instance { get; private set; }

	public PlayerActions playerActions;

	void Awake ()
	{
		instance = this;
		playerActions = PlayerActions.CreateWithDefaultBindings ();
	}
}
