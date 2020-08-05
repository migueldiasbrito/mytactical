using System.Collections.Generic;
using UnityEngine;

using mdb.MyTactial.Model;

namespace mdb.MyTactial.Controller.BasicAI.Conditions
{
    public class HasCellBeenReached : Condition
    {
        [SerializeField]
        private int[] _cellIndex = new int[0];

        private List<Cell> _cells;

        private void Start()
        {
            _cells = new List<Cell>();

            foreach (int cellIndex in _cellIndex)
            {
                _cells.Add(BattleController.instance.Battle.Cells[cellIndex]);
            }

            BattleStateMachine.instance.MoveUnit.OnExit += OnMoveUnitExit;
        }

        private void OnMoveUnitExit()
        {
            if (_cells.Contains(BattleController.instance.CurrentUnit.Cell))
            {
                TestCondition = true;
                BattleStateMachine.instance.MoveUnit.OnExit -= OnMoveUnitExit;
            }
        }
    }
}