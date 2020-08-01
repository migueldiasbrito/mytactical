using System;

namespace mdb.MyTactial.Model
{
    [Serializable]
    public class Unit
    {
        public Team Team { get { return _team; } set { _team = value; } }
        public Cell Cell { get { return _cell; } set { _cell = value; } }

        [NonSerialized]
        private Team _team;
        [NonSerialized]
        private Cell _cell;

        public Unit(Team team, Cell cell)
        {
            _team = team;
            _cell = cell;
        }
    }
}