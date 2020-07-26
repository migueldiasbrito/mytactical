using System;

namespace MyTactial.Model
{
    [Serializable]
    public class Cell
    {
        public Unit Unit { get { return _unit; } }

        [NonSerialized]
        Unit _unit;

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
