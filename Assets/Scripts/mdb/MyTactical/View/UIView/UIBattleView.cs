using System.Collections.Generic;
using UnityEngine;

using mdb.MyTactial.Controller;
using mdb.MyTactial.Model;

namespace mdb.MyTactial.View.UIView
{
    [RequireComponent(typeof(BattleController))]
    public class UIBattleView : MonoBehaviour
    {
        public static UIBattleView instance;
        public UICellView[] cellViews;

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
    }
}