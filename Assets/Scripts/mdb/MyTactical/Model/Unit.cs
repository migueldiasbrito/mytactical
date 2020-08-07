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

        public string Name { get { return _name; } }
        public int CurrentHealthPoints { get { return _currentHealthPoints; } }
        public int Attack { get { return _attack; } }
        public int Defense { get { return _defense; } }
        public int Agility { get { return _agility; } }
        public int Movement { get { return _movement; } }

        [SerializeField]
        private string _name;
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

        public Unit(string name)
        {
            _name = name;
        }

        public Unit(string name, int healthPoints, int attack, int defense, int movement, int agility)
        {
            _name = name;
            _healthPoints = healthPoints;
            _attack = attack;
            _defense = defense;
            _movement = movement;
            _agility = agility;
        }

        public void Init()
        {
            _currentHealthPoints = _healthPoints;
        }

        public void DecreaseHealthPointsBy(int damage)
        {
            _currentHealthPoints = Math.Max(_currentHealthPoints - damage, 0);
        }

        public void SetState(State state)
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