using UnityEngine;
using InControl;

public class PlayerActions : PlayerActionSet
{
	public PlayerAction Jump;
	public PlayerAction Crouch;

	public PlayerAction Left;
	public PlayerAction Right;
	public PlayerAction Up;
	public PlayerAction Down;

	public PlayerTwoAxisAction Move;

	public PlayerAction CamLeft;
	public PlayerAction CamRight;
	public PlayerAction CamUp;
	public PlayerAction CamDown;

	public TwoAxisInputControl CamRot;

	public PlayerActions()
	{
		Jump = CreatePlayerAction( "Jump" );
		Crouch = CreatePlayerAction ( "Crouch" );

		Left = CreatePlayerAction( "Move Left" );
		Right = CreatePlayerAction( "Move Right" );
		Up = CreatePlayerAction( "Move Up" );
		Down = CreatePlayerAction( "Move Down" );
		Move = CreateTwoAxisPlayerAction( Left, Right, Down, Up );

		CamLeft = CreatePlayerAction ( "CamLeft" );
		CamRight = CreatePlayerAction ( "CamRight" );
		CamUp = CreatePlayerAction ( "CamUp" );
		CamDown = CreatePlayerAction ( "CamDown" );
		CamRot = CreateTwoAxisPlayerAction ( CamLeft, CamRight, CamUp, CamDown );
	}

	public static PlayerActions CreateWithDefaultBindings()
	{
		var playerActions = new PlayerActions();

		playerActions.Jump.AddDefaultBinding( Key.Space );
		playerActions.Jump.AddDefaultBinding( InputControlType.Action1 );

		playerActions.Crouch.AddDefaultBinding( Key.C );
		playerActions.Crouch.AddDefaultBinding ( InputControlType.Action2 );

		playerActions.Up.AddDefaultBinding( Key.UpArrow );
		playerActions.Up.AddDefaultBinding( Key.W );
		playerActions.Up.AddDefaultBinding( InputControlType.LeftStickUp );

		playerActions.Down.AddDefaultBinding( Key.DownArrow );
		playerActions.Down.AddDefaultBinding( Key.S );
		playerActions.Down.AddDefaultBinding( InputControlType.LeftStickDown );

		playerActions.Left.AddDefaultBinding( Key.LeftArrow );
		playerActions.Left.AddDefaultBinding( Key.A );
		playerActions.Left.AddDefaultBinding( InputControlType.LeftStickLeft );

		playerActions.Right.AddDefaultBinding( Key.RightArrow );
		playerActions.Right.AddDefaultBinding( Key.D );
		playerActions.Right.AddDefaultBinding( InputControlType.LeftStickRight );

		playerActions.CamLeft.AddDefaultBinding (Mouse.NegativeX);
		playerActions.CamLeft.AddDefaultBinding (InputControlType.RightStickLeft);

		playerActions.CamRight.AddDefaultBinding (Mouse.PositiveX);
		playerActions.CamRight.AddDefaultBinding (InputControlType.RightStickRight);

		playerActions.CamUp.AddDefaultBinding (Mouse.NegativeY);
		playerActions.CamUp.AddDefaultBinding (InputControlType.RightStickUp);

		playerActions.CamDown.AddDefaultBinding (Mouse.PositiveY);
		playerActions.CamDown.AddDefaultBinding (InputControlType.RightStickDown);

		//Debug.Log ("Created Player Actions.");
		return playerActions;
	}
}
