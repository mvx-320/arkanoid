using Godot;
using System;

namespace GA.GameProgramming
{
	public partial class GameManager : Node
	{
		# region Singleton
		public static GameManager Instance 
		{ 
			get; 
			private set; 
		}
		public sealed override void _Ready()
		{
			GD.Print("Initializing GameManager");
			if (Instance == null)
			{
				Instance = this;
			}
			else if (Instance != this)
			{
				QueueFree();
				return;
			}
			Initialize();
		}
		# endregion Singleton

		[Signal] public delegate void ScoreChangedEventHandler(int newScore);
		[Signal] public delegate void LivesChangedEventHandler(int newLives);
		[Signal] public delegate void RespawnEventHandler();
		[Signal] public delegate void GameOverEventHandler();
		
		private int _score = 0;
		private int _lives = 3;
  
		public int Score
		{
			get { return _score; }
			private set
			{
				// TODO: Validate the score value.
				_score = value;
				EmitSignal(SignalName.ScoreChanged, _score);
			}
		}

		public int Lives
		{
			get { return _lives; }
			private set
			{
				_lives = value;
				GD.Print($"Lives changed to {_lives}");
				EmitSignal(SignalName.LivesChanged, _lives);
			}
		}

		protected virtual void Initialize()
		{
			GD.Print("GameManager initialized");
		}

		public void AddScore(int scoreToAdd)
		{
			if (scoreToAdd < 0) { return; }

			Score += scoreToAdd;
		}

		public void DecrementLives()
		{
			Lives--;

			if (Lives > 0)
			{
				EmitSignal(SignalName.Respawn);
			}
			else
			{
				EmitSignal(SignalName.GameOver);
			}

		}
	}
}