using Godot;

namespace GA.GameProgramming
{
    public class Settings
    {
        public const string SettingsFilePath = "user://settings.cfg";
        private SettingsData _data = null;

        public SettingsData Data
        {
            get { return _data; }
            set
            {
                _data = value;
                ApplySettings(_data);
            }
        }

        public Settings()
        {
            Data = Load();
        }

        public Settings(SettingsData data)
        {
            Data = data;
        }

        public static async void ApplySettings(SettingsData data)
        {
            AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), data.MasterVolume);
            AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Music"), data.MusicVolume);
            AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("SFX"), data.SfxVolume);

            // From Sami
            // DisplayServer.WindowSetSize(data.Resolution);

            // DisplayServer.WindowMode windowMode = data.Fullscreen
            //     ? DisplayServer.WindowMode.Fullscreen
            //     : DisplayServer.WindowMode.Windowed;
            // DisplayServer.WindowSetMode(windowMode);

            SceneTree tree = GameManager.Instance.GetTree();
            Window window = tree.Root.GetWindow();

            if (data.Fullscreen)
            {
                // int screen = DisplayServer.WindowGetCurrentScreen();
                // if (DisplayServer.WindowGetSize() == DisplayServer.ScreenGetSize(screen))
                // {
                //     // window.Size = window.ContentScaleSize;
                //     // await tree.ToSignal(tree, SceneTree.SignalName.ProcessFrame);
                //     // await tree.ToSignal(tree, SceneTree.SignalName.ProcessFrame);
                // }
                window.Mode = Window.ModeEnum.Fullscreen;
            }
            else
            {
                window.Mode = Window.ModeEnum.Windowed;
                await tree.ToSignal(tree, SceneTree.SignalName.ProcessFrame);
                await tree.ToSignal(tree, SceneTree.SignalName.ProcessFrame);
                window.Size = data.Resolution;
                window.MoveToCenter();
            }
        }

        public void Save()
        {
            ConfigFile settingsFile = new ConfigFile();
            settingsFile.SetValue("Audio", "MasterVolume", Data.MasterVolume);
            settingsFile.SetValue("Audio", "MusicVolume", Data.MusicVolume);
            settingsFile.SetValue("Audio", "SfxVolume", Data.SfxVolume);

            settingsFile.SetValue("Video", "Resolution", Data.Resolution);
            settingsFile.SetValue("Video", "Fullscreen", Data.Fullscreen);

            settingsFile.Save(SettingsFilePath);
        }

        //TODO: Return loaded data.
        public SettingsData Load()
        {
            ConfigFile savedSettings = new ConfigFile();
            Error readError = savedSettings.Load(SettingsFilePath);
            SettingsData defaultData = SettingsData.Default;

            if (readError != Error.Ok)
            {
                return defaultData;
            }

            return new SettingsData()
            {
                MasterVolume = (float)savedSettings.GetValue("Audio", "MasterVolume", defaultData.MasterVolume),
                MusicVolume = (float)savedSettings.GetValue("Audio", "MusicVolume", defaultData.MusicVolume),
                SfxVolume = (float)savedSettings.GetValue("Audio", "SfxVolume", defaultData.SfxVolume),

                Resolution = (Vector2I)savedSettings.GetValue("Video", "Resolution", defaultData.Resolution),
                Fullscreen = (bool)savedSettings.GetValue("Video", "Fullscreen", defaultData.Fullscreen)
            };
        }
    }
}