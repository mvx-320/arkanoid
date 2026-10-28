using GA.GameProgramming.State;
using Godot;
using System;
using System.Collections.Generic;

namespace GA.GameProgramming
{
	/// <summary>
	/// Contains game's data and acts as the main state machine
	/// </summary>
	public partial class GameManager : Node
	{
		# region Singleton
		public static GameManager Instance 
		{ 
			get; 
			private set; 
		}
		
		public GameManager()
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
		}
		# endregion Singleton

		[Signal] public delegate void ScoreChangedEventHandler(int newScore);
		[Signal] public delegate void LivesChangedEventHandler(int newLives);
		[Signal] public delegate void RespawnEventHandler();
		[Signal] public delegate void GameOverEventHandler();
		
		private int _score = 0;
		private int _lives = Config.StartLives;
		
		private SceneTree _sceneTree = null;
		// Containes all states that are currently loaded.
		private Stack<GameStateBase> _loadedStates = new Stack<GameStateBase>();
		// TODO: Use Dictionary instead.
		private List<GameStateBase> _gameStates = new List<GameStateBase>
		{
			new MainMenuState(),
			new InGameState(),
			new SettingsState(),
			new GameOverState(),
		};

  
		// GameStateBase mainMenu = GetState(GameStateTypeMainMenu
		public Stack<GameStateBase> LoadedStates { get; private set; } = new Stack<GameStateBase>();
		public GameStateBase ActiveState
		{
			get { return LoadedStates.Count > 0 ? LoadedStates.Peek() : null; }
		}
  
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
				_lives = Mathf.Clamp(value, 0, Config.MaxLives);
				EmitSignal(SignalName.LivesChanged, _lives);
				GD.Print($"Lives changed to {_lives}");
			}
		}

		public SceneTree SceneTree
		{
			get
			{
				// Cache the SceneTree reference f
				if (_sceneTree == null)
				{
					_sceneTree = GetTree();
				}

				return _sceneTree;
			}
		}

		public override void _Ready()
		{
			// HACK: Initialize the first state
			// TODO: Replace this with a proper implementation

			// Override currently loaded scene.
			// TODO: Keep Godot's SceneTree up-to-date with currently active scene.
			SceneTree.CurrentScene.QueueFree();

			GameStateBase mainMenu = GetState(GameStateType.MainMenu);
			mainMenu.OnEnter();
			LoadedStates.Push(mainMenu);
		}

		/// <summary>
		/// Finds and returns the state matching with the stateType.
		/// </summary>
		/// <param name="stateType"></param>
		/// <returns>The state or null, if state is not found</returns>
		private GameStateBase GetState(GameStateType stateType)
		{
			foreach (var state in _gameStates)
			{
				if (state.StateType == stateType)
				{
					return state;
				}
			}

			return null;
		}

		/// <summary>
		/// Changes to current game state to a new state
		/// Note: Don't use this to transition to te previous state. For that, use the ActivatePreviousState() method.
		/// </summary>
		/// <param name="targetStateType">The new state to transition to.</param>
		/// <param name="force">Is state activation forced or not.</param>
		/// <returns>True if the state was changed, false otherwise.</returns>
		public bool ChangeState(GameStateType targetStateType, bool force = false)
		{
			// GameManager.ChangeState, first line (temporary)
			GD.Print($"ChangeState {ActiveState?.StateType} -> {targetStateType}, stack: {LoadedStates.Count}");


			// Is the ransition from the current state to the target state valid?
			if (ActiveState != null && !ActiveState.CanTransitionTo(targetStateType))
			{
				GD.PushError($"Transition from {ActiveState.StateType} to {targetStateType} is not allowed.");
				return false;
			}

			GameStateBase nextState = GetState(targetStateType);
			if (nextState == null)
			{
				GD.PushError($"State {targetStateType} not found.");
				return false;
			}

			if (nextState.IsAdditive)
			{
				// The new state is shown on top of the surrent one, which stays loaded.
				ActiveState?.OnExit(keepLoaded: true);
			}
			else
			{
				while (LoadedStates.Count > 0)
				{
					GameStateBase poppedState = LoadedStates.Pop();
					poppedState.OnExit(keepLoaded: false);
				}
			}

			nextState.OnEnter(force);
			LoadedStates.Push(nextState);

			return true;
		}

		/// <summary>
		/// Activates the previous game state.
		/// TODO: Refactor ChangeState to support this corner case and get rid of this method.
		/// </summary>
		/// <returns>True if the previous state was activated, false otherwise.</returns>
		public bool ActivatePreviousState()
		{
			if (LoadedStates.Count < 2)
			{
				GD.PushError("No previous state to activate.");
				return false;
			}

			// Pop the current state from the stack.
			GameStateBase previousState = LoadedStates.Pop();
			previousState.OnExit(keepLoaded: false);

			ActiveState.OnEnter();

			// Activate the previous state, which is now on top of the stack.
			return true;
		}

		public bool StartNewGame()
		{
			Score = 0;
			Lives = Config.StartLives;

			if (!ChangeState(GameStateType.InGame, force: true))
			{
				GD.PushError("Failed to start a new game.");
				return false;
			}

			return true;
		}

		public void AddScore(int scoreToAdd)
		{
			if (scoreToAdd < 0) { return; }

			Score += scoreToAdd;
		}
		
		public void IncrementLives()
		{
			Lives++;
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
				ChangeState(GameStateType.GameOver);
			}

		}
	}
}