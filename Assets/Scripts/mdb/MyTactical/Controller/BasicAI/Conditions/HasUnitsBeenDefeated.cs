using System.Collections.Generic;
using UnityEngine;

using mdb.MyTactial.Model;

namespace mdb.MyTactial.Controller.BasicAI.Conditions
{
    public class HasUnitsBeenDefeated : Condition
    {
        [SerializeField]
        private int[] _unitsIndex = new int[0];

        private List<Unit> _units;

        private void Start()
        {
            BattleStateMachine.instance.PlaceUnits.OnExit += OnPlaceUnitsExit;
            BattleStateMachine.instance.UnitDefeated.OnEnter += OnUnitDefeated;
        }

        private void OnPlaceUnitsExit()
        {
            _units = new List<Unit>();

            for(int index = 0; index < _unitsIndex.Length; index++)
            {
                _units.Add(Unit.Team.Units[index]);
            }
        }

        private void OnUnitDefeated()
        {
            if (_units.Contains(BattleController.instance.CurrentTarget))
            {
                _units.Remove(BattleController.instance.CurrentTarget);
                TestCondition = _units.Count == 0;
            }
        }
    }
}