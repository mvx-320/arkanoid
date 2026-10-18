using Godot;
using System;

namespace GA.GameProgramming
{
	public partial class Ball : CharacterBody2D 
	{
        [Export] private Vector2 _direction = Vector2.Zero;
		[Export] private float _speed = 100;
        [Export] private CharacterBody2D _paddle;
        [Export] private float _paddleOffsetY = -35;
        /// <summary>
        /// The maximum angle in degrees that the ball can be launched from the paddle [0, 180[.
        /// </summary>
        [Export] private float _maxLaunchAngle = 120;

        private enum BallState
        {
            OnPaddle,
            Flying
        }
        private BallState _ballState = BallState.OnPaddle;
  
        public float Speed
        {
            get { return _speed; }
        }

        public bool IsFlying
        {
            get { return _ballState == BallState.Flying; }
        }

        public Vector2 Direction
        {
            get 
            { 
                // Ball gets random lauch angle when OnPaddle
                float randomAngle = GD.Randf() * _maxLaunchAngle - _maxLaunchAngle / 2;
                float randomDirectionX = Mathf.Tan(randomAngle / 180 * Mathf.Pi);
                _direction = new Vector2(randomDirectionX, -1).Normalized();

                return _direction; 
            }
        }

        public void Launch(Vector2 direction)
        {
            if (_ballState == BallState.OnPaddle)
            {
                _ballState = BallState.Flying;
                _direction = direction.Normalized();
                Velocity = _direction * _speed;
            }
        }


        /*
		public override void _Process(double delta)
        {
            float deltaTime = (float)delta;

            if (Input.IsActionJustPressed("LaunchBall"))
            {
                _ballState = _ballState == BallState.OnPaddle ? BallState.Flying : BallState.OnPaddle;
                GD.Print($"Ball state: {_ballState}");
            }

            if (_ballState == BallState.OnPaddle)
            {

                Position = new Vector2(_paddle.Position.X, _paddle.Position.Y + _paddleOffsetY);
            }
            else if (_ballState == BallState.Flying)
            {
                KinematicCollision2D collision = MoveAndCollide(_direction * _speed * deltaTime);
                if (collision != null)
                {
                    StringName colliderName = ((Node)collision.GetCollider()).Name;
                    GD.Print($"Ball collided with: {colliderName}");
                    if (colliderName == "Paddle" || colliderName == "Top")
                    {
                        GD.Print("Ball hit paddle");
                        _direction = new Vector2(_direction.X, -_direction.Y);
                    }
                    else if (colliderName == "Right" || colliderName == "Left")
                    {
                        GD.Print("Ball hit wall");
                        _direction = new Vector2(-_direction.X, _direction.Y);
                    }
                }
            }
        }
        */

        /// <summary>
        /// If physics engine is used, physics calculations should be done here.
        /// </summary>
        public override void _PhysicsProcess(double delta)
        {
            if (_direction.IsZeroApprox())
            {
                // Ball not launched yet
                return;
            }

            float deltaTime = (float)delta;

            var collisionInfo = MoveAndCollide(Velocity * deltaTime);
            if (collisionInfo != null)
            {
                GodotObject collider = collisionInfo.GetCollider();
                if (collider != null)
                {
                    if (collider is Block block)
                    {
                        block.Hit();
                    }
                }

                _direction = _direction.Bounce(collisionInfo.GetNormal());
                Velocity = _direction * _speed;
            }   
        }
	}
}