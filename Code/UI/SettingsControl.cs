using Godot;
namespace GA.GameProgramming.UI
{
    public partial class SettingsControl : Control
    {

        #region Audio
        [Export] private AudioControl _masterAudioControl = null;
        [Export] private AudioControl _musicAudioControl = null;
        [Export] private AudioControl _sfxAudioControl = null;

        [Export] private string _masterBusName = null;
        [Export] private string _musicBusName = null;
        [Export] private string _sfxBusName = null;

        [Export] private string _masterDisplayName = null;
        [Export] private string _musicDisplayName = null;
        [Export] private string _sfxDisplayName = null;
        #endregion Audio
        #region Video
        [Export] private ResolutionControl _resolutionControl = null;
        [Export] private string _resolutionDisplayName = null;
        #endregion Video
        #region Buttons
        [Export] private Button _mainMenuButton = null;
        [Export] private Button _cancelButton = null;
        [Export] private Button _saveButton = null;
        [Export] private Button _okButton = null;
        #endregion Buttons
        private static readonly Vector2I DesignResolution = new(Config.MinResolutionWidth, Config.MinResolutionHeight);
        private Vector2I _windowedResolution = DesignResolution;
        private bool _changingMode = false;

        public override void _EnterTree()
        {

            if (_masterAudioControl != null)
            {
            _masterAudioControl.VolumeChanged += OnVolumeChanged;
            }
            if (_musicAudioControl != null)
            {
                _musicAudioControl.VolumeChanged += OnVolumeChanged;
            }
            if (_sfxAudioControl != null)
            {
                _sfxAudioControl.VolumeChanged += OnVolumeChanged;
            }

            if (_resolutionControl != null)
            {
                _resolutionControl.ResolutionChanged += OnResolutionChanged;
                _resolutionControl.FullscreenToggled += OnFullscreenToggled;
            }
            GetWindow().SizeChanged += OnWindowSizeChanged;

            if (_mainMenuButton != null)
            {
                _mainMenuButton.Pressed += MainMenu;
            }
            _saveButton.Pressed += Save;
            _cancelButton.Pressed += Close;

            _okButton.Pressed += Save;
            _okButton.Pressed += Close;
        }
        public override void _ExitTree()
        {
            if (_masterAudioControl != null)
            {
            _masterAudioControl.VolumeChanged -= OnVolumeChanged;
            }
            if (_musicAudioControl != null)
            {
                _musicAudioControl.VolumeChanged -= OnVolumeChanged;
            }
            if (_sfxAudioControl != null)
            {
                _sfxAudioControl.VolumeChanged -= OnVolumeChanged;
            }

            if (_resolutionControl != null)
            {
                _resolutionControl.ResolutionChanged -= OnResolutionChanged;
                _resolutionControl.FullscreenToggled -= OnFullscreenToggled;
            }
            GetWindow().SizeChanged -= OnWindowSizeChanged;

            if (_mainMenuButton != null)
            {
                _mainMenuButton.Pressed -= MainMenu;
            }
            _saveButton.Pressed -= Save;
            _cancelButton.Pressed -= Close;

            _okButton.Pressed -= Save;
            _okButton.Pressed -= Close;
        }

        public override void _Ready()
        {
            // TODO: When settings data is implemented, read this from the data.
            // For now, let's query the volume from the AudioServer.
            if (_masterAudioControl != null)
            {
                SetUpAudioControl(_masterAudioControl, _masterBusName, _masterDisplayName);
            }
            if (_musicAudioControl != null)
            {
                SetUpAudioControl(_musicAudioControl, _musicBusName, _musicDisplayName);
            }
            if (_sfxAudioControl != null)
            {
                SetUpAudioControl(_sfxAudioControl, _sfxBusName, _sfxDisplayName);
            }
            if (_resolutionControl != null)
            {
                Window window = GetWindow();
                bool isFullscreen = window.Mode == Window.ModeEnum.Fullscreen;

                _resolutionControl.Setup(_resolutionDisplayName, DesignResolution, window.Size, isFullscreen);

                _windowedResolution = isFullscreen ? DesignResolution : window.Size;
            }
        }

        private void SetUpAudioControl(AudioControl audioControl, string busName, string displayName)
        {
                int audioBusIndex = AudioServer.GetBusIndex(busName);
                if (audioBusIndex < 0) { return; }
                float volume = AudioServer.GetBusVolumeDb(audioBusIndex);
                audioControl.Setup(busName, displayName, volume);
        }

        private void OnVolumeChanged(string audioBus, float decibel)
        {
            // TODO: Add support for previewing audio and then saving (or restoring) volume seperately
            int audioBusIndex = AudioServer.GetBusIndex(audioBus);
            if (audioBusIndex < 0)
            {
                return;
            }

            AudioServer.SetBusVolumeDb(audioBusIndex, decibel);
        }

        private async void OnResolutionChanged(Vector2I resolution)
        {
            _windowedResolution = resolution;

            // DisplayServer.WindowSetSize(resolution);

            // int screen = DisplayServer.WindowGetCurrentScreen();
            // Vector2I screenPosition = DisplayServer.ScreenGetPosition(screen);
            // Vector2I screenSize = DisplayServer.ScreenGetSize(screen);
            // DisplayServer.WindowSetPosition(screenPosition + (screenSize - resolution) / 2);

            Window window = GetWindow();
            window.Size = resolution;
            window.MoveToCenter();

            // Force the content scale to be recalculated for the new window size
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            window.ContentScaleSize = DesignResolution;

            GD.Print($"{GetWindow().Size}, {DisplayServer.WindowGetSize()}, {DisplayServer.WindowGetPosition()}");
        }

        private async void OnFullscreenToggled(bool fullscreen)
        {
            Window window = GetWindow();
            _changingMode = true;

            if (fullscreen)
            {
                // window.Mode = Window.ModeEnum.Fullscreen;
                
                // await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
                // await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
                    
                // // Force the content scale to be recalculated for the new window state
                // window.ContentScaleMode = Window.ContentScaleModeEnum.Disabled;
                // window.ContentScaleMode = Window.ContentScaleModeEnum.CanvasItems;

                 // A window as big as the screen causes no resize when entering fullscreen,

                // so the content scale is not recalculated. Shrink it first.
                int screen = DisplayServer.WindowGetCurrentScreen();
                if (DisplayServer.WindowGetSize() == DisplayServer.ScreenGetSize(screen))
                {
                    // DisplayServer.WindowSetSize(DesignResolution);
                    await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
                    await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
                }

                window.Mode = Window.ModeEnum.Fullscreen;
            }
            else
            {
                window.Mode = Window.ModeEnum.Windowed;

                // Let the window finish restoring itself first, then apply our own value
                await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
                await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

                OnResolutionChanged(_windowedResolution);
                // _resolutionControl.SyncToWindow(_windowedResolution);
            }

            _changingMode = false;
            // if (fullscreen)
            // {
            //     window.Mode = Window.ModeEnum.Fullscreen;
            //     return;
            // }

            // window.Mode = Window.ModeEnum.Windowed;
            // // Get out of fullscreen is executed asynchronously.
            // // We wait a frame, otherwise the restore overwrites the size we set.
            // await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            // OnResolutionChanged(_resolutionControl.SelectedResolution);
        }

        private void OnWindowSizeChanged()
        {
            if (_changingMode) { return; }

            Window window = GetWindow();
            if (_resolutionControl != null && window.Mode == Window.ModeEnum.Windowed)
            {
                _resolutionControl.SyncToWindow(window.Size);
            }
        }
        public void MainMenu()
        {
            if (!GameManager.Instance.ChangeState(GameStateType.MainMenu))
            {
                GD.PushError("Failed to change state to MainMenu.");
            }
        }
        public void Close()
        {
            if (!GameManager.Instance.ActivatePreviousState())
            {
                GD.PushError("Failed to activate previous state.");
            }
        }
        
        public void Save()
        {
            GD.Print("Settings saved.");
        }
    }
}