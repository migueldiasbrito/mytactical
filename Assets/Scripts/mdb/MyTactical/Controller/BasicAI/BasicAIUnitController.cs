using System;
using System.Collections.Generic;
using UnityEngine;

using mdb.MyTactial.Controller.BasicAI.Conditions;
using mdb.MyTactial.Model;

namespace mdb.MyTactial.Controller.BasicAI
{
    public class BasicAIUnitController : MonoBehaviour
    {
        [Serializable]
        public struct Reaction
        {
            public Condition[] Conditions;
            public Actions.Action Action;

            public Reaction(Condition[] condition, Actions.Action action)
            {
                Conditions = condition;
                Action = action;
            }

            public bool TestConditions()
            {
                if (Conditions == null)
                {
                    return true;
                }

                foreach (Condition condition in Conditions)
                {
                    if(!condition.TestCondition)
                    {
                        return false;
                    }
                }

                return true;
            }

            public object[] GetConditionsResults()
            {
                if (Conditions == null)
                {
                    return new object[0];
                }

                List<object> results = new List<object>();
                
                foreach (Condition condition in Conditions)
                {
                    if (condition.TestResults != null)
                    {
                        results.AddRange(condition.TestResults);
                    }
                }

                return results.ToArray();
            }
        }

        public int TeamIndex;
        public int UnitIndex;

        private Unit _unit;

        public Reaction[] Reactions = new Reaction[0];

        private void Start()
        {
            _unit = BattleController.instance.Battle.Teams[TeamIndex].Units[UnitIndex];
            BattleStateMachine.instance.MoveUnit.OnEnter += OnMoveUnit;

            foreach (Reaction reaction in Reactions)
            {
                if (reaction.Conditions != null)
                {
                    foreach (Condition condition in reaction.Conditions)
                    {
                        condition.Unit = _unit;
                    }
                }
                reaction.Action.Unit = _unit;
            }
        }

        private void OnMoveUnit()
        {
            if (BattleController.instance.CurrentUnit == _unit)
            {
                foreach (Reaction reactions in Reactions)
                {
                    if (reactions.Conditions == null)
                    {
                        if (reactions.Action.DoAction(null))
                        {
                            return;
                        }
                    }
                    else if (reactions.TestConditions())
                    {
                        if (reactions.Action.DoAction(reactions.GetConditionsResults()))
                        {
                            return;
                        }
                    }
                }
            }
        }
    }
}
