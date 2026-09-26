using Godot;
using System;

namespace GA.GameProgramming
{
	public partial class Paddle : CharacterBody2D
	{
		[Export] private float _paddleSpeed = 10;
		[Export] private Sprite2D _sprite;
		private bool _mouseControl = false;

		private enum ControlMode
		{
			Mouse,
			Keyboard
		}

		public Vector2 Size
		{
			get
			{
				Vector2 size = _sprite.GetRect().Size;
				return size * _sprite.Scale;
			}
		}
		public float PaddleSpeed
		{
			get { return _paddleSpeed; }
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			float deltaTime = (float)delta;
			
			// My OS has always the mouse focused on the current window, so the button wouldn't work anymore without that

			if (Input.IsActionJustPressed("MouseLeft"))
			{
				_mouseControl = !_mouseControl;
				GD.Print($"Mouse control toggled: {_mouseControl}");
			}

			if (_mouseControl)
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
			// GetNode is not nice to use
			var collision = GetNode<CollisionShape2D>("CollisionShape2D");
			var capsule = collision.Shape as CapsuleShape2D;
			float shapeWidth = capsule.Height; // capsule is rotated 90°C

			return Mathf.Clamp(positionX, 0 + shapeWidth / 2, GetViewportRect().Size.X - shapeWidth / 2);
		}
	}
}