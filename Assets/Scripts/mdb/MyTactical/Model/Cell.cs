using System;
using UnityEngine;

namespace mdb.MyTactial.Model
{
    [Serializable]
    public class Cell
    {
        public delegate void StateChanged(bool active);
        public event StateChanged StateChangedCallback;

        public Unit Unit { get { return _unit; } }
        public Cell[] AdjacentCells { get { return _adjacentCells; } set { _adjacentCells = value; } }

        [NonSerialized]
        public Unit _unit;
        [NonSerialized]
        Cell[] _adjacentCells;

        [SerializeField]
        private int _terrainEffect = 0;

        private bool _active;

        public bool UnitEnter(Unit unit)
        {
            if (_unit != null) { return false; }

            if(unit.Cell != null)
            {
                unit.Cell.UnitExit();
            }

            _unit = unit;
            unit.Cell = this;

            return true;
        }

        public void UnitExit()
        {
            _unit = null;
        }

        public void SetActive(bool active)
        {
            _active = active;
            StateChangedCallback?.Invoke(_active);
        }

        public bool IsActive()
        {
            return _active;
        }
    }
}
