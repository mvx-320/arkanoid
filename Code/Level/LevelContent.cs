using Godot;
using System;

public partial class LevelContent : Node2D
{
	[Export] private StaticBody2D[] blocks = new StaticBody2D[6];
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
