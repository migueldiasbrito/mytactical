using System;
using UnityEngine;

namespace mdb.MyTactial.Model
{
    [Serializable]
    public class Unit
    {
        public delegate void StateChanged(bool active);
        public event StateChanged StateChangedCallback;

        public delegate void ChangePosition(Cell cell);
        public event ChangePosition ChangePositionCallback;

        public Team Team { get { return _team; } set { _team = value; } }
        public Cell Cell {
            get { return _cell; } 
            set { _cell = value; ChangePositionCallback?.Invoke(value); } 
        }

        public int Agility { get { return _agility; } }
        public int Movement { get { return _movement; } }

        [SerializeField]
        private int _agility = 1;
        [SerializeField]
        private int _movement = 1;

        [NonSerialized]
        private Team _team;
        [NonSerialized]
        private Cell _cell;

        private bool _active;

        public void SetActive(bool active)
        {
            _active = active;
            StateChangedCallback?.Invoke(_active);
        }

        public bool IsActive()
        {
            return _active;
        }
    }
}