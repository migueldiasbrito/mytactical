using UnityEngine;
using System.Collections.Generic;

using mdb.MyTactial.Model;
using mdb.StateMachine;

namespace mdb.MyTactial.Controller
{
    public class BattleController : MonoBehaviour
    {
        public static BattleController instance;

        public Battle Battle { get { return _battle; } set { _battle = value; } }

        [SerializeField]
        private Battle _battle;

        private StateMachine.StateMachine _stateMachine;

        private int START_BATTLE;
        private int START_FIRST_TURN;
        private int START_FIRST_UNIT_TURN;
        private int MOVE_UNIT;
        private int END_UNIT_TURN;
        private int START_NEXT_UNIT_TURN;
        private int END_TURN;
        private int START_NEW_TURN;
        private int END_BATTLE;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            BuildStateMachine();
        }

        private void OnDestroy()
        {
            instance = null;
        }

        private void BuildStateMachine()
        {
            State StartBattle = new State();
            StartBattle.OnEnter += OnStartBattle;

            State StartTurn = new State();
            StartTurn.OnEnter += OnStartTurn;

            State StartUnitTurn = new State();
            StartUnitTurn.OnEnter += OnStartUnitTurn;

            State MoveUnit = new State();
            State EndUnitTurn = new State();
            State EndTurn = new State();
            State EndBattle = new State();

            List<Transition> transitions = new List<Transition>();

            START_BATTLE = transitions.Count;
            transitions.Add(new Transition(null, StartBattle));

            START_FIRST_TURN = transitions.Count;
            transitions.Add(new Transition(StartBattle, StartTurn));

            START_FIRST_UNIT_TURN = transitions.Count;
            transitions.Add(new Transition(StartTurn, StartUnitTurn));

            MOVE_UNIT = transitions.Count;
            transitions.Add(new Transition(StartUnitTurn, MoveUnit));

            END_UNIT_TURN = transitions.Count;
            transitions.Add(new Transition(MoveUnit, EndUnitTurn));

            START_NEXT_UNIT_TURN = transitions.Count;
            transitions.Add(new Transition(EndUnitTurn, StartUnitTurn));

            END_TURN = transitions.Count;
            transitions.Add(new Transition(EndUnitTurn, EndTurn));

            START_NEW_TURN = transitions.Count;
            transitions.Add(new Transition(EndTurn, StartTurn));

            END_BATTLE = transitions.Count;
            transitions.Add(new Transition(EndTurn, EndBattle));

            _stateMachine = new StateMachine.StateMachine(transitions.ToArray());
            _stateMachine.MakeTransition(START_BATTLE);
        }

        private void OnStartBattle()
        {
            _stateMachine.MakeTransition(START_FIRST_TURN);
        }

        private void OnStartTurn()
        {
            Debug.Log("Here we want to make a ordered list to character for the turn");
            _stateMachine.MakeTransition(START_FIRST_UNIT_TURN);
        }

        private void OnStartUnitTurn()
        {
            Debug.Log("Here we want to see where can the selected unit move");
            _stateMachine.MakeTransition(MOVE_UNIT);
        }
    }
}