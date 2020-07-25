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
    }
}