using System.Collections.Generic;
using UnityEngine;

using mdb.MyTactial.Controller;
using mdb.MyTactial.Model;

namespace mdb.MyTactial.View.TilemapView
{
    [RequireComponent(typeof(GridMovement))]
    public class TilemapUnitView : UnitView
    {
        public int TeamIndex;
        public int UnitIndex;

        private Unit _unit;
        private GridMovement _gridMovement;
        private Queue<Vector2> _path;
        private bool _moving = false;
        private Cell _lastCell;
        private bool _controlUnit = false;

        public override void DoPath(Cell[] cells)
        {
            _moving = true;
            _lastCell = cells.Length > 0 ? cells[cells.Length - 1] : _unit.Cell;

            foreach(Cell cell in cells)
            {
                Vector3Int target = TilemapBattleView.instance.CellPositions[cell];
                _path.Enqueue(new Vector2(target.x, target.y));
            }
        }
        public void DoPath(Vector3Int target, Cell cell)
        {
            _moving = true;
            _lastCell = cell;
            _path.Enqueue(new Vector2(target.x, target.y));
        }

        private void Start()
        {
            _unit = BattleController.instance.Battle.Teams[TeamIndex].Units[UnitIndex];

            _gridMovement = GetComponent<GridMovement>();
            _path = new Queue<Vector2>();

            BattleStateMachine.instance.PlaceUnits.OnExit += OnPlaceUnitExit;
        }

        private void Update()
        {
            if (_moving && _gridMovement.MoveTo == null)
            {
                if (_path.Count > 0)
                {
                    _gridMovement.MoveTo = _path.Dequeue();
                }
                else
                {
                    _moving = false;

                    if (!_controlUnit)
                    {
                        BattleStateMachine.instance.OnClick(_lastCell);
                        _lastCell = null;
                    }
                }
            }

            if (_controlUnit && !_moving)
            {
                Vector3Int position = new Vector3Int(
                    Mathf.RoundToInt(transform.position.x),
                    Mathf.RoundToInt(transform.position.y),
                    Mathf.RoundToInt(transform.position.z)
                );

                if (Input.GetAxisRaw("Fire1") == 1)
                {
                    if (_lastCell.Unit == null)
                    {
                        BattleStateMachine.instance.OnClick(TilemapBattleView.instance.ReachableCells[position]);
                        _controlUnit = false;
                    }
                    return;
                }

                float verticalInput = Input.GetAxisRaw("Vertical");

                if(Mathf.Abs(verticalInput) == 1)
                {
                    position += new Vector3Int(0, Mathf.RoundToInt(verticalInput), 0);
                    if(TilemapBattleView.instance.ReachableCells.ContainsKey(position))
                    {
                        DoPath(position, TilemapBattleView.instance.ReachableCells[position]);
                    }
                    return;
                }

                float horizontalInput = Input.GetAxisRaw("Horizontal");

                if (Mathf.Abs(horizontalInput) == 1)
                {
                    position += new Vector3Int(Mathf.RoundToInt(horizontalInput), 0, 0);
                    if (TilemapBattleView.instance.ReachableCells.ContainsKey(position))
                    {
                        DoPath(position, TilemapBattleView.instance.ReachableCells[position]);
                    }
                    return;
                }
            }
        }

        private void OnPlaceUnitExit()
        {
            if (!_unit.Team.IsAIControlled)
            {
                BattleStateMachine.instance.MoveUnit.OnEnter += OnMoveUnit;
            }
        }

        private void OnMoveUnit()
        {
            if (BattleController.instance.CurrentUnit == _unit)
            {
                _controlUnit = true;
            }
        }
    }
}