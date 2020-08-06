using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using mdb.MyTactial.Controller;
using mdb.MyTactial.Model;
using UnityEngine.SceneManagement;

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

        public Button MessageButton;
        public Text MessageText;

        private Dictionary<Cell, UICellView> _cellMap;

        private Queue<string> _messages = new Queue<string>();

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

            BattleStateMachine.instance.SelectAction.OnEnter += OnSelectAction;
            BattleStateMachine.instance.SelectAction.OnExit += OnSelectActionExit;
            BattleStateMachine.instance.Attack.OnEnter += OnAttack;
            BattleStateMachine.instance.UnitDefeated.OnEnter += OnUnitDefeated;
            BattleStateMachine.instance.EndBattle.OnEnter += OnEndBattle;

            BattleStateMachine.instance.EndBattle.OnClickEvent += OnEndBattleClick;

            AttackButton.onClick.AddListener(Attack);
            NoActionButton.onClick.AddListener(NoAction);

            MessageButton.onClick.AddListener(NextMessage);
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

        private void OnSelectAction()
        {
            if (!BattleController.instance.CurrentUnit.Team.IsAIControlled)
            {
                AttackButton.gameObject.SetActive(BattleController.instance.HasTargets());
                ActionsMenu.SetActive(true);
            }
        }

        private void OnSelectActionExit()
        {
            ActionsMenu.SetActive(false);
        }

        private void OnAttack()
        {
            _messages.Enqueue(BattleController.instance.CurrentUnit.Name + " attacks " + BattleController.instance.CurrentTarget.Name);
            NextMessage();
        }

        private void OnUnitDefeated()
        {
            _messages.Enqueue(BattleController.instance.CurrentTarget.Name + " defeated");
            NextMessage();
        }

        private void OnEndBattle()
        {
            _messages.Enqueue(BattleController.instance.CurrentUnit.Team.Name + " won");
            NextMessage();
        }

        private void OnEndBattleClick(object obj)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void Attack()
        {
            BattleStateMachine.instance.AddTransition(BattleStateMachine.instance.SELECT_TARGET);
        }

        private void NoAction()
        {
            BattleStateMachine.instance.AddTransition(BattleStateMachine.instance.NO_ACTION);
        }

        private void NextMessage()
        {
            if (_messages.Count > 0)
            {
                MessageText.text = _messages.Dequeue();
                MessageText.gameObject.SetActive(true);

                MessageButton.gameObject.SetActive(true);
            }
            else
            {
                MessageText.gameObject.SetActive(false);
                MessageButton.gameObject.SetActive(false);

                BattleStateMachine.instance.OnClick(null);
            }
        }
    }
}