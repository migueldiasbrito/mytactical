using System;

namespace MyTactial.Model
{
    [Serializable]
    public class Battle
    {
        public Cell[] cells;
        public Team[] teams;

        public Battle(int gridSize)
        {
            if (gridSize < 1)
            {
                throw new ArgumentException("Battle grid has to have more than 0 cells");
            }

            cells = new Cell[gridSize];
        }
    }
}