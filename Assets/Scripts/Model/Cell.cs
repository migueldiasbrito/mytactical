using System;

namespace MyTactial.Model
{
    [Serializable]
    public class Cell
    {
        [NonSerialized]
        public Unit unit;
    }
}
