﻿using UnityEngine;
using InControl;

public class PlayerActions : PlayerActionSet
{
	public PlayerAction Jump;
	public PlayerAction Crouch;
	public PlayerAction Aim;
	public PlayerAction Use;
	public PlayerAction Pause;
	public PlayerAction Submit;
	public PlayerAction Back;
	public PlayerAction Shoot;
	public PlayerAction Melee;
	public PlayerAction DodgeLeft;
	public PlayerAction DodgeRight;
	public OneAxisInputControl Dodge;

	public PlayerAction Left;
	public PlayerAction Right;
	public PlayerAction Up;
	public PlayerAction Down;

	public PlayerTwoAxisAction Move;

	public PlayerAction CameraChange;
	public PlayerAction CamLeft;
	public PlayerAction CamRight;
	public PlayerAction CamUp;
	public PlayerAction CamDown;

	public TwoAxisInputControl CamRot;

	public PlayerActions()
	{
		Jump = CreatePlayerAction ("Jump");
		Crouch = CreatePlayerAction ("Crouch");
		Aim = CreatePlayerAction ("Aim");
		Use = CreatePlayerAction ("Use");
		Pause = CreatePlayerAction ("Pause");
		Submit = CreatePlayerAction ("Submit");
		Back = CreatePlayerAction ("Back");

		Shoot = CreatePlayerAction ("Shoot");
		Melee = CreatePlayerAction ("Melee");
		DodgeLeft = CreatePlayerAction ("DodgeLeft");
		DodgeRight = CreatePlayerAction ("DodgeRight");
		Dodge = CreateOneAxisPlayerAction (DodgeLeft, DodgeRight);

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

		//playerActions.Use.AddDefaultBinding (Key.E);
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
		playerActions.CamUp.AddDefaultBinding (InputControlType.RightStickUp );

		playerActions.CamDown.AddDefaultBinding (Mouse.PositiveY);
		playerActions.CamDown.AddDefaultBinding (InputControlType.RightStickDown);

		playerActions.CameraChange.AddDefaultBinding (Key.Tab);
		playerActions.CameraChange.AddDefaultBinding (InputControlType.LeftStickButton);

		//Debug.Log ("Created Player Actions.");
		return playerActions;
	}
}
