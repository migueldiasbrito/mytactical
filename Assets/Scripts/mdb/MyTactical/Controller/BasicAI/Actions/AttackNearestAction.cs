using System.Collections.Generic;

using mdb.MyTactial.Model;

namespace mdb.MyTactial.Controller.BasicAI.Actions
{
    public class AttackNearestAction : Action
    {
        private Unit _target = null;

        public override bool DoAction(object[] arguments)
        {
            Cell cell = null;

            if (arguments != null)
            {
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
                        AttackNearest(cell);
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
                            AttackNearest(cell);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void AttackNearest(Cell cell)
        {
            BattleStateMachine.instance.OnClick(cell);

            BattleStateMachine.instance.ActionsMenu.OnEnter += OnActionsMenu;
            BattleStateMachine.instance.SelectTarget.OnEnter += OnSelectTarget;
        }

        private void OnActionsMenu()
        {
            BattleStateMachine.instance.AddTransition(BattleStateMachine.instance.SELECT_TARGET);
            BattleStateMachine.instance.ActionsMenu.OnEnter -= OnActionsMenu;
        }

        private void OnSelectTarget()
        {
            BattleStateMachine.instance.OnClick(_target);
            BattleStateMachine.instance.SelectTarget.OnEnter -= OnSelectTarget;

            _target = null;
        }
    }
}