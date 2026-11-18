using Godot;

namespace GA.GameProgramming.State
{
    public class MainMenuState : GameStateBase
    {
        public override GameStateType StateType => GameStateType.MainMenu;
        public override string ScenePath => "res://Scenes/Menus/MainMenu.tscn";
        public override MusicType Music => MusicType.MainMenu;

        public MainMenuState()
        {
            AddTargetState(GameStateType.InGame);
            AddTargetState(GameStateType.Settings);
        }
    }
}