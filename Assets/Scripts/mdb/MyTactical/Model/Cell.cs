﻿using System;

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
        
        private bool _active;

        public bool UnitEnter(Unit unit)
        {
            if (_unit != null) { return false; }

            _unit = unit;
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
