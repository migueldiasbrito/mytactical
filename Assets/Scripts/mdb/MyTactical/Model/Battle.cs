using System;
using UnityEngine;

namespace mdb.MyTactial.Model
{
    [Serializable]
    public class Battle
    {
        [Serializable]
        public struct CellAdjacentsBuilder
        {
            public int[] AdjacentCells;
        }

        public Cell[] Cells { get { return _cells; } }

        [SerializeField]
        private Cell[] _cells;
        
        public Team[] Teams { get { return _teams; } }

        [SerializeField]
        private Team[] _teams;

        [SerializeField]
        private int[] _initialPositions;

        [SerializeField]
        private CellAdjacentsBuilder[] _adjacentsBuilders;

        public Battle(Cell[] cells, CellAdjacentsBuilder[] adjacentsBuilders, Team[] teams, int[] initialPositions)
        {
            _cells = cells;
            _teams = teams;

            _adjacentsBuilders = adjacentsBuilders;
            _initialPositions = initialPositions;
        }

        public void BuildAdjacents()
        {
            for(int cellIndex = 0; cellIndex < _cells.Length; cellIndex++)
            {
                if (cellIndex >= _adjacentsBuilders.Length)
                {
                    Debug.LogWarning("Adjacents not defined for all cells");
                    break;
                }

                int[] adjacentIndexes = _adjacentsBuilders[cellIndex].AdjacentCells;
                _cells[cellIndex].AdjacentCells = new Cell[adjacentIndexes.Length];

                for(int adjacentIndex = 0; adjacentIndex < adjacentIndexes.Length; adjacentIndex++)
                {
                    if(adjacentIndexes[adjacentIndex] < 0 || adjacentIndexes[adjacentIndex] >= _cells.Length)
                    {
                        Debug.LogWarning("Adjacent outside of bounds");
                        continue;
                    }

                    _cells[cellIndex].AdjacentCells[adjacentIndex] = _cells[adjacentIndexes[adjacentIndex]];
                }
            }
        }

        public void SetUnits()
        {
            for (int teamIndex = 0, positionIndex = 0; teamIndex < _teams.Length; teamIndex++)
            {
                for (int unitIndex = 0; unitIndex < _teams[teamIndex].Units.Length; unitIndex++, positionIndex++)
                {
                    _teams[teamIndex].Units[unitIndex].Team = _teams[teamIndex];
                }
            }
        }

        public void PlaceUnits()
        {
            for (int teamIndex = 0, positionIndex = 0; teamIndex < _teams.Length; teamIndex++)
            {
                for (int unitIndex = 0; unitIndex < _teams[teamIndex].Units.Length; unitIndex++, positionIndex++)
                {
                    if (positionIndex >= _initialPositions.Length)
                    {
                        Debug.LogWarning("Not enough cell sets for all team's units initial positions");
                        break;
                    }

                    Unit unit = _teams[teamIndex].Units[unitIndex];
                    Cell cell = _cells[_initialPositions[positionIndex]];

                    if (cell.UnitEnter(unit))
                    {
                        unit.Cell = cell;
                    }
                    else
                    {
                        Debug.LogWarning("Multiple units on the same cell");
                    }
                }
            }
        }
    }
}