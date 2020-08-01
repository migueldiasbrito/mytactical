using System;

namespace mdb.MyTactial.Model
{
    [Serializable]
    public class Cell
    {
        public Unit Unit { get { return _unit; } }
        public Cell[] AdjacentCells { get { return _adjacentCells; } set { _adjacentCells = value; } }

        [NonSerialized]
        Unit _unit;
        [NonSerialized]
        Cell[] _adjacentCells;

        public Cell(int totalAdjacentCells)
        {
            _adjacentCells = new Cell[totalAdjacentCells];
        }

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
    }
}
