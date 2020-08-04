using System;
using UnityEngine;

namespace mdb.MyTactial.Model
{
    [Serializable]
    public class Unit
    {
        public enum State { Idle, Active, Target }

        public delegate void StateChanged(State state);
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

        private State _state;

        public void SetActive(State state)
        {
            _state = state;
            StateChangedCallback?.Invoke(_state);
        }

        public State GetState()
        {
            return _state;
        }
    }
}