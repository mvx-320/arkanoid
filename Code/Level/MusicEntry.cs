using Godot;

namespace GA.GameProgramming
{
    [GlobalClass]
    public partial class MusicEntry : Resource
    {
        [Export] public MusicType MusicType { get; set; }
        [Export] public AudioStream Stream { get; set; }
    }
}