using Godot;
using System.Collections.Generic;
using System;

namespace GA.GameProgramming.UI
{
	public partial class ResolutionControl : VBoxContainer
	{
		[Signal] public delegate void ResolutionChangedEventHandler(Vector2I resolution);
		[Signal] public delegate void FullscreenToggledEventHandler(bool fullscreen);
		[Export] private Label _nameLabel = null;
		[Export] private OptionButton _resolutionOption = null;
		[Export] private CheckBox _fullscreenCheckBox = null;

		private readonly List<Vector2I> _resolutions = new();

		public Vector2I SelectedResolution
		{
			get
			{
				if (_resolutionOption == null || _resolutionOption.Selected < 0)
				{
					return Vector2I.Zero;
				}
				return _resolutions[_resolutionOption.Selected];
			}
		}

        public override void _EnterTree()
		{
			if (_resolutionOption != null)
			{
				_resolutionOption.ItemSelected += OnItemSelected; 
			}
			if (_fullscreenCheckBox != null)
			{
				_fullscreenCheckBox.Toggled += OnFullscreenToggled;
			}
		}

		public override void _ExitTree()
		{
			if (_resolutionOption != null)
			{
				_resolutionOption.ItemSelected -= OnItemSelected;
			}
			if (_fullscreenCheckBox != null)
			{
				_fullscreenCheckBox.Toggled -= OnFullscreenToggled;
			}
		}

        public void Setup(string displayName, Vector2I designSize, Vector2I currentSize, bool isFullscreen)
		{
			if (_nameLabel != null)
			{
				_nameLabel.Text = displayName;
			}

			FillResolutions(designSize, currentSize);
			SetFullscreen(isFullscreen);
		}

		public void SyncToWindow(Vector2I actualSize)
		{
			if (_resolutionOption == null) { return; }

			int index = _resolutions.IndexOf(actualSize);
			if (index >= 0)
			{
				_resolutionOption.Select(index);
			}
		}

		private void FillResolutions(Vector2I designSize, Vector2I currentSize)
		{
			if (_resolutionOption == null) { return; }

			_resolutions.Clear();
			_resolutionOption.Clear();

			Vector2I screenSize = DisplayServer.ScreenGetSize();
			int max = Mathf.Min(screenSize.X / designSize.X, screenSize.Y / designSize.Y);
			max = Mathf.Max(max, 1);

			for (int scale = 1; scale <= max; scale++)
			{
				Vector2I resolution = designSize * scale;
				_resolutions.Add(resolution);
				_resolutionOption.AddItem($"{resolution.X} x {resolution.Y}");
			}

			int currentIndex = _resolutions.IndexOf(currentSize);
			_resolutionOption.Select(
				currentIndex >= 0 ? currentIndex : 0
			);
		}

		private void SetFullscreen(bool fullscreen)
		{
			_fullscreenCheckBox.SetPressedNoSignal(fullscreen);

			if (_resolutionOption != null)
			{
				_resolutionOption.Disabled = fullscreen;
			}
		}

		private void OnItemSelected(long index)
		{
			EmitSignal(SignalName.ResolutionChanged, _resolutions[(int) index]);
		}

		private void OnFullscreenToggled(bool isPressed)
		{
			if (_resolutionOption != null)
			{
				_resolutionOption.Disabled = isPressed;
			}

			EmitSignal(SignalName.FullscreenToggled, isPressed);
		}
	}
}