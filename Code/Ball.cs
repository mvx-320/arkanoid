using Godot;
using System;

namespace GA.GameProgramming
{
	public partial class Ball : Node2D 
	{
		[Export] private float _speed = 10;
  
        public float Speed
        {
            get { return _speed; }
        }


		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
        {
            float deltaTime = (float)delta;

            /*
            float horizontal = Input.GetAxis("Left", "Right");
            float vertical = Input.GetAxis("Up", "Down");
            */

            Vector2 input = Input.GetVector("Left", "Right", "Up", "Down");

            Position += input * _speed * deltaTime;
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