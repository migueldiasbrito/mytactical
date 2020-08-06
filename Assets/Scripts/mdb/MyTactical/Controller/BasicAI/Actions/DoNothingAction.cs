using System.Collections;
using UnityEngine;

namespace mdb.MyTactial.Controller.BasicAI.Actions
{
    public class DoNothingAction : Action
    {
        public override bool DoAction(object[] arguments)
        {
            StartCoroutine(SelectCell());
            return true;
        }

        private IEnumerator SelectCell()
        {
            yield return new WaitForSeconds(1);

            BattleStateMachine.instance.OnClick(Unit.Cell);
            BattleStateMachine.instance.SelectAction.OnEnter += OnSelectAction;
        }

        private void OnSelectAction()
        {
            BattleStateMachine.instance.SelectAction.OnEnter -= OnSelectAction;
            StartCoroutine(SelectAction());
        }

        private IEnumerator SelectAction()
        {
            yield return new WaitForSeconds(1);

            BattleStateMachine.instance.AddTransition(BattleStateMachine.instance.NO_ACTION);
        }
    }
}