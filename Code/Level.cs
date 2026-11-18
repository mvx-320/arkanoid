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

		private int _blockCount = 0;

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

		public EffectPlayer EffectPlayer
		{
			get;
			private set;
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

			EffectPlayer = this.GetNode<EffectPlayer>();
			if (EffectPlayer == null)
			{
				GD.PushError("No EffectPlayer found from the scene.");
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
			if (_levelBlockMapping == null)
			{
				GD.PrintErr("LevelBlockMapping is not assigned.");
				return;
			}

			int levelId = GameManager.Instance.CurrentLevelId;

			PackedScene levelBlocksScene = _levelBlockMapping.GetLevelBlocks(levelId);
			if (levelBlocksScene == null)
			{
				GD.PrintErr($"No level blocks found for level {levelId}.");
				return;
			}

			Node2D levelBlocksInstance = levelBlocksScene.Instantiate() as Node2D;
			if (levelBlocksInstance == null)
			{
				GD.PrintErr($"Failed to instantiate level blocks for level {_levelId}.");
				return;
			}
			AddChild(levelBlocksInstance);
		
			foreach (Block block in levelBlocksInstance.GetNodes<Block>(recursive: true))
			{
				_blockCount++;
				block.BlockDestroyed += OnBlockDestroyed;
			}
			GD.Print($"Loaded blocks for level {levelId}, block count: {_blockCount}");
		}

		private void OnBlockDestroyed(Block block)
		{
			_blockCount--;
			if (_blockCount > 0) return;

			bool isLastLevel = !_levelBlockMapping.HasLevel(GameManager.Instance.CurrentLevelId + 1);
			GD.Print($"Level {GameManager.Instance.CurrentLevelId}, is {(isLastLevel ? "" : "not")} the last level.");
			GameManager.Instance.CompleteLevel(isLastLevel);
		}
		
		private void OnGameOver()
		{
			_ball.QueueFree();
			_paddle.QueueFree();
			GD.Print("Game Over!");
		}
	}
}
