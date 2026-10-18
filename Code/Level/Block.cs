using Godot;
using System;

namespace GA.GameProgramming
{
	public partial class Block : StaticBody2D
	{
		[Signal] public delegate void BlockDestroyedEventHandler(Block block);
		[Export] private int _score = 10;
		public void Hit()
		{
			// Should be called whenever a ball hits the block. 
			// For now all blocks are destroyed in one hit.
			// TODO: Implement health functionality to enable multiple hit support.
			GD.Print($"Block hit! Score: {_score}");
		
			GameManager.Instance.AddScore(_score);
			EmitSignal(SignalName.BlockDestroyed);
			QueueFree();
		}
	}
}