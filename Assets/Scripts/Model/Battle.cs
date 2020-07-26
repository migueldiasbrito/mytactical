using System;
using UnityEngine;

namespace MyTactial.Model
{
    [Serializable]
    public class Battle
    {
        public Cell[] Cells { get { return _cells; } set { _cells = value; } }
        public Team[] Teams { get { return _teams; } set { _teams = value; } }

        [SerializeField]
        private Cell[] _cells;
        [SerializeField]
        private Team[] _teams;
    }
}