using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using mdb.MyTactial.Controller;
using mdb.MyTactial.Model;

namespace mdb.MyTactial.View.UIView
{
    [RequireComponent(typeof(BattleController))]
    public class UIBattleView : MonoBehaviour
    {
        public static UIBattleView instance;
        public UICellView[] cellViews;

        public GameObject ActionsMenu;
        public Button AttackButton;
        public Button NoActionButton;

        public Button TextMessage;

        private Dictionary<Cell, UICellView> _cellMap;

        public Transform GetCellView(Cell cell)
        {
            if (_cellMap.ContainsKey(cell))
            {
                return _cellMap[cell].transform;
            }

            return null;
        }

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            BattleStateMachine.instance.BuildMap.OnExit += OnBuildMapExit;

            BattleStateMachine.instance.ActionsMenu.OnEnter += OnActionsMenu;
            BattleStateMachine.instance.ActionsMenu.OnExit += OnActionsMenuExit;
            BattleStateMachine.instance.Attack.OnEnter += OnAttack;

            AttackButton.onClick.AddListener(Attack);
            NoActionButton.onClick.AddListener(NoAction);

            TextMessage.onClick.AddListener(CloseAttackMessage);
        }

        private void OnDestroy()
        {
            instance = null;
        }

        private void OnBuildMapExit()
        {
            _cellMap = new Dictionary<Cell, UICellView>();

            foreach (UICellView cellView in cellViews)
            {
                _cellMap.Add(cellView.Cell, cellView);
            }
        }

        private void OnActionsMenu()
        {
            AttackButton.gameObject.SetActive (BattleController.instance.HasTargets()) ;

            ActionsMenu.SetActive(true);
        }

        private void OnActionsMenuExit()
        {
            ActionsMenu.SetActive(false);
        }

        private void OnAttack()
        {
            TextMessage.gameObject.SetActive(true);
        }

        private void Attack()
        {
            BattleStateMachine.instance.AddTransition(BattleStateMachine.instance.SELECT_TARGET);
        }

        private void NoAction()
        {
            BattleStateMachine.instance.AddTransition(BattleStateMachine.instance.NO_ACTION);
        }

        private void CloseAttackMessage()
        {
            TextMessage.gameObject.SetActive(false);
            BattleStateMachine.instance.AddTransition(BattleStateMachine.instance.END_ATTACK);
        }
    }
}