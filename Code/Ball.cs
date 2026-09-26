using Godot;
using System;

namespace GA.GameProgramming
{
	public partial class Ball : CharacterBody2D 
	{
        [Export] private Vector2 _direction = new Vector2(0, 1);
		[Export] private float _speed = 100;
  
        public float Speed
        {
            get { return _speed; }
        }


		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
        {
            float deltaTime = (float)delta;

            var collision = MoveAndCollide(_direction * _speed * deltaTime);
            if (collision != null)
            {
                var colliderName = ((Node)collision.GetCollider()).Name;
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

        /// <summary>
        /// If physics engine is used, physics calculations should be done here.
        /// </summary>
        public override void _PhysicsProcess(double delta)
        {
            base._PhysicsProcess(delta);
        }
	}
}