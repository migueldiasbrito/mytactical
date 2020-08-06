namespace mdb.MyTactial.Controller.BasicAI.Actions
{
    public class DoNothingAction : Action
    {
        public override bool DoAction(object[] arguments)
        {
            BattleStateMachine.instance.OnClick(Unit.Cell);
            BattleStateMachine.instance.SelectAction.OnEnter += OnSelectAction;

            return true;
        }

        private void OnSelectAction()
        {
            BattleStateMachine.instance.AddTransition(BattleStateMachine.instance.NO_ACTION);
            BattleStateMachine.instance.SelectAction.OnEnter -= OnSelectAction;
        }
    }
}