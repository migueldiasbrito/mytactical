using System.Collections.Generic;

using mdb.StateMachine;

namespace mdb.MyTactial.Controller
{
    public class BattleStateMachine : StateMachine.StateMachine
    {
        public static BattleStateMachine instance;

        public int START_BATTLE { get; private set; }
        public int BUILD_MAP { get; private set; }
        public int PLACE_UNITS { get; private set; }
        public int START_FIRST_TURN { get; private set; }
        public int START_FIRST_UNIT_TURN { get; private set; }
        public int MOVE_UNIT { get; private set; }
        public int END_UNIT_TURN { get; private set; }
        public int START_NEXT_UNIT_TURN { get; private set; }
        public int END_TURN { get; private set; }
        public int START_NEW_TURN { get; private set; }
        public int END_BATTLE { get; private set; }

        public readonly State StartBattle = new State();
        public readonly State BuildMap = new State();
        public readonly State PlaceUnits = new State();
        public readonly State StartTurn = new State();
        public readonly State StartUnitTurn = new State();
        public readonly State MoveUnit = new State();
        public readonly State EndUnitTurn = new State();
        public readonly State EndTurn = new State();
        public readonly State EndBattle = new State();

        private void Awake()
        {
            instance = this;

            List<Transition> transitions = new List<Transition>();

            START_BATTLE = transitions.Count;
            transitions.Add(new Transition(null, StartBattle));

            BUILD_MAP = transitions.Count;
            transitions.Add(new Transition(StartBattle, BuildMap));

            PLACE_UNITS = transitions.Count;
            transitions.Add(new Transition(BuildMap, PlaceUnits));

            START_FIRST_TURN = transitions.Count;
            transitions.Add(new Transition(PlaceUnits, StartTurn));

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

            _transitions = transitions.ToArray();
        }

        private void OnDestroy()
        {
            instance = null;
        }
    }
}