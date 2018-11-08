using UnityEngine;
using InControl;

public class PlayerActions : PlayerActionSet
{
	public PlayerAction Jump;

	public PlayerAction Left;
	public PlayerAction Right;
	public PlayerAction Up;
	public PlayerAction Down;
	public PlayerTwoAxisAction Move;

	public PlayerActions()
	{
		Jump = CreatePlayerAction( "Jump" );
		Left = CreatePlayerAction( "Move Left" );
		Right = CreatePlayerAction( "Move Right" );
		Up = CreatePlayerAction( "Move Up" );
		Down = CreatePlayerAction( "Move Down" );
		Move = CreateTwoAxisPlayerAction( Left, Right, Down, Up );
	}

	public static PlayerActions CreateWithDefaultBindings()
	{
		var playerActions = new PlayerActions();

		playerActions.Jump.AddDefaultBinding( Key.Space );
		playerActions.Jump.AddDefaultBinding( InputControlType.Action1 );

		//playerActions.Up.AddDefaultBinding( Key.UpArrow );
		//playerActions.Down.AddDefaultBinding( Key.DownArrow );
		//playerActions.Left.AddDefaultBinding( Key.LeftArrow );
		//playerActions.Right.AddDefaultBinding( Key.RightArrow );

		//playerActions.Left.AddDefaultBinding( InputControlType.LeftStickLeft );
		//playerActions.Right.AddDefaultBinding( InputControlType.LeftStickRight );
		//playerActions.Up.AddDefaultBinding( InputControlType.LeftStickUp );
		//playerActions.Down.AddDefaultBinding( InputControlType.LeftStickDown );

		//Debug.Log ("Created Player Actions.");
		return playerActions;
	}
}
