using Godot;
using System;

namespace GA.GameProgramming
{
	public partial class CustomBall : Sprite2D
	{
		[Export] private Vector2 _direction = new Vector2(-1, -1);
		[Export] private float _speed = 100f;
		[Export] private Sprite2D[] _walls = null;
		public Vector2 Direction
		{
			get
			{
				return _direction.IsNormalized() ? _direction : _direction.Normalized();
			}
		}

		public float Speed
		{
			get
			{
				return Mathf.Clamp(_speed, Config.MinSpeed, Config.MaxSpeed);
			}
		}

		public Vector2 Velocity
		{
			get
			{
				return Direction * Speed;
			}
		}

		public override void _Process(double delta)
		{
			float deltaTime = (float)delta;
			Vector2 initialPosition = Position;
			Vector2 movement = Velocity * deltaTime;
			Position = ResolveWallCollisions(initialPosition + movement);
		}
	
		private Vector2 ResolveWallCollisions(Vector2 newPosition)
		{
			// TODO: Do collision checks with all walls here and bounce the ball if needed.
			// Bouncing here means that you calculate the new direction for the ball. Take into
			// account how far into the wall the ball ended up and use that distance in the
			// bounce vector.
			for (int i = 0; i < _walls.Length; i++)
			{
				Sprite2D wall = _walls[i];
				Vector2 wallCenter = wall.Position;
				Vector2 wallExtents = wall.Texture.GetSize() * wall.Scale * 0.5f;

				Hit hit = CustomPhysics.IntersectsCircle(wallCenter, wallExtents, newPosition, 10f);

				if (hit == null)
				{
					continue;
				}

				newPosition += hit.Penetration;
				_direction = CustomPhysics.Bounce(_direction, hit.Normal);
			}

			return newPosition;
		}
	}

}