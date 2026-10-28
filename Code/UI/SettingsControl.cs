using Godot;
namespace GA.GameProgramming
{
    public partial class SettingsControl : Control
    {
        [Export] private Button _closeButton = null;

        public override void _EnterTree()
        {
            ProcessMode = ProcessModeEnum.Always;
            _closeButton.Pressed += OnClosePressed;
        }
        public override void _ExitTree()
        {
            _closeButton.Pressed -= OnClosePressed;
        }

        private void OnClosePressed()
        {
            if (!GameManager.Instance.ActivatePreviousState())
            {
                GD.PushError("Failed to activate previous state.");
            }
        }
        
    }
}