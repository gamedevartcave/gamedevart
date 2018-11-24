using UnityEngine;
using InControl;

public class PlayerActions : PlayerActionSet
{
	// Movement actions.
	public PlayerAction Left;
	public PlayerAction Right;
	public PlayerAction Up;
	public PlayerAction Down;
	public PlayerTwoAxisAction Move;

	// Camera actions.
	public PlayerAction CameraChange;
	public PlayerAction CamLeft;
	public PlayerAction CamRight;
	public PlayerAction CamUp;
	public PlayerAction CamDown;
	public TwoAxisInputControl CamRot;

	public PlayerAction LockOnLeft;
	public PlayerAction LockOnRight;
	public OneAxisInputControl LockOn;

	// Main actions.
	public PlayerAction Jump;
	public PlayerAction Use;
	public PlayerAction Crouch; // This isn't used yet.

	// Attack actions.
	public PlayerAction Aim;
	public PlayerAction Shoot;
	public PlayerAction Melee;
	public PlayerAction Ability;

	// Dodge actions.
	public PlayerAction DodgeLeft;
	public PlayerAction DodgeRight;
	public OneAxisInputControl Dodge;

	// Menu actions.
	public PlayerAction Pause;
	public PlayerAction Submit;
	public PlayerAction Back;

	public PlayerActions()
	{
		Left = CreatePlayerAction ("Move Left");
		Right = CreatePlayerAction ("Move Right");
		Up = CreatePlayerAction ("Move Up");
		Down = CreatePlayerAction ("Move Down");
		Move = CreateTwoAxisPlayerAction (Left, Right, Down, Up);

		CamLeft = CreatePlayerAction ("CamLeft");
		CamRight = CreatePlayerAction ("CamRight");
		CamUp = CreatePlayerAction ("CamUp");
		CamDown = CreatePlayerAction ("CamDown");
		CamRot = CreateTwoAxisPlayerAction (CamLeft, CamRight, CamUp, CamDown);
		CameraChange = CreatePlayerAction ("CameraChange");

		LockOnLeft = CreatePlayerAction ("LockOnLeft");
		LockOnRight = CreatePlayerAction ("LockOnRight");
		LockOn = CreateOneAxisPlayerAction (Left, Right);

		Jump = CreatePlayerAction ("Jump");
		Use = CreatePlayerAction ("Use");
		Crouch = CreatePlayerAction ("Crouch");

		Aim = CreatePlayerAction ("Aim");
		Shoot = CreatePlayerAction ("Shoot");
		Melee = CreatePlayerAction ("Melee");
		Ability = CreatePlayerAction ("Ability");

		DodgeLeft = CreatePlayerAction ("DodgeLeft");
		DodgeRight = CreatePlayerAction ("DodgeRight");
		Dodge = CreateOneAxisPlayerAction (DodgeLeft, DodgeRight);

		Pause = CreatePlayerAction ("Pause");
		Submit = CreatePlayerAction ("Submit");
		Back = CreatePlayerAction ("Back");
	}

	public static PlayerActions CreateWithDefaultBindings()
	{
		var playerActions = new PlayerActions ();

		playerActions.Jump.AddDefaultBinding (Key.Space);
		playerActions.Jump.AddDefaultBinding (InputControlType.Action1);

		//playerActions.Crouch.AddDefaultBinding(Key.C);
		//playerActions.Crouch.AddDefaultBinding (InputControlType.Action2);

		playerActions.Aim.AddDefaultBinding (Mouse.RightButton);
		playerActions.Aim.AddDefaultBinding (InputControlType.LeftTrigger);

		playerActions.Use.AddDefaultBinding (Key.F);
		playerActions.Use.AddDefaultBinding (InputControlType.Action3);

		playerActions.Pause.AddDefaultBinding (Key.Escape);
		playerActions.Pause.AddDefaultBinding (InputControlType.Command);

		playerActions.Submit.AddDefaultBinding (Key.Return);
		playerActions.Submit.AddDefaultBinding (InputControlType.Action1);

		playerActions.Back.AddDefaultBinding (Key.Escape);
		playerActions.Back.AddDefaultBinding (InputControlType.Action2);

		playerActions.Shoot.AddDefaultBinding (Mouse.LeftButton);
		playerActions.Shoot.AddDefaultBinding (InputControlType.RightTrigger);

		playerActions.Melee.AddDefaultBinding (Mouse.MiddleButton);
		playerActions.Melee.AddDefaultBinding (InputControlType.Action4);

		playerActions.Ability.AddDefaultBinding (Key.LeftAlt);
		playerActions.Ability.AddDefaultBinding (InputControlType.RightStickButton);

		playerActions.DodgeLeft.AddDefaultBinding (Key.Q);
		playerActions.DodgeLeft.AddDefaultBinding (InputControlType.LeftBumper);

		playerActions.DodgeRight.AddDefaultBinding (Key.E);
		playerActions.DodgeRight.AddDefaultBinding (InputControlType.RightBumper);

		playerActions.Up.AddDefaultBinding (Key.UpArrow);
		playerActions.Up.AddDefaultBinding (Key.W);
		playerActions.Up.AddDefaultBinding (InputControlType.LeftStickUp);

		playerActions.Down.AddDefaultBinding (Key.DownArrow);
		playerActions.Down.AddDefaultBinding (Key.S);
		playerActions.Down.AddDefaultBinding (InputControlType.LeftStickDown);

		playerActions.Left.AddDefaultBinding (Key.LeftArrow);
		playerActions.Left.AddDefaultBinding (Key.A);
		playerActions.Left.AddDefaultBinding (InputControlType.LeftStickLeft);

		playerActions.Right.AddDefaultBinding (Key.RightArrow);
		playerActions.Right.AddDefaultBinding (Key.D);
		playerActions.Right.AddDefaultBinding (InputControlType.LeftStickRight);

		playerActions.CamLeft.AddDefaultBinding (Mouse.NegativeX);
		playerActions.CamLeft.AddDefaultBinding (InputControlType.RightStickLeft);

		playerActions.CamRight.AddDefaultBinding (Mouse.PositiveX);
		playerActions.CamRight.AddDefaultBinding (InputControlType.RightStickRight);

		playerActions.CamUp.AddDefaultBinding (Mouse.NegativeY);
		playerActions.CamUp.AddDefaultBinding (InputControlType.RightStickUp);

		playerActions.CamDown.AddDefaultBinding (Mouse.PositiveY);
		playerActions.CamDown.AddDefaultBinding (InputControlType.RightStickDown);

		playerActions.CameraChange.AddDefaultBinding (Key.Tab);
		playerActions.CameraChange.AddDefaultBinding (InputControlType.LeftStickButton);

		playerActions.LockOnLeft.AddDefaultBinding (Key.V);
		playerActions.LockOnLeft.AddDefaultBinding (InputControlType.DPadLeft);

		playerActions.LockOnRight.AddDefaultBinding (Key.B);
		playerActions.LockOnRight.AddDefaultBinding (InputControlType.DPadRight);

		//Debug.Log ("Created Player Actions.");
		return playerActions;
	}
}
