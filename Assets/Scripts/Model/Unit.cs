using System;

namespace MyTactial.Model
{
    [Serializable]
    public class Unit
    {
        [NonSerialized]
        public Team team;
        [NonSerialized]
        public Cell cell;

        public Unit(Team team, Cell cell)
        {
            this.team = team;
            this.cell = cell;
        }
    }
}