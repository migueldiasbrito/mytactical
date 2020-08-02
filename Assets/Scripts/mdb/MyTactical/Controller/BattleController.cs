using UnityEngine;
using System.Collections.Generic;

using mdb.MyTactial.Model;

namespace mdb.MyTactial.Controller
{
    public class BattleController : MonoBehaviour
    {
        public static BattleController instance;

        public Battle Battle { get { return _battle; } set { _battle = value; } }

        [SerializeField]
        private Battle _battle;

        private BattleStateMachine _stateMachine;

        private Queue<Unit> _turnUnitsOrder;
        private Unit _currentUnit;
        private Cell[] _currentUnitReachableCells;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            BuildStateMachine();
        }

        private void OnDestroy()
        {
            instance = null;
        }

        private void BuildStateMachine()
        {
            _stateMachine = new BattleStateMachine();
            _stateMachine.StartBattle.OnEnter += OnStartBattle;
            _stateMachine.BuildMap.OnEnter += OnBuildMap;
            _stateMachine.PlaceUnits.OnEnter += OnPlaceUnits;
            _stateMachine.StartTurn.OnEnter += OnStartTurn;
            _stateMachine.StartUnitTurn.OnEnter += OnStartUnitTurn;

            _stateMachine.MakeTransition(_stateMachine.START_BATTLE);
        }

        private void OnStartBattle()
        {
            _stateMachine.MakeTransition(_stateMachine.BUILD_MAP);
        }

        private void OnBuildMap()
        {
            _battle.BuildAdjacents();
            _stateMachine.MakeTransition(_stateMachine.PLACE_UNITS);
        }

        private void OnPlaceUnits()
        {
            _battle.SetUnits();
            _battle.PlaceUnits();
            _stateMachine.MakeTransition(_stateMachine.START_FIRST_TURN);
        }

        private void OnStartTurn()
        {
            List<Unit> units = new List<Unit>();

            for(int teamIndex = 0; teamIndex < _battle.Teams.Length; teamIndex++)
            {
                units.AddRange(_battle.Teams[teamIndex].Units);
            }

            units.Sort(TurnOrderSort);

            _turnUnitsOrder = new Queue<Unit>(units);

            _stateMachine.MakeTransition(_stateMachine.START_FIRST_UNIT_TURN);
        }

        private void OnStartUnitTurn()
        {
            _currentUnit = _turnUnitsOrder.Dequeue();
            _currentUnit.SetActive(true);

            GetUnitPossibleReachableCells();

            _stateMachine.MakeTransition(_stateMachine.MOVE_UNIT);
        }

        private int TurnOrderSort(Unit u1, Unit u2)
        {
            if (u1.Agility > u2.Agility)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

        private struct CellPathHelper
        {
            public Cell Cell;
            public float Distance;
        }

        private void GetUnitPossibleReachableCells()
        {
            List<Cell> cells = new List<Cell>();
            List<CellPathHelper> cellsHelper = new List<CellPathHelper>();

            cellsHelper.Add(new CellPathHelper { Cell = _currentUnit.Cell, Distance = 0 });
            cells.Add(_currentUnit.Cell);
            _currentUnit.Cell.SetActive(true);

            for (int cellsIndex = 0; cellsIndex < cellsHelper.Count; cellsIndex++)
            {
                CellPathHelper cellHelper = cellsHelper[cellsIndex];
                for (int adjacentIndex = 0; adjacentIndex < cellHelper.Cell.AdjacentCells.Length; adjacentIndex++)
                {
                    Cell cell = cellHelper.Cell.AdjacentCells[adjacentIndex];

                    if (cell == null) { continue; }

                    if (cell.Unit != null && cell.Unit.Team != _currentUnit.Team) { continue; }

                    if (cellHelper.Distance + 1 <= _currentUnit.Movement)
                    {
                        cellsHelper.Add(new CellPathHelper { Cell = cell, Distance = cellHelper.Distance + 1 });
                        cells.Add(cell);
                        cell.SetActive(true);
                    }
                }
            }

            _currentUnitReachableCells = cells.ToArray();
        }
    }
}