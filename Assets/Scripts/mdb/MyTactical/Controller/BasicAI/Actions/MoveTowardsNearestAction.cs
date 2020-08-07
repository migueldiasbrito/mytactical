using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using mdb.MyTactial.Model;
using mdb.MyTactial.View;

namespace mdb.MyTactial.Controller.BasicAI.Actions
{
    public class MoveTowardsNearestAction : Action
    {
        public UnitView unitView;

        public override bool DoAction(object[] arguments)
        {
            Queue<CellPathHelper> pathHelper = new Queue<CellPathHelper>();
            HashSet<Cell> visitedCell = new HashSet<Cell>();
            HashSet<Cell> noEmenyCells = new HashSet<Cell>();
            List<Cell> currentReachableCells = new List<Cell>(BattleController.instance.CurrentUnitReachableCells);

            pathHelper.Enqueue(new CellPathHelper { Cell = Unit.Cell, Path = new Cell[1] { Unit.Cell } });

            do
            {
                CellPathHelper currentCell = pathHelper.Dequeue();

                if (currentReachableCells.Contains(currentCell.Cell))
                {
                    currentCell.Path = new List<Cell>(currentCell.Path) { currentCell.Cell }.ToArray();
                }
                
                foreach (Cell ajdacentCell in currentCell.Cell.AdjacentCells)
                {
                    if ((currentCell.Cell.Unit == null || currentCell.Cell.Unit == Unit) && noEmenyCells.Add(ajdacentCell))
                    {
                        if(ajdacentCell.Unit != null && ajdacentCell.Unit.Team != Unit.Team)
                        {
                            StartCoroutine(SelectCell(currentCell.Path));
                            return true;
                        }
                    }

                    if(visitedCell.Add(ajdacentCell))
                    {
                        pathHelper.Enqueue(new CellPathHelper { Cell = ajdacentCell, Path = currentCell.Path });
                    }
                }

            } while (pathHelper.Count > 0);

            return false;
        }

        private IEnumerator SelectCell(Cell[] path)
        {
            yield return new WaitForSeconds(1);

            unitView.DoPath(path);

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