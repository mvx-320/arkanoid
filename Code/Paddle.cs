using Godot;
using System;

namespace GA.GameProgramming
{
	public partial class Paddle : CharacterBody2D
	{
		[Export] private float _paddleSpeed = 10;
		[Export] private Node2D _ballStartPoint;
		[Export] private Sprite2D _spritePaddle;
		[Export] private Sprite2D _spriteWallLeft;
		[Export] private Sprite2D _spriteWallRight;

		private enum ControlMode
		{
			Mouse,
			Keyboard
		}
		ControlMode _controlMode = ControlMode.Keyboard;



		public Vector2 PaddleSize
		{
			get
			{
				Vector2 size = _spritePaddle.GetRect().Size;
				return size * _spritePaddle.Scale;
			}
		}
		public Vector2 WallLeftSize
		{
			get
			{
				Vector2 size = _spriteWallLeft.GetRect().Size;
				return size * _spriteWallLeft.Scale;
			}
		}
		public Vector2 WallRightSize
		{
			get
			{
				Vector2 size = _spriteWallRight.GetRect().Size;
				return size * _spriteWallRight.Scale;
			}
		}

		public float PaddleSpeed
		{
			get { return _paddleSpeed; }
		}

		public Ball Ball
		{
			get
			{
				return Level.Current.Ball;
			}
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			float deltaTime = (float)delta;
			
			// My OS has always the mouse focused on the current window, so the button wouldn't work anymore without that

			if (Input.IsActionJustPressed("MouseLeft"))
			{
				_controlMode = _controlMode == ControlMode.Keyboard ? ControlMode.Mouse : ControlMode.Keyboard;
				GD.Print($"Control mode: {_controlMode}");
			}

			if (_controlMode == ControlMode.Mouse)
			{
				Vector2 mousePosViewport = GetViewport().GetMousePosition();
				float mouseX = mousePosViewport.X;
				mouseX = ClampPaddlePosition(mouseX);
				Position = new Vector2(mouseX, Position.Y);
			}
			else
			{
				float paddleInput = Input.GetAxis("MoveLeft", "MoveRight");
				Position = GetNewPaddlePosition(deltaTime, Position, paddleInput);
			}

			if (Input.IsActionJustPressed("LaunchBall"))
			{
				GD.Print("LaunchBall action pressed");
				if (Ball != null && Ball.IsFlying == false)
				{
					Ball.Launch(Ball.Direction);
				}
			}

			if (Ball != null && Ball.IsFlying == false)
			{
				Vector2 ballPosition = _ballStartPoint.GlobalPosition;
				Ball.GlobalPosition = ballPosition;
			}
		}

		private Vector2 GetNewPaddlePosition(float deltaTime, Vector2 currentPosition, float paddleInput)
		{
			float positionX = currentPosition.X + paddleInput * _paddleSpeed * deltaTime;
			positionX = ClampPaddlePosition(positionX);

			return new Vector2(positionX, currentPosition.Y);
		}

		private float ClampPaddlePosition(float positionX)
		{
			float minX = 0 + PaddleSize.X / 2 + WallLeftSize.X;
			float maxX = GetViewportRect().Size.X - PaddleSize.X / 2 - WallRightSize.X;
			return Mathf.Clamp(positionX, minX, maxX);
		}
	}
}