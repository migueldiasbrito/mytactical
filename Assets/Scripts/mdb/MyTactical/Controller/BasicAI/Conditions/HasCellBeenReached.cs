using System.Collections.Generic;

using mdb.MyTactial.Model;

namespace mdb.MyTactial.Controller.BasicAI.Conditions
{
    public class HasCellBeenReached : Condition
    {
        public int[] CellIndex = new int[0];

        private List<Cell> _cells;

        private void Start()
        {
            _cells = new List<Cell>();

            foreach (int cellIndex in CellIndex)
            {
                _cells.Add(BattleController.instance.Battle.Cells[cellIndex]);
            }

            BattleStateMachine.instance.MoveUnit.OnExit += OnMoveUnitExit;
        }

        private void OnMoveUnitExit()
        {
            if (BattleController.instance.CurrentUnit.Team != Unit.Team &&
                _cells.Contains(BattleController.instance.CurrentUnit.Cell))
            {
                TestCondition = true;
                BattleStateMachine.instance.MoveUnit.OnExit -= OnMoveUnitExit;
            }
        }
    }
}