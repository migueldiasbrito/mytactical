using System;

namespace MyTactial.Model
{
    [Serializable]
    public class Battle
    {
        public Cell[] cells;
        public Team[] teams;

        public Battle(int gridSize, int totalTeams)
        {
            if (gridSize < 1)
            {
                throw new ArgumentException("Battle grid has to have more than 0 cells");
            }

            if (totalTeams < 1)
            {
                throw new ArgumentException("Battle has to have more than 0 teams");
            }

            cells = new Cell[gridSize];
            teams = new Team[totalTeams];
        }
    }
}