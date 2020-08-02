using UnityEngine;
using UnityEngine.UI;

using mdb.MyTactial.Controller;
using mdb.MyTactial.Model;
using mdb.Tools;

namespace mdb.MyTactial.View.UIView
{
    [RequireComponent(typeof(Image))]
    public class UIUnitView : MonoBehaviour
    {
        public int TeamIndex;
        public int UnitIndex;

        private Unit _unit;

        private Image _image;
        private Color _defaultColor;

        private void Start()
        {
            _unit = BattleController.instance.Battle.Teams[TeamIndex].Units[UnitIndex];
            _unit.StateChangedCallback += UnitStateChanged;

            _image = GetComponent<Image>();
            _defaultColor = _image.color;

            BattleStateMachine.instance.PlaceUnits.OnExit += OnPlaceUnitExit;
        }

        private void UnitStateChanged(bool active)
        {
            _image.color = active ? Color.green : _defaultColor;
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
    }
}