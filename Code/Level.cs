using Godot;
using System;
using System.Collections.Generic;
using GA.Common;

namespace GA.GameProgramming
{
	public partial class Level : Node2D
	{
		#region Statics
		public static Level Current
		{
			get;
			private set;
		}
		#endregion Statics


		[Export] private Paddle _paddle = null;
		[Export] private Ball _ball = null;
		[Export] private int _wallWidth = 0;
		// [Export] private Node2D _levelContent = null;
		[Export] private LevelBlockMapping _levelBlockMapping = null;
		[Export] private int _levelId = 1; // TODO: Move this to GameManager!


		public Paddle Paddle { get { return _paddle; } }
		public Ball Ball { get { return _ball; } }
		public int WallWidth { get { return _wallWidth; } }

		public Vector2 WindowSize
		{
			get
			{
				return GetViewport().GetVisibleRect().Size;
			}
		}

        public override void _EnterTree()
        {
            GameManager.Instance.LivesChanged += OnLivesChanged;
        }
        public override void _ExitTree()
        {
			GameManager.Instance.LivesChanged -= OnLivesChanged;
        }
		private void OnLivesChanged(int lives)
		{
			if (lives <= 0)
			{
				// Game over, so reset the level.
				GD.Print("Game Over! Resetting the level.");
				if (Ball != null)
				{
					Ball.QueueFree();
					_ball = null;
				}
			}
		}

		public override void _Ready()
		{
			Current = this;

			if (_paddle == null)
			{
				_paddle = this.GetNode<Paddle>();
				// Same as this
				// _paddle = NodeExtensions.GetNode<Paddle>(this);
			}

			if (_ball == null)
			{
				_ball = this.GetNode<Ball>("Ball");
			}
			
			InstantiateLevel();
			/*
			IList<Ball> balls = this.GetNodes<Ball>(recursive:true);
			GD.Print($"Found {balls.Count} balls.");
			*/

			// GameManager.Instance.Respawn += _ball.Reset;
			// GameManager.Instance.GameOver += OnGameOver;

			// PackedScene scene = GD.Load<PackedScene>("res://Scenes/Levels/LevelContent.tscn");
			// Node content = scene.Instantiate();
			// AddChild(content);
		}

        public override void _Input(InputEvent @event)
		{
			if (@event.IsActionPressed(Config.PauseActionName))
			{
				GameManager.Instance.ChangeState(GameStateType.Pause);
			}
		}

		private void InstantiateLevel()
		{
			if (_levelBlockMapping != null)
			{
				PackedScene levelBlocksScene = _levelBlockMapping.GetLevelBlocks(_levelId);
				if (levelBlocksScene != null)
				{
					Node2D levelBlocksInstance = levelBlocksScene.Instantiate() as Node2D;
					if (levelBlocksInstance != null)
					{
						AddChild(levelBlocksInstance);
					}
					else
					{
						GD.PrintErr($"Failed to instantiate level blocks for level {_levelId}.");
					}
				}
				else
				{
					GD.PrintErr($"No level blocks found for level {_levelId}.");
				}
			}
			else
			{
				GD.PrintErr("LevelBlockMapping is not assigned.");
			}
		}

		
		private void OnGameOver()
		{
			_ball.QueueFree();
			_paddle.QueueFree();
			GD.Print("Game Over!");
		}
	}
}
