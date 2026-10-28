using Godot;

namespace GA.GameProgramming
{
    public partial class GameOverControl : Control
    {
        [Export] private Button _retryButton = null;
        [Export] private Button _mainMenuButton = null;
        [Export] private Button _quitButton = null;

        public override void _EnterTree()
        {
            _retryButton.Pressed += OnRetryPressed;
            _mainMenuButton.Pressed += OnMainMenuPressed;
            _quitButton.Pressed += OnQuitPressed;
        }

        public override void _ExitTree()
        {
            _retryButton.Pressed -= OnRetryPressed;
            _mainMenuButton.Pressed -= OnMainMenuPressed;
            _quitButton.Pressed -= OnQuitPressed;
        }

        private void OnRetryPressed()
        {
            if (!GameManager.Instance.StartNewGame())
            {
                GD.PushError("Failed to start a new game.");
            }
        } 

        private void OnMainMenuPressed()
        {
            if (!GameManager.Instance.ChangeState(GameStateType.MainMenu))
            {
                GD.PushError("Failed to change state to MainMenu.");
            }
        }

        private void OnQuitPressed() { GetTree().Quit(); }
    }
}