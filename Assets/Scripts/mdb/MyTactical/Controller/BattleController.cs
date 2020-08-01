using UnityEngine;

using mdb.MyTactial.Model;

namespace mdb.MyTactial.Controller
{
    public class BattleController : MonoBehaviour
    {
        public static BattleController instance;

        public Battle Battle { get { return _battle; } set { _battle = value; } }

        [SerializeField]
        private Battle _battle;

        private BattleStateMachine _stateMachine;

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
            _stateMachine = new BattleStateMachine();
            _stateMachine.StartBattle.OnEnter += OnStartBattle;
            _stateMachine.StartTurn.OnEnter += OnStartTurn;
            _stateMachine.StartUnitTurn.OnEnter += OnStartUnitTurn;

            _stateMachine.MakeTransition(_stateMachine.START_BATTLE);
        }

        private void OnStartBattle()
        {
            _stateMachine.MakeTransition(_stateMachine.START_FIRST_TURN);
        }

        private void OnStartTurn()
        {
            Debug.Log("Here we want to make a ordered list to character for the turn");
            _stateMachine.MakeTransition(_stateMachine.START_FIRST_UNIT_TURN);
        }

        private void OnStartUnitTurn()
        {
            Debug.Log("Here we want to see where can the selected unit move");
            _stateMachine.MakeTransition(_stateMachine.MOVE_UNIT);
        }
    }
}