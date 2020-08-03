using UnityEngine;
using System.Collections.Generic;

using mdb.MyTactial.Model;

namespace mdb.MyTactial.Controller
{
    [RequireComponent(typeof(BattleStateMachine))]
    public class BattleController : MonoBehaviour
    {
        public static BattleController instance;

        public Battle Battle { get { return _battle; } set { _battle = value; } }

		[SerializeField]
        private Battle _battle;
        private Queue<Unit> _turnUnitsOrder;
        private Unit _currentUnit;
        private Cell[] _currentUnitReachableCells;
        private Unit[] _currentTargetUnits;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            AddStateMachineListeners();

            BattleStateMachine.instance.AddTransition(BattleStateMachine.instance.START_BATTLE);
        }

        private void OnDestroy()
        {
            instance = null;
        }

        private void AddStateMachineListeners()
        {
            BattleStateMachine.instance.StartBattle.OnEnter += OnStartBattle;
            BattleStateMachine.instance.BuildMap.OnEnter += OnBuildMap;
            BattleStateMachine.instance.PlaceUnits.OnEnter += OnPlaceUnits;
            BattleStateMachine.instance.StartTurn.OnEnter += OnStartTurn;
            BattleStateMachine.instance.StartUnitTurn.OnEnter += OnStartUnitTurn;
            BattleStateMachine.instance.ActionsMenu.OnEnter += OnActionsMenu;
            BattleStateMachine.instance.EndUnitTurn.OnEnter += OnEndUnitTurn;
            BattleStateMachine.instance.EndTurn.OnEnter += OnEndTurn;

            BattleStateMachine.instance.MoveUnit.OnClickEvent += OnMoveUnitClick;
        }

        private void OnStartBattle()
        {
            BattleStateMachine.instance.AddTransition(BattleStateMachine.instance.BUILD_MAP);
        }

        private void OnBuildMap()
        {
            _battle.BuildAdjacents();
            BattleStateMachine.instance.AddTransition(BattleStateMachine.instance.PLACE_UNITS);
        }

        private void OnPlaceUnits()
        {
            _battle.SetUnits();
            _battle.PlaceUnits();
            BattleStateMachine.instance.AddTransition(BattleStateMachine.instance.START_FIRST_TURN);
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

            BattleStateMachine.instance.AddTransition(BattleStateMachine.instance.START_FIRST_UNIT_TURN);
        }

        private void OnStartUnitTurn()
        {
            _currentUnit = _turnUnitsOrder.Dequeue();
            _currentUnit.SetActive(true);

            GetUnitPossibleReachableCells();

            BattleStateMachine.instance.AddTransition(BattleStateMachine.instance.MOVE_UNIT);
        }

        private void OnMoveUnitClick(object obj)
        {
            if (obj is Cell cell)
            {
                if (cell == _currentUnit.Cell)
                {
                    BattleStateMachine.instance.AddTransition(BattleStateMachine.instance.ACTIONS_MENU);
                }

                if (cell.IsActive() && cell.UnitEnter(_currentUnit))
                {
                    BattleStateMachine.instance.AddTransition(BattleStateMachine.instance.ACTIONS_MENU);
                }
            }
        }

        private void OnActionsMenu()
        {
            foreach (Cell cell in _currentUnitReachableCells)
            {
                cell.SetActive(false);
            }

            GetUnitPossibleTargets();
        }

        private void OnEndUnitTurn()
        {
            _currentUnit.SetActive(false);

            foreach (Unit unit in _currentTargetUnits)
            {
                unit.SetActive(false);
            }

            if (_turnUnitsOrder.Count > 0)
            {
                BattleStateMachine.instance.AddTransition(BattleStateMachine.instance.START_NEXT_UNIT_TURN);
            }
            else
            {
                BattleStateMachine.instance.AddTransition(BattleStateMachine.instance.END_TURN);
            }
        }

        private void OnEndTurn()
        {
            BattleStateMachine.instance.AddTransition(BattleStateMachine.instance.START_NEW_TURN);
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

        private void GetUnitPossibleTargets()
        {
            List<Unit> targets = new List<Unit>();
            for (int cellIndex = 0; cellIndex < _currentUnit.Cell.AdjacentCells.Length; cellIndex++)
            {
                Cell cell = _currentUnit.Cell.AdjacentCells[cellIndex];

                if(cell.Unit != null && cell.Unit.Team != _currentUnit.Team)
                {
                    targets.Add(cell.Unit);
                    cell.Unit.SetActive(true);
                }
            }
            _currentTargetUnits = targets.ToArray();
        }
    }
}