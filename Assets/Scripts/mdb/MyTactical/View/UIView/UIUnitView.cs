using UnityEngine;
using UnityEngine.UI;

using mdb.MyTactial.Controller;
using mdb.MyTactial.Model;
using mdb.Tools;

namespace mdb.MyTactial.View.UIView
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(Button))]
    public class UIUnitView : UnitView
    {
        public int TeamIndex;
        public int UnitIndex;

        private Unit _unit;

        private Image _image;
        private Color _defaultColor;

        private Button _button;

        private void Start()
        {
            _unit = BattleController.instance.Battle.Teams[TeamIndex].Units[UnitIndex];
            _unit.StateChangedCallback += UnitStateChanged;

            _image = GetComponent<Image>();
            _defaultColor = _image.color;

            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);

            BattleStateMachine.instance.PlaceUnits.OnExit += OnPlaceUnitExit;
        }

        private void UnitStateChanged(Unit.State state)
        {
            switch (state)
            {
                case Unit.State.Idle:
                    _image.color = _defaultColor;
                    _button.enabled = false;
                    break;
                case Unit.State.Active:
                    _image.color = Color.green;
                    _button.enabled = false;
                    break;
                case Unit.State.Target:
                    if (_defaultColor == Color.red)
                    {
                        _image.color = Color.magenta;
                    }
                    else if (_defaultColor == Color.blue)
                    {
                        _image.color = Color.cyan;
                    }
                    _button.enabled = true;
                    break;
                case Unit.State.Dead:
                    _image.enabled = false;
                    _button.enabled = false;
                    break;
            }
        }

        private void OnPlaceUnitExit()
        {
            _unit.ChangePositionCallback += ChangePosition;
        }

        private void ChangePosition(Cell cell)
        {
            transform.SetParent(UIBattleView.instance.GetCellView(cell));

            RectTransformTools.SetTop((RectTransform)transform, 0);
            RectTransformTools.SetLeft((RectTransform)transform, 0);
            RectTransformTools.SetBottom((RectTransform)transform, 0);
            RectTransformTools.SetRight((RectTransform)transform, 0);
        }

        private void OnClick()
        {
            if (_unit.GetState() == Unit.State.Target && !BattleController.instance.CurrentUnit.Team.IsAIControlled)
            {
                BattleStateMachine.instance.OnClick(_unit);
            }
        }

        public override void DoPath(Cell[] cells)
        {
            BattleStateMachine.instance.OnClick(cells[cells.Length - 1]);
        }
    }
}