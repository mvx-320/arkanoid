using Godot;
using System;

namespace GA.GameProgramming
{
	public partial class RotatingSprite : Sprite2D
	{
		[Export] private int _speed = 480;
		[Export] private float _angularSpeed = Mathf.Pi;
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			GD.Print("RotatingSprite is ready!");
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			float deltaTime = (float)delta;
			// Property of the Node
			Rotation += _angularSpeed * deltaTime;
		}
	}
}