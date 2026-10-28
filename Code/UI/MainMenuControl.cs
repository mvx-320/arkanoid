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
        }

        public override void _ExitTree()
        {
            _newGameButton.Pressed -= OnNewGamePressed;
        }

        private void OnNewGamePressed()
        {
            if (!GameManager.Instance.ChangeState(GameStateType.InGame))
            {
                GD.PushError("Failed to change state to InGame.");
            }   
        }
    }
}