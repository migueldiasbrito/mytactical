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

        void Start()
        {
            _unit = BattleController.instance.Battle.Teams[TeamIndex].Units[UnitIndex];

            _gridMovement = GetComponent<GridMovement>();
            _path = new Queue<Vector2>();
        }

        void Update()
        {
            if (_moving && _gridMovement.MoveTo == null)
            {
                if (_path.Count > 0)
                {
                    _gridMovement.MoveTo = _path.Dequeue();
                }
                else
                {
                    BattleStateMachine.instance.OnClick(_lastCell);
                    _lastCell = null;
                    _moving = false;
                }
            }
        }
    }
}