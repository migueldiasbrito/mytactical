using System;
using UnityEngine;

namespace mdb.MyTactial.Model
{
    [Serializable]
    public class Unit
    {
        public enum State { Idle, Active, Target, Dead }

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
        private int _healthPoints = 1;
        [SerializeField]
        private int _attack = 1;
        [SerializeField]
        private int _defense = 0;
        [SerializeField]
        private int _agility = 1;
        [SerializeField]
        private int _movement = 1;

        [NonSerialized]
        private Team _team;
        [NonSerialized]
        private Cell _cell;
        [NonSerialized]
        private int _currentHealthPoints;

        private State _state;

        public void Init()
        {
            _currentHealthPoints = _healthPoints;
        }

        public void GetAttackedBy(Unit unit)
        {
            _healthPoints -= Math.Max(unit._attack - _defense, 1);

            if (_healthPoints <= 0)
            {
                Cell.UnitExit();
                SetActive(State.Dead);
            }
        }

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