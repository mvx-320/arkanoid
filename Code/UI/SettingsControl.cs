using Godot;
namespace GA.GameProgramming.UI
{
    public partial class SettingsControl : Control
    {
        // [Export] private Button _closeButton = null;
        [Export] private Button _saveButton = null;
        [Export] private Button _cancelButton = null;
        [Export] private Button _okButton = null;

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
        [Export] private OptionButton _resolutionOptionButton = null;
        [Export] private CheckBox _fullscreenCheckBox = null;
        #endregion Video

        public override void _EnterTree()
        {
            // ProcessMode = ProcessModeEnum.Always;
            // _closeButton.Pressed += OnClosePressed;
            _saveButton.Pressed += Save;
            _cancelButton.Pressed += Close;

            _okButton.Pressed += Save;
            _okButton.Pressed += Close;

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

            // _resolutionOptionButton.ItemSelected += OnResolutionSelected;
            // _fullscreenCheckBox.Toggled += OnFullscreenToggled;
        }
        public override void _ExitTree()
        {
            // _closeButton.Pressed -= OnClosePressed;
            _saveButton.Pressed -= Save;
            _cancelButton.Pressed -= Close;

            _okButton.Pressed -= Save;
            _okButton.Pressed -= Close;

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

        // private void OnResolutionSelected(int index)
        // {

        // }

        // private void OnFullscreenToggled(bool isChecked)
        // {
        //     // GetWindow().Mode = isChecked ? Window.ModeEnum.Fullscreen : Window.ModeEnum.Windowed;
        // }

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