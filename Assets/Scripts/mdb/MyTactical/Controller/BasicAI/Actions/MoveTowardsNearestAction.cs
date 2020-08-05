using System.Collections.Generic;

using mdb.MyTactial.Model;

namespace mdb.MyTactial.Controller.BasicAI.Actions
{
    public class MoveTowardsNearestAction : Action
    {
        private struct CellPathHelper
        {
            public Cell Cell;
            public Cell LastReachableCell;
        }

        public override bool DoAction(object[] arguments)
        {
            Queue<CellPathHelper> pathHelper = new Queue<CellPathHelper>();
            HashSet<Cell> visitedCell = new HashSet<Cell>();
            HashSet<Cell> noEmenyCells = new HashSet<Cell>();
            List<Cell> currentReachableCells = new List<Cell>(BattleController.instance.CurrentUnitReachableCells);

            pathHelper.Enqueue(new CellPathHelper { Cell = Unit.Cell, LastReachableCell = Unit.Cell });

            do
            {
                CellPathHelper currentCell = pathHelper.Dequeue();


                if (currentReachableCells.Contains(currentCell.Cell))
                {
                    currentCell.LastReachableCell = currentCell.Cell;
                }
                
                foreach (Cell ajdacentCell in currentCell.Cell.AdjacentCells)
                {
                    if (noEmenyCells.Add(ajdacentCell))
                    {
                        if(ajdacentCell.Unit != null && ajdacentCell.Unit.Team != Unit.Team)
                        {
                            BattleStateMachine.instance.OnClick(currentCell.LastReachableCell);
                            BattleStateMachine.instance.ActionsMenu.OnEnter += OnActionsMenu;
                            return true;
                        }
                    }

                    if(visitedCell.Add(ajdacentCell))
                    {
                        pathHelper.Enqueue(new CellPathHelper { Cell = ajdacentCell, LastReachableCell = currentCell.LastReachableCell });
                    }
                }

            } while (pathHelper.Count > 0);

            return false;
        }

        private void OnActionsMenu()
        {
            BattleStateMachine.instance.AddTransition(BattleStateMachine.instance.NO_ACTION);
            BattleStateMachine.instance.ActionsMenu.OnEnter -= OnActionsMenu;
        }
    }
}