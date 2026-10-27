using Godot;

namespace GA.GameProgramming.State
{
    public class MainMenuState : GameStateBase
    {
        public override GameStateType StateType => GameStateType.MainMenu;
        public override string ScenePath => "res://Scenes/Menu/MainMenu.tscn";

        public MainMenuState()
        {
            AddTargetState(GameStateType.InGame);
            AddTargetState(GameStateType.Settings);
        }
    }
}