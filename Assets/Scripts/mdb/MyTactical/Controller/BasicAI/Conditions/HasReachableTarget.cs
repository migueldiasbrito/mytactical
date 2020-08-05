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
                HashSet<Cell> visitedCells = new HashSet<Cell>();

                foreach (Cell reachableCell in BattleController.instance.CurrentUnitReachableCells)
                {
                    foreach(Cell adjacentCell in reachableCell.AdjacentCells)
                    {
                        if (visitedCells.Add(adjacentCell))
                        {
                            if (adjacentCell.Unit != null && adjacentCell.Unit.Team != Unit.Team)
                            {
                                TestCondition = true;
                                TestResults = new object[] { reachableCell, adjacentCell.Unit };
                                return;
                            }
                        }
                    }
                }

                TestCondition = false;
                TestResults = null;
            }
        }
    }
}