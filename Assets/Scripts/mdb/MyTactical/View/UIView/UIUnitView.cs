using UnityEngine;
using UnityEngine.UI;

using mdb.MyTactial.Controller;
using mdb.MyTactial.Model;

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
        }

        private void UnitStateChanged(bool active)
        {
            _image.color = active ? Color.green : _defaultColor;
        }
    }
}