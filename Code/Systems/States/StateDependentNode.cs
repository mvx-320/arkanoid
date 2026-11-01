using Godot;

namespace GA.GameProgramming.UI
{
    public partial class StateDependentNode : Node
    {
        [Export] private GameStateType _enabledInState = GameStateType.Pause;

        public override void _Ready()
        {
            bool active = GameManager.Instance.ActiveState?.StateType == _enabledInState;
            SetParentActive(active);
        }

        private void SetParentActive(bool active)
        {
            Node parent = GetParent();

            if (parent is CanvasItem item)
            {
                item.Visible = active;
            }

            if (parent is BaseButton button)
            {
                button.Disabled = !active;
            }
        }
    }
}