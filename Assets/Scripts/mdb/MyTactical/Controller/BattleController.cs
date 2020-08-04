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

		public Unit CurrentUnit { get; private set; }
		public Unit CurrentTarget { get; private set; }

        [SerializeField]
        private Battle _battle;
        private Queue<Unit> _turnUnitsOrder;
        private Cell[] _currentUnitReachableCells;
        private Unit[] _currentTargetUnits = new Unit[0];

        public bool HasTargets()
        {
            return _currentTargetUnits.Length > 0;
        }

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
            BattleStateMachine.instance.SelectTarget.OnEnter += OnSelectTarget;
            BattleStateMachine.instance.EndUnitTurn.OnEnter += OnEndUnitTurn;
            BattleStateMachine.instance.EndTurn.OnEnter += OnEndTurn;

            BattleStateMachine.instance.MoveUnit.OnExit += OnMoveUnitExit;

            BattleStateMachine.instance.MoveUnit.OnClickEvent += OnMoveUnitClick;
            BattleStateMachine.instance.SelectTarget.OnClickEvent += OnSelectTargetClick;
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
                for (int unitIndex = 0; unitIndex < _battle.Teams[teamIndex].Units.Length; unitIndex++)
                {
                    Unit unit = _battle.Teams[teamIndex].Units[unitIndex];
                    if (unit.GetState() != Unit.State.Dead)
                    {
                        units.Add(unit);
                    }
                }
            }

            units.Sort(TurnOrderSort);

            _turnUnitsOrder = new Queue<Unit>(units);

            CurrentUnit = _turnUnitsOrder.Dequeue();

            BattleStateMachine.instance.AddTransition(BattleStateMachine.instance.START_FIRST_UNIT_TURN);
        }

        private void OnStartUnitTurn()
        {
            CurrentUnit.SetActive(Unit.State.Active);

            GetUnitPossibleReachableCells();

            BattleStateMachine.instance.AddTransition(BattleStateMachine.instance.MOVE_UNIT);
        }

        private void OnMoveUnitClick(object obj)
        {
            if (obj is Cell cell)
            {
                if (cell == CurrentUnit.Cell)
                {
                    BattleStateMachine.instance.AddTransition(BattleStateMachine.instance.ACTIONS_MENU);
                }

                if (cell.IsActive() && cell.UnitEnter(CurrentUnit))
                {
                    BattleStateMachine.instance.AddTransition(BattleStateMachine.instance.ACTIONS_MENU);
                }
            }
        }

        private void OnMoveUnitExit()
        {
            foreach (Cell cell in _currentUnitReachableCells)
            {
                cell.SetActive(false);
            }

            GetUnitPossibleTargets();
        }

        private void OnSelectTargetClick(object obj)
        {
            if (obj is Unit unit)
            {
                CurrentTarget = unit;
                foreach (Unit target in _currentTargetUnits)
                {
                    target.SetActive(Unit.State.Idle);
                }
                _currentTargetUnits = new Unit[0];

                unit.GetAttackedBy(CurrentUnit);
                BattleStateMachine.instance.AddTransition(BattleStateMachine.instance.ATTACK_TARGET);
            }
        }

        private void OnSelectTarget()
        {
            foreach (Unit unit in _currentTargetUnits)
            {
                unit.SetActive(Unit.State.Target);
            }
        }

        private void OnEndUnitTurn()
        {
            CurrentUnit.SetActive(Unit.State.Idle);
            CurrentTarget = null;

            do
            {
                if (_turnUnitsOrder.Count > 0)
                {
                    CurrentUnit = _turnUnitsOrder.Dequeue();
                }
                else
                {
                    BattleStateMachine.instance.AddTransition(BattleStateMachine.instance.END_TURN);
                    return;
                }
            } while (CurrentUnit.GetState() == Unit.State.Dead);

            BattleStateMachine.instance.AddTransition(BattleStateMachine.instance.START_NEXT_UNIT_TURN);
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

            cellsHelper.Add(new CellPathHelper { Cell = CurrentUnit.Cell, Distance = 0 });
            cells.Add(CurrentUnit.Cell);
            CurrentUnit.Cell.SetActive(true);

            for (int cellsIndex = 0; cellsIndex < cellsHelper.Count; cellsIndex++)
            {
                CellPathHelper cellHelper = cellsHelper[cellsIndex];
                for (int adjacentIndex = 0; adjacentIndex < cellHelper.Cell.AdjacentCells.Length; adjacentIndex++)
                {
                    Cell cell = cellHelper.Cell.AdjacentCells[adjacentIndex];

                    if (cell == null) { continue; }

                    if (cell.Unit != null && cell.Unit.Team != CurrentUnit.Team) { continue; }

                    if (cellHelper.Distance + 1 <= CurrentUnit.Movement)
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
            for (int cellIndex = 0; cellIndex < CurrentUnit.Cell.AdjacentCells.Length; cellIndex++)
            {
                Cell cell = CurrentUnit.Cell.AdjacentCells[cellIndex];

                if(cell.Unit != null && cell.Unit.Team != CurrentUnit.Team)
                {
                    targets.Add(cell.Unit);
                }
            }
            _currentTargetUnits = targets.ToArray();
        }
    }
}