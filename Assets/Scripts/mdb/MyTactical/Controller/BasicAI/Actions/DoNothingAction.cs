namespace mdb.MyTactial.Controller.BasicAI.Actions
{
    public class DoNothingAction : Action
    {
        public override bool DoAction(object[] arguments)
        {
            BattleStateMachine.instance.OnClick(Unit.Cell);
            BattleStateMachine.instance.ActionsMenu.OnEnter += OnActionsMenu;

            return true;
        }

        private void OnActionsMenu()
        {
            BattleStateMachine.instance.AddTransition(BattleStateMachine.instance.NO_ACTION);
            BattleStateMachine.instance.ActionsMenu.OnEnter -= OnActionsMenu;
        }
    }
}