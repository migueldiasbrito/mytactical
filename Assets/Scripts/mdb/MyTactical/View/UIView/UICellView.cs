using UnityEngine;
using UnityEngine.UI;

using mdb.MyTactial.Controller;
using mdb.MyTactial.Model;

namespace mdb.MyTactial.View.UIView
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(Button))]
    public class UICellView : MonoBehaviour
    {
        public int CellIndex;

        public Cell Cell { get; private set; }

        private Image _image;
        private Color _defaultColor;

        private Button _button;

        private void Start()
        {
            Cell = BattleController.instance.Battle.Cells[CellIndex];
            Cell.StateChangedCallback += CellStateChanged;

            _image = GetComponent<Image>();
            _defaultColor = _image.color;

            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);
        }

        private void CellStateChanged(bool active)
        {
            _image.color = active ? Color.black : _defaultColor;

            _button.enabled = !BattleController.instance.CurrentUnit.Team.IsAIControlled;
        }

        private void OnClick()
        {
            if (Cell.IsActive())
            {
                BattleStateMachine.instance.OnClick(Cell);
            }
        }
    }
}