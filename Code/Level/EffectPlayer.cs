using System;
using Godot;
using GA.Common;

namespace GA.GameProgramming
{
    [GlobalClass]
    public partial class EffectPlayer : Node2D
    {
        [Export] private EffectMap _effectMap = null;

        public void PlayEffect(EffectType type, Vector2 globalPosition)
        {
            if (_effectMap == null)
            {
                GD.PushError("EffectMap is not assigned!");
                return;
            }

            PackedScene effectScene = _effectMap.GetScene(type);
            if (effectScene == null)
            {
                GD.PushError($"Effect scene matching the effect type {type} is not found!");
                return;
            }

            GpuParticles2D effect = effectScene.Instantiate<GpuParticles2D>();
            AddChild(effect);
            effect.GlobalPosition = globalPosition;

            PlayEffect(effect);
        }

        private void PlayEffect(GpuParticles2D effect)
        {
            int effectsPlaying = 1;
            effect.OneShot = true;
            effect.Emitting = true;

            AudioStreamPlayer2D audio = effect.GetNode<AudioStreamPlayer2D>();
            if (audio != null)
            {
                effectsPlaying++;
                audio.Play();
                audio.Finished += () =>
                {
                    EffectFinished();
                };
            }

            effect.Finished += () =>
            {
                // Get's called when the signal is emitted.
                EffectFinished();
            };

            // Intentially declared here! Don't move this!
            void EffectFinished()
            {
                effectsPlaying--;
                if (effectsPlaying <= 0)
                {
                    effect.QueueFree();
                }
            }
        }
    }
}