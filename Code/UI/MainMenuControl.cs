using Godot;
using System;

namespace GA.GameProgramming
{
    public partial class MainMenuControl : Control
    {
        [Export] private Button _newGameButton = null;
        [Export] private Button _settingsGameButton = null;
        [Export] private Button _quitGameButton = null;

        public override void _EnterTree()
        {
            // TODO: Subscribe to the rest of the vuttons Pressed signals as well.
            _newGameButton.Pressed += OnNewGamePressed;
            _settingsGameButton.Pressed += OnSettingsPressed;
            _quitGameButton.Pressed += OnQuitPressed;
        }

        public override void _ExitTree()
        {
            _newGameButton.Pressed -= OnNewGamePressed;
            _settingsGameButton.Pressed -= OnSettingsPressed;
            _quitGameButton.Pressed -= OnQuitPressed;
        }

        private void OnNewGamePressed()
        {
            if (!GameManager.Instance.ChangeState(GameStateType.InGame))
            {
                GD.PushError("Failed to change state to InGame.");
            }   
        }

        private void OnSettingsPressed()
        {
            if (!GameManager.Instance.ChangeState(GameStateType.Settings))
            {
                GD.PushError("Failed to change state to Settings.");
            }
        }

        private void OnQuitPressed() { GetTree().Quit(); }
    }
}