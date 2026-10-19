using Godot;
using System;

namespace GA.GameProgramming
{
	public partial class DeathZone : StaticBody2D
	{
		[Signal]
		public delegate void DeathZoneHitEventHandler();


		public void Hit()
		{
			// Should be called whenever a ball hits the death zone. 
			GD.Print($"Ball hit death zone!");
			
			GameManager.Instance.DecrementLives();
			EmitSignal(SignalName.DeathZoneHit);
		}
	}
}