using Godot;
using System;

namespace GA.GameProgramming.UI
{
    public partial class LevelUi : CanvasLayer
    {
        [Export] private Label _scoreLabel = null;
        [Export] private Label _livesLabel = null;

        public override void _EnterTree()
        {
            GameManager.Instance.ScoreChanged += OnScoreChanged;
            GameManager.Instance.LivesChanged += OnLivesChanged;
        }

        public override void _ExitTree()
        {
            GameManager.Instance.ScoreChanged -= OnScoreChanged;
            GameManager.Instance.LivesChanged -= OnLivesChanged;
        }

        private void OnScoreChanged(int newScore)
        {
            if (_scoreLabel != null)
            {
                _scoreLabel.Text = $"Score: {newScore}";
            }
        }

        private void OnLivesChanged(int newLives)
        {
            if (_livesLabel != null)
            {
                _livesLabel.Text = $"Lives: {newLives}";
            }
        }
    }
}