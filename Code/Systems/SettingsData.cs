using Godot;

namespace GA.GameProgramming
{
    public class SettingsData
    {
        public static SettingsData Default
        {
            get
            {
                return new SettingsData()
                {
                    MasterVolume = -6.0f,
                    MusicVolume = -6.0f,
                    SfxVolume = -6.0f,

                    Resolution = new Vector2I(640, 360), //TODO: Replace this magic number.
                    Fullscreen = false
                };
            }
        }

        public float MasterVolume { get; set; }
        public float MusicVolume { get; set; }
        public float SfxVolume { get; set; }
        public Vector2I Resolution { get; set; }
        public bool Fullscreen { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public SettingsData()
        {
        }

        /// <summary>
        /// Copy-constructor.
        /// </summary>
        /// <param name="other">The object to be copied.</param>
        public SettingsData(SettingsData other)
        {
            MasterVolume = other.MasterVolume;
            MusicVolume = other.MusicVolume;
            SfxVolume = other.SfxVolume;
            Resolution = other.Resolution;
            Fullscreen = other.Fullscreen;
        }
    }
}