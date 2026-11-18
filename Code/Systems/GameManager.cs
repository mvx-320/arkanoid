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

		private AudioStreamPlayer _musicPlayer = null;
		private MusicMap _musicMap = null;
		private MusicType _currentMusic = MusicType.None;
		private readonly Dictionary<MusicType, float> _musicPlaybacks = new();
		
		private int _score = 0;
		private int _lives = Config.StartLives;
		
		private SceneTree _sceneTree = null;
		// Containes all states that are currently loaded.
		// TODO: Use Dictionary instead.
		private List<GameStateBase> _gameStates = new List<GameStateBase>
		{
			new MainMenuState(),
			new InGameState(),
			new SettingsState(),
			new GameOverState(),
			new PauseState(),
			new WinState(),
		};

		private GameStateBase _nextState = null;
  
		// GameStateBase mainMenu = GetState(GameStateTypeMainMenu
		public Stack<GameStateBase> LoadedStates { get; private set; } = new Stack<GameStateBase>();
		public GameStateBase ActiveState
		{
			get { return LoadedStates.Count > 0 ? LoadedStates.Peek() : null; }
		}

		public Settings Settings { get; private set; } = null;
		
		public int CurrentLevelId
		{
			get; 
			set;
		} = 1;
  
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

		public static Vector2I MinWindowSize { get; private set; }

		public override void _Ready()
		{
			// HACK: Initialize the first state
			// TODO: Replace this with a proper implementation

			MinWindowSize = GetWindow().Size;

			// Override currently loaded scene.
			// TODO: Keep Godot's SceneTree up-to-date with currently active scene.
			SceneTree.CurrentScene.QueueFree();

			GameStateBase mainMenu = GetState(GameStateType.MainMenu);
			mainMenu.OnEnter();
			LoadedStates.Push(mainMenu);

			Settings = new Settings();


			// TODO: Don't call this when loading the game from a save file.
			Score = 0;
			Lives = Config.StartLives;

			_musicMap = ResourceLoader.Load<MusicMap>("res://Data/MusicMap.tres");
			_musicPlayer = new AudioStreamPlayer
			{
				Name = "MusicPlayer",
				Bus = "Music",
				ProcessMode = ProcessModeEnum.Always,
			};
			AddChild(_musicPlayer);
			UpdateMusic();
			_musicPlayer.Finished += OnMusicFinished;

			// To start the game InGame
			//ChangeState(GameStateType.InGame, force: true);
		}

		private void OnMusicFinished()
		{
			_musicPlayer.Play();
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

			_nextState = nextState;

			CallDeferred(nameof(DelayChangeState), force);
			// if (nextState.IsAdditive)
			// {
			// 	// The new state is shown on top of the surrent one, which stays loaded.
			// 	ActiveState?.OnExit(keepLoaded: true);
			// }
			// else
			// {
			// 	while (LoadedStates.Count > 0)
			// 	{
			// 		GameStateBase poppedState = LoadedStates.Pop();
			// 		poppedState.OnExit(keepLoaded: false);
			// 	}
			// }

			// nextState.OnEnter(force);
			// LoadedStates.Push(nextState);

			return true;
		}

		private void DelayChangeState(bool force)
		{
			if (_nextState.IsAdditive)
			{
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

			LoadedStates.Push(_nextState);
			_nextState.OnEnter(force);

			_nextState = null;

			UpdateMusic();
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

			UpdateMusic();

			// Activate the previous state, which is now on top of the stack.
			return true;
		}

		public bool StartNewGame()
		{

			if (!ChangeState(GameStateType.InGame, force: true))
			{
				GD.PushError("Failed to start a new game.");
				return false;
			}
			CurrentLevelId = 1;

			Score = 0;
			Lives = Config.StartLives;
			// The level scene is added deferred in OnEnter. A deferred call queued
			// after it runs once the scene (and its HUD) is in the tree.
			// Callable.From(ResetGameData).CallDeferred();

			// If InGame music should always start from the beginning
			_musicPlaybacks.Remove(MusicType.InGame); 

			return true;
		}

		// private void ResetGameData()
		// {
		// 	Score = 0;
		// 	Lives = Config.StartLives;
		// }

		public void CompleteLevel(bool isLastLevel)
		{
			if (isLastLevel)
			{
				ChangeState(GameStateType.Win);
				return;
			}

			CurrentLevelId++;
			ChangeState(GameStateType.InGame, force: true);
		}

		public void AddScore(int scoreToAdd)
		{
			if (scoreToAdd <= 0) { return; }

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

		private void UpdateMusic()
		{
			MusicType wanted = ActiveState?.Music ?? MusicType.None;
			if (wanted == MusicType.None || wanted == _currentMusic) return;

			AudioStream stream = _musicMap?.GetStream(wanted);
			if (stream == null)
			{
				GD.PushError($"No music assigned for {wanted}.");
				return;
			}

			if (_currentMusic != MusicType.None)
			{
				_musicPlaybacks[_currentMusic] = _musicPlayer.GetPlaybackPosition();
			}

			_currentMusic = wanted;
			_musicPlayer.Stream = stream;
			_musicPlayer.Play(_musicPlaybacks.GetValueOrDefault(wanted, 0));
		}
	}
}