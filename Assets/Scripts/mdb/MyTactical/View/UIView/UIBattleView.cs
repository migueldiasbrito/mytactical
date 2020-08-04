﻿using System.Collections.Generic;
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

            BattleStateMachine.instance.ActionsMenu.OnEnter += OnActionsMenu;
            BattleStateMachine.instance.ActionsMenu.OnExit += OnActionsMenuExit;
            BattleStateMachine.instance.Attack.OnEnter += OnAttack;
            BattleStateMachine.instance.UnitDefeated.OnEnter += OnUnitDefeated;

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
            _messages.Enqueue(BattleController.instance.CurrentUnit.Name + " attacks " + BattleController.instance.CurrentTarget.Name);

            /*if(BattleController.instance.CurrentTarget.GetState() == Unit.State.Dead)
            {
                _messages.Enqueue(BattleController.instance.CurrentTarget.Name + " defeated");
            }*/

            MessageText.text = _messages.Dequeue();
            MessageText.gameObject.SetActive(true);

            MessageButton.gameObject.SetActive(true);
        }

        private void OnUnitDefeated()
        {
            _messages.Enqueue(BattleController.instance.CurrentTarget.Name + " defeated");

            MessageText.text = _messages.Dequeue();
            MessageText.gameObject.SetActive(true);

            MessageButton.gameObject.SetActive(true);
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