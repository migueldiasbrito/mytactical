using System;
using UnityEngine;

namespace mdb.MyTactial.Model
{
    [Serializable]
    public class Action
    {
        public enum AttackType { Melee, OfensiveMagic, HealingMagic }
        public enum AttackTarget { Enemy, Team }

        public string Name { get { return _name; } }
        public int HealthPoints { get { return _healthPoints; } }
        public int MinRange { get { return _minRange; } }
        public int MaxRange { get { return _maxRange; } }
        public int Area { get { return _area; } }
        public int ManaCost { get { return _manaCost; } }
        public AttackType Type { get { return _type; } }
        public AttackTarget Target { get { return _target; } }

        [SerializeField]
        private string _name;

        [SerializeField]
        private int _healthPoints;

        [SerializeField]
        private int _minRange;

        [SerializeField]
        private int _maxRange;

        [SerializeField]
        private int _area;

        [SerializeField]
        private int _manaCost;

        [SerializeField]
        private AttackType _type;

        [SerializeField]
        private AttackTarget _target;

        public Action()
        {
            _name = "Melee attack";
            _healthPoints = 0;
            _minRange = 1;
            _maxRange = 1;
            _area = 1;
            _manaCost = 0;
            _type = AttackType.Melee;
            _target = AttackTarget.Enemy;
        }
    }
}