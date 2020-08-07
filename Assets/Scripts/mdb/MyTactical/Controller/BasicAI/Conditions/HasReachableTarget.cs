using System.Collections.Generic;

using mdb.MyTactial.Model;

namespace mdb.MyTactial.Controller.BasicAI.Conditions
{
    public class HasReachableTarget : Condition
    {
        private void Start()
        {
            BattleStateMachine.instance.StartUnitTurn.OnExit += OnStartUnitTurnExit;
        }

        private void OnStartUnitTurnExit()
        {
            if (BattleController.instance.CurrentUnit == Unit)
            {
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
                        if (noEmenyCells.Add(adjacentCell))
                        {
                            if (adjacentCell.Unit != null && adjacentCell.Unit.Team != Unit.Team)
                            {
                                TestCondition = true;
                                TestResults = new object[] { currentCell.Path, adjacentCell.Unit };
                                return;
                            }
                        }

                        if (visitedCells.Add(adjacentCell))
                        {
                            if (currentReachableCells.Contains(adjacentCell))
                            {
                                pathHelper.Enqueue(new CellPathHelper {
                                    Cell = adjacentCell,
                                    Path = new List<Cell>(currentCell.Path) { adjacentCell }.ToArray()
                                });
                            }
                        }
                    }
                } while (pathHelper.Count > 0);

                TestCondition = false;
                TestResults = null;
            }
        }
    }
}