using Godot;

namespace GA.GameProgramming
{
	[GlobalClass]
	public partial class EffectMap : Resource
	{
		[Export] private EffectSceneEntry[] _effectSceneEntries = null;

		public PackedScene GetScene(EffectType effectType)
		{
			foreach (EffectSceneEntry effectSceneEntry in _effectSceneEntries)
			{
				if (effectSceneEntry.EffectType == effectType)
				{
					return effectSceneEntry.EffectScene;
				}
			}
			return null;
		}
	}
}