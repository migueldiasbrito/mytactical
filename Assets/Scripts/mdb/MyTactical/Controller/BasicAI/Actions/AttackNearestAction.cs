using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using mdb.MyTactial.Model;

namespace mdb.MyTactial.Controller.BasicAI.Actions
{
    public class AttackNearestAction : Action
    {
        private Unit _target = null;

        public override bool DoAction(object[] arguments)
        {
            if (arguments != null)
            {
                Cell cell = null;
                for (int argumentIndex = 0; argumentIndex < arguments.Length; argumentIndex++)
                {
                    if (arguments[argumentIndex] is Cell)
                    {
                        cell = (Cell)arguments[argumentIndex];
                    }
                    else if (arguments[argumentIndex] is Unit)
                    {
                        _target = (Unit)arguments[argumentIndex];
                    }

                    if (cell != null && _target != null)
                    {
                        StartCoroutine(AttackNearest(cell));
                        return true;
                    }
                }
            }

            HashSet<Cell> visitedCells = new HashSet<Cell>();

            foreach (Cell reachableCell in BattleController.instance.CurrentUnitReachableCells)
            {
                foreach (Cell adjacentCell in reachableCell.AdjacentCells)
                {
                    if (visitedCells.Add(adjacentCell))
                    {
                        if (adjacentCell.Unit != null && adjacentCell.Unit.Team != Unit.Team)
                        {
                            _target = adjacentCell.Unit;
                            AttackNearest(reachableCell);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private IEnumerator AttackNearest(Cell cell)
        {
            yield return new WaitForSeconds(1);

            BattleStateMachine.instance.OnClick(cell);

            BattleStateMachine.instance.SelectAction.OnEnter += OnSelectAction;
            BattleStateMachine.instance.SelectTarget.OnEnter += OnSelectTarget;
        }

        private void OnSelectAction()
        {
            BattleStateMachine.instance.AddTransition(BattleStateMachine.instance.SELECT_TARGET);
            BattleStateMachine.instance.SelectAction.OnEnter -= OnSelectAction;
        }

        private void OnSelectTarget()
        {
            BattleStateMachine.instance.SelectTarget.OnEnter -= OnSelectTarget;
            StartCoroutine(SelectTarget());
        }

        private IEnumerator SelectTarget()
        {
            yield return new WaitForSeconds(1);

            BattleStateMachine.instance.OnClick(_target);
            _target = null;
        }
    }
}