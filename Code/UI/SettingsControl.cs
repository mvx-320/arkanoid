using Godot;
using System;
using System.Collections.Generic;

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
        [Export] private Button _fullscreenButton = null;
        [Export] private OptionButton _resolutionsButton = null;
        private Dictionary<int, Resolution> _resolutions = new Dictionary<int, Resolution>();
        #endregion Video
        #region Buttons
        [Export] private Button _mainMenuButton = null;
        [Export] private Button _cancelButton = null;
        [Export] private Button _saveButton = null;
        [Export] private Button _okButton = null;
        #endregion Buttons
        #region SettingsData
        private SettingsData _data = null;
        #endregion SettingsData

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

            if (_fullscreenButton != null)
            {
                _fullscreenButton.Toggled += OnFullscreenToggled;
            }
            if (_resolutionsButton != null)
            {
                _resolutionsButton.ItemSelected += OnResolutionSelected;
            }

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

            if (_fullscreenButton != null)
            {
                _fullscreenButton.Toggled -= OnFullscreenToggled;
            }
            if (_resolutionsButton != null)
            {
                _resolutionsButton.ItemSelected -= OnResolutionSelected;
            }

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
            _data = new SettingsData(GameManager.Instance.Settings.Data);

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
        
            InitializeResolutions(_data);
        }

        private void InitializeResolutions(SettingsData data)
        {
            Vector2I viewPortSize = GameManager.MinWindowSize;
            Vector2I screenSize = DisplayServer.ScreenGetSize();

            Resolution savedResolution = new Resolution()
            {
                Width = data.Resolution.X,
                Height = data.Resolution.Y,
            };

            int multiplyer = 1;
            int id = 0;
            int selected = 0;

            while (GetWidth(viewPortSize, multiplyer) <= screenSize.X
                && GetHeight(viewPortSize, multiplyer) <= screenSize.Y)
            {
                Resolution resolution = new Resolution()
                {
                    Width = GetWidth(viewPortSize, multiplyer),
                    Height = GetHeight(viewPortSize, multiplyer),
                    Multiplyer = multiplyer,
                };

                if (resolution == savedResolution)
                {
                    selected = id;
                }

                _resolutions.Add(id, resolution);

                _resolutionsButton.AddItem(resolution.ToString(), id);
                multiplyer++;
                id++;
            }

            if (_resolutionsButton.GetItemCount() > 0)
                _resolutionsButton.Select(selected);
            else
                GD.PushWarning("No resolutions were added to the resolution button. " +
                "This may be due to the editor.");

            _fullscreenButton.ButtonPressed = data.Fullscreen;

            // Methods in methods bcs they are not neaded after Init, but a little unconventional
            static int GetWidth(Vector2I viewPortSize, int multiplier)
            {
                return viewPortSize.X * multiplier;
            }
            static int GetHeight(Vector2I viewPortSize, int multiplier)
            {
                return viewPortSize.Y * multiplier;
            }
        }

        #region Audio
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

            switch (audioBus)
            {
                case "Master":
                    _data.MasterVolume = decibel;
                    break;
                case "Music":
                    _data.MusicVolume = decibel;
                    break;
                case "SFX":
                    _data.SfxVolume = decibel;
                    break;
            }

            Settings.ApplySettings(_data);
        }
        #endregion Audio
        #region Video
        private void OnResolutionSelected(long index)
        {
            if (_resolutions.TryGetValue((int)index, out Resolution resolution))
            {
                _data.Resolution = (Vector2I) resolution;
                Settings.ApplySettings(_data);
               // GD.Print($"{GetWindow().Size}, {DisplayServer.WindowGetSize()}, {DisplayServer.WindowGetPosition()}");
            }
            else
            {
                GD.PushError($"Resolution index {index} not found in resolutions dictionary.");
            }
        }

        private void OnFullscreenToggled(bool fullscreen)
        {
            _data.Fullscreen = fullscreen;
            _resolutionsButton.Disabled = fullscreen;
            Settings.ApplySettings(_data);
        }
        #endregion Video
        #region Buttons
        public void MainMenu()
        {
            if (!GameManager.Instance.ChangeState(GameStateType.MainMenu))
            {
                GD.PushError("Failed to change state to MainMenu.");
            }
        }
        public void Close()
        {
            Settings.ApplySettings(GameManager.Instance.Settings.Data);
            GameManager.Instance.ActivatePreviousState();
        }
        
        public void Save()
        {
            GameManager.Instance.Settings.Data = _data;
            GameManager.Instance.Settings.Save();
        }
        #endregion Buttons
    }
}