using System;
using UnityEngine;

using mdb.MyTactial.Controller.BasicAI.Conditions;
using mdb.MyTactial.Model;

namespace mdb.MyTactial.Controller.BasicAI
{
    public class BasicAIUnitController : MonoBehaviour
    {
        [Serializable]
        private struct Pair
        {
            public Condition Condition;
            public Actions.Action Action;

            public Pair(Condition condition, Actions.Action action)
            {
                Condition = condition;
                Action = action;
            }
        }

        public int TeamIndex;
        public int UnitIndex;

        private Unit _unit;

        [SerializeField]
        private Pair[] _behaviours = new Pair[0];

        private void Start()
        {
            _unit = BattleController.instance.Battle.Teams[TeamIndex].Units[UnitIndex];
            BattleStateMachine.instance.MoveUnit.OnEnter += OnMoveUnit;

            foreach (Pair pair in _behaviours)
            {
                if (pair.Condition != null)
                {
                    pair.Condition.Unit = _unit;
                }
                pair.Action.Unit = _unit;
            }
        }

        private void OnMoveUnit()
        {
            if (BattleController.instance.CurrentUnit == _unit)
            {
                foreach (Pair pair in _behaviours)
                {
                    if (pair.Condition == null)
                    {
                        if (pair.Action.DoAction(null))
                        {
                            return;
                        }
                    }
                    else if (pair.Condition.TestCondition)
                    {
                        if (pair.Action.DoAction(pair.Condition.TestResults))
                        {
                            return;
                        }
                    }
                }
            }
        }
    }
}
