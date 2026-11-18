using Godot;

namespace GA.GameProgramming
{
    [GlobalClass]
    public partial class MusicMap : Resource
    {
        [Export] private MusicEntry[] _entries = null;

        public AudioStream GetStream(MusicType musicType)
        {
            if (_entries == null) return null;

            foreach (MusicEntry entry in _entries)
            {
                if (entry.MusicType == musicType)
                {
                    return entry.Stream;
                }
            }
            return null;
        }
    }
}