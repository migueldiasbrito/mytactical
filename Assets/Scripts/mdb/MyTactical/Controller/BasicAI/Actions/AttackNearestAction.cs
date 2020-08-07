using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using mdb.MyTactial.Model;
using mdb.MyTactial.View;

namespace mdb.MyTactial.Controller.BasicAI.Actions
{
    public class AttackNearestAction : Action
    {
        [SerializeField]
        private int _maxPathLength = 1;

        public UnitView unitView;

        private Unit _target = null;

        public override bool DoAction(object[] arguments)
        {
            if (arguments != null)
            {
                Cell[] cells = null;
                for (int argumentIndex = 0; argumentIndex < arguments.Length; argumentIndex++)
                {
                    if (arguments[argumentIndex] is Cell[])
                    {
                        cells = (Cell[])arguments[argumentIndex];
                    }
                    else if (arguments[argumentIndex] is Unit)
                    {
                        _target = (Unit)arguments[argumentIndex];
                    }

                    if (cells != null && _target != null)
                    {
                        StartCoroutine(AttackNearest(cells));
                        return true;
                    }
                }
            }

            Queue<CellPathHelper> pathHelper = new Queue<CellPathHelper>();
            HashSet<Cell> visitedCells = new HashSet<Cell>();
            HashSet<Cell> noEmenyCells = new HashSet<Cell>();
            List<Cell> currentReachableCells = new List<Cell>(BattleController.instance.CurrentUnitReachableCells);

            pathHelper.Enqueue(new CellPathHelper { Cell = Unit.Cell, Path = new Cell[1] { Unit.Cell } });

            do
            {
                CellPathHelper currentCell = pathHelper.Dequeue();

                foreach (Cell adjacentCell in currentCell.Cell.AdjacentCells)
                {
                    if ((currentCell.Cell.Unit == null || currentCell.Cell.Unit == Unit) && noEmenyCells.Add(adjacentCell))
                    {
                        if (adjacentCell.Unit != null && adjacentCell.Unit.Team != Unit.Team)
                        {
                            _target = adjacentCell.Unit;
                            AttackNearest(currentCell.Path);
                            return true;
                        }
                    }

                    if (currentCell.Path.Length <= _maxPathLength + 1 && visitedCells.Add(adjacentCell))
                    {
                        if (currentReachableCells.Contains(adjacentCell))
                        {
                            pathHelper.Enqueue(new CellPathHelper
                            {
                                Cell = adjacentCell,
                                Path = new List<Cell>(currentCell.Path) { adjacentCell }.ToArray()
                            });
                        }
                    }
                }
            } while (pathHelper.Count > 0);

            return false;
        }

        private IEnumerator AttackNearest(Cell[] path)
        {
            yield return new WaitForSeconds(1);

            unitView.DoPath(path);

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
            _target.SetState(Unit.State.Target);
            StartCoroutine(SelectTarget());
        }

        private IEnumerator SelectTarget()
        {
            yield return new WaitForSeconds(1);

            _target.SetState(Unit.State.Idle);
            BattleStateMachine.instance.OnClick(_target);
            _target = null;
        }
    }
}